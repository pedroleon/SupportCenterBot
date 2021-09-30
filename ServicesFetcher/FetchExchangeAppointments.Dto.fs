namespace ServicesFetcher.FetchExchangeAppointments

open Common.Types
open InternalTypes
open System.Xml.Serialization

// ======================================================
// This file contains the logic for working with data transfer objects (DTOs)
//
//
// Each type of DTO is defined using primitive, serializable types
// and then there are `toDomain` and `fromDomain` functions defined for each DTO.
//
// ======================================================


// =========================
// DTOs of FetchExchangeAppointments workflow
//
// =========================
[<CLIMutable>]
type ItemIdData = {
[<XmlAttribute>] Id: string
[<XmlAttribute>] ChangeKey: string
}

[<CLIMutable>]
[<XmlTypeAttribute("CalendarItem")>]
type FolderCalendarItemDto = {

        [<XmlElement("ItemId")>]
        ItemId: ItemIdData

        [<XmlElement("Location")>]
        Location: string

        [<XmlElement("Subject")>]
        Subject: string
    }

[<RequireQualifiedAccess>]
module internal FolderCalendarItemDto = 

    let toPartiallyLoadedAppointment (dto: FolderCalendarItemDto) : Result<FolderCalendarItemDto, WorkflowError> =
        (Error (WorkflowError.withError "Not implemented"))
