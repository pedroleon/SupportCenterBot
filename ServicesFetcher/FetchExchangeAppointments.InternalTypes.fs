module internal ServicesFetcher.FetchExchangeAppointments.InternalTypes

open System
open Common.Types

type SoapOperation = {
    Description: string
    Xml: string
    SoapActionHeader: Uri
}

type XmlString = string

type FolderCalendarItem = {

    Id: string
    Subject: string
    Location: string
}

type FullCalendarItem = {

    Id: string
    Subject: string
    Location: string
    Body: string
}


// ==========
// * Internal API method signatures
// ==========

type FetchFolderCalendarItems = Config -> Query -> Result<FolderCalendarItem list, FetchError> // output
type FetchFullCalendarItems = Config -> FolderCalendarItem list -> Result<FullCalendarItem list, FetchError>
type ToAppointment = FullCalendarItem -> Appointment