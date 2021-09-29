namespace Exchange

open Common.Types

open System.IO
open System.Text
open System.Xml
open System.Xml.Serialization

[<RequireQualifiedAccess>]
module Soap =

    let toString (x: byte[]) = Encoding.UTF8.GetString x
    let toBytes (x : string) = Encoding.UTF8.GetBytes x

    let serializeXml<'a> (item : 'a) =

        let ns = new XmlSerializerNamespaces();
        ns.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");
        ns.Add("t", "http://schemas.microsoft.com/exchange/services/2006/types");
        ns.Add("m", "http://schemas.microsoft.com/exchange/services/2006/messages")


        let xmlSerializer = new XmlSerializer(typeof<'a>)
        use stream = new MemoryStream()
        xmlSerializer.Serialize(stream, item, ns)
        toString <| stream.ToArray()

    let deserializeXml<'a> (xml : string) =


        try 
            let xmlSerializer = new XmlSerializer(typeof<'a>)
            use stream = new MemoryStream(toBytes xml)
            let obj = xmlSerializer.Deserialize stream :?> 'a

            (Ok obj)
        with
        | ex -> (Error (WorkflowError.withDebug "Deserialization error" ex.Message xml))
    

    (*let deserializeEnvelope<'T> (xml: string) : Result<'T, WorkflowError> =

        try

            let stream = new StringReader(xml);
            let xmlSerializer = XmlSerializer(typeof<'T>);
            let reader = XmlReader.Create(stream)
            let obj = xmlSerializer.Deserialize(reader) :?> 'T
            *)

    module Types =



        [<CLIMutable>]
        [<XmlTypeAttribute("RootFolder")>]        
         type RootFolderData<'TItem> = {
             [<XmlElement(ElementName="TotalItemsInView")>] TotalItemsInView: int
             [<XmlArrayAttribute("Items", Namespace="http://schemas.microsoft.com/exchange/services/2006/types")>] Items: 'TItem[]
         }

         [<CLIMutable>] 
         [<XmlTypeAttribute("FindItemResponseMessage")>]
         type FindItemResponseMessageData<'TItem> = {
            
             [<XmlElement(ElementName="ResponseCode")>] ResponseCode: string
             RootFolder: RootFolderData<'TItem>
         }

         [<CLIMutable>] 
         [<XmlTypeAttribute("m:ResponseMessage")>]
         type ResponseMessagesData<'TItem> = {
            FindItemResponseMessage: FindItemResponseMessageData<'TItem>
         }

         [<CLIMutable>]
         [<XmlTypeAttribute("FindItemResponse", Namespace="http://schemas.microsoft.com/exchange/services/2006/messages")>]
         type FindItemResponseData<'TItem> = {
            
             ResponseMessages: ResponseMessagesData<'TItem> 
         }
            
         [<CLIMutable>]
         [<XmlTypeAttribute("Body", Namespace="http://schemas.microsoft.com/exchange/services/2006/messages")>]
         type BodyData<'TItem> = {
             FindItemResponse: FindItemResponseData<'TItem>
         }

         [<CLIMutable>]
         [<XmlTypeAttribute("ServerVersionInfo")>]
         type ServerVersionInfoData = {

            [<XmlAttribute>] MajorVersion: int
            [<XmlAttribute>] MinorVersion: int
            [<XmlAttribute>] MajorBuildNumber: int
            [<XmlAttribute>] MinorBuildNumber: int
         }

         [<CLIMutable>]
         [<XmlTypeAttribute("Header", Namespace="http://schemas.microsoft.com/exchange/services/2006/types")>]
         type HeaderData = {
            ServerVersionInfo: ServerVersionInfoData
         }
            
         [<CLIMutable>]
         [<XmlRoot(ElementName="Envelope", Namespace="http://schemas.xmlsoap.org/soap/envelope/")>]
         type Envelope<'TItem> = {

             Header: HeaderData
             Body: BodyData<'TItem>
         }

    module Helpers =

        open Types

        let unpackEnvelopeItems<'T> (envelope: Envelope<'T>) =
            envelope.Body.FindItemResponse.ResponseMessages.FindItemResponseMessage.RootFolder.Items

        let buildEnvelope<'T> (items: 'T list)  =

            let serverVersion : ServerVersionInfoData = {

                MinorVersion = 1
                MajorBuildNumber = 234
                MajorVersion = 10
                MinorBuildNumber = 100
            }
            let header : HeaderData = {

                ServerVersionInfo = serverVersion
            }

            let rootFolder : RootFolderData<'T> = {
                TotalItemsInView = List.length items
                Items = List.toArray items
            }
           
            let fiResponseMessage : FindItemResponseMessageData<'T> =  {
           
                ResponseCode = "OK"
                RootFolder = rootFolder
            }
 
            let responseMessage : ResponseMessagesData<'T> = {
                FindItemResponseMessage = fiResponseMessage
            }
           
            let fiResponse : FindItemResponseData<'T> = {
                ResponseMessages = responseMessage
            }
           
            let body : BodyData<'T> = {
                FindItemResponse = fiResponse
            }
           
            let envelope : Envelope<'T>  = {
                Header = header
                Body = body
            }
           
            envelope