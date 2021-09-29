open System
open Common.Types
open ServicesFetcher.FetchExchangeAppointments

open System.Runtime.Serialization


let exchangeConfig : Exchange.Types.Config = {
    Endpoint = (Uri "https://correo.aytogetafe.es/EWS/exchange.asmx")
    User = "zammad"
    Password = "6w7y1,s5T,U"
}

let envelopeReal = """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><a><soap:Header><t:ServerVersionInfo MajorVersion="8" MinorVersion="2" MajorBuildNumber="254" MinorBuildNumber="0" xmlns:t="http://schemas.microsoft.com/exchange/services/2006/types" /></soap:Header><soap:Body><m:FindItemResponse xmlns:t="http://schemas.microsoft.com/exchange/services/2006/types" xmlns:m="http://schemas.microsoft.com/exchange/services/2006/messages"><m:ResponseMessages><m:FindItemResponseMessage ResponseClass="Success"><m:ResponseCode>NoError</m:ResponseCode><m:RootFolder TotalItemsInView="1" IncludesLastItemInRange="true"><t:Items><t:CalendarItem><t:ItemId Id="AAAqAHphbW1hZC5nZXN0b3JkZWluY2lkZW5jaWFzQGF5dG8tZ2V0YWZlLm9yZwFRAAiI2YE4/EJwAEYAAAAAi5aaiDZmz0iV7HV3uEfoiwcAEgOljLDEfECtxHnPoQfitAASyUXctAAAEgOljLDEfECtxHnPoQfitAASyUaJLwAAEA==" ChangeKey="DwAAABYAAAASA6WMsMR8QK3Eec+hB+K0ABLJVl8X" /><t:Subject>Borrado ficheros de log</t:Subject><t:Location>Dept. INF.</t:Location></t:CalendarItem></t:Items></m:RootFolder></m:FindItemResponseMessage></m:ResponseMessages></m:FindItemResponse></soap:Body></a></soap:Envelope>"""

[<EntryPoint>]
let main argv =

    (*
    let itemId : FindItem.Types.ItemIdData = { Id = "XXXX"; ChangeKey="CHJEWASD"}
    let appointment : FindItem.Types.AppointmentDto = { ItemId=itemId; Location="Get";Subject="Sub"}
    let appointmentEnvelop = Soap.Helpers.buildEnvelope [appointment]
    let envelopeXml = Soap.serializeXml appointmentEnvelop
    
    printfn "AppointmentDTO: %A\nXml:%s" appointment envelopeXml

    let obj = Soap.deserializeXml<Soap.Types.Envelope<FindItem.Types.AppointmentDto>> envelopeXml

    printfn "AppointmentDTO Deserialized: %A" obj
    *)


    (*
    
    let soapAppointmentResult = Soap.deserializeXml<Soap.Types.Envelope<FindItem.Types.AppointmentDto>> envelopeReal

    printf "DEBUIG: %s" envelopeReal
    match soapAppointmentResult with
          | Ok item -> printfn "Appointment deserialized OK:\n %A" item
          | Error err -> printfn "[-START DESERIALIZATION:ERROR]\n%s\n[END-DESERIALIZATION ERROR]" (WorkflowError.format err)

    

    let appointmentsResult = Exchange.Ews.fetchWeeklyAppointments exchangeConfig
    match appointmentsResult with
             | Ok items -> printfn "Appointment deserialized OK:\n %A" items
             | Error err -> printfn "[-START DESERIALIZATION:ERROR]\n%s\n[END-DESERIALIZATION ERROR]" (WorkflowError.format err)
    *)

    0