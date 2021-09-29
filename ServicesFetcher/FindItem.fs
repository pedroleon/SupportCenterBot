namespace Exchange

open Common.Types
open Types
open InternalTypes

open System 
open System.Xml.Serialization

[<RequireQualifiedAccess>]
module FindItem = 

    let private xmlEnvelope (startDate: DateTime) (endDate: DateTime) : string =

        let ewsPattern = "yyyy-MM-ddT00:00:00-00:00"
        let startStr = startDate.ToString(ewsPattern)
        let endStr = endDate.ToString(ewsPattern)
        let calendarView = sprintf """<CalendarView MaxEntriesReturned="100" StartDate="%s" EndDate="%s"/>""" startStr endStr
      
        sprintf """<?xml version="1.0"?>
        <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
        <soap:Body>
            <FindItem xmlns="http://schemas.microsoft.com/exchange/services/2006/messages" xmlns:t="http://schemas.microsoft.com/exchange/services/2006/types" Traversal="Shallow">
            <ItemShape>
                    <t:BaseShape>IdOnly</t:BaseShape>
                    <t:AdditionalProperties>
                    <t:FieldURI FieldURI="item:ItemId"/>
                    <t:FieldURI FieldURI="item:Subject"/>
                    <t:FieldURI FieldURI="calendar:Location"/>
                    </t:AdditionalProperties>
            </ItemShape>
            %s
            <ParentFolderIds>
                <t:FolderId Id="AAAqAHphbW1hZC5nZXN0b3JkZWluY2lkZW5jaWFzQGF5dG8tZ2V0YWZlLm9yZwAuAAAAAACLlpqINmbPSJXsdXe4R+iLAQASA6WMsMR8QK3Eec+hB+K0ABLJRdy0AAA="/>
            </ParentFolderIds>
            </FindItem>
        </soap:Body>
        </soap:Envelope>""" calendarView


    let configOperation (startDate: DateTime) (endDate: DateTime) : SoapOperation = {
            Description = "FindCalendarAppointments"
            Xml = xmlEnvelope startDate endDate
            SoapActionHeader = Uri("http://schemas.microsoft.com/exchange/services/2006/messages/FindItem")
        }


     
   