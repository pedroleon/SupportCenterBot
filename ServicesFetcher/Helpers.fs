namespace Exchange

open Common.Types
open Types
open InternalTypes

open System.Net
open System
open System.IO

module internal Helpers = 

    // Web helpers 
    let private encodeBytes (payload: string) : byte[] = 
        Text.Encoding.UTF8.GetBytes payload

    let private getResponseBody (response: WebResponse) : string =

        use stream = response.GetResponseStream()
        use reader = new IO.StreamReader(stream)    
        reader.ReadToEnd()

    let private configureSoapRequest (operationEnvelope: SoapOperation) (config: Config) : WebRequest= 

        let req = WebRequest.Create config.Endpoint
        let payload = encodeBytes operationEnvelope.Xml
        
        let soapOperationUri = operationEnvelope.SoapActionHeader.ToString()
        let soapActionHeader = sprintf "SOAPACTION: \"%s\"" soapOperationUri

        req.ContentLength <- (int64 payload.Length)
        req.Credentials <- NetworkCredential (config.User, config.Password)
        
        req.Method <- "POST"  
        req.ContentType <- "text/xml;charset=UTF-8"
        req.Headers.Add(soapActionHeader)

        let reqStream = req.GetRequestStream()
        reqStream.Write (ReadOnlySpan payload)
        reqStream.Close()

        req

    let private executeSoapRequest (request: WebRequest) : string =
        use response = request.GetResponse()
        use stream = response.GetResponseStream()
        use reader = new StreamReader(stream)
        reader.ReadToEnd()

    let private handleRequestWebException (ex: WebException) =
        let protocolError = "EWS server returned error response"
        match ex.Status with
            | WebExceptionStatus.ProtocolError -> 
                let errorRequestBody = getResponseBody ex.Response
                let failure = WorkflowError.withDebug protocolError ex.Message errorRequestBody
                (Error failure)
            | _ -> 
                let failure = WorkflowError.withDetail protocolError ex.Message
                (Error failure)

    let performSoapOperation  (config: Config) (operation: SoapOperation): Result<XmlString, WorkflowError> =
      
        // Bypass certificate validation
        ServicePointManager.ServerCertificateValidationCallback <- (fun _ _ _ _ -> true)

        try
            let payload = config
                            |> configureSoapRequest operation
                            |> executeSoapRequest
                      
            (Ok payload)
        with
        | :? WebException as ex -> handleRequestWebException ex // Errores de protocolo    
        | exn as ex -> (Error (WorkflowError.withDetail "Unexpected network error" ex.Message)) // Errores de infrastructura
