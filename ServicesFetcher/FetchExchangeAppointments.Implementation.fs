module internal ServicesFetcher.FetchExchangeAppointments.Implementation

open ServicesFetcher.FetchExchangeAppointments.InternalTypes

open Result

// ======================================================
// This file contains the final implementation for the FetchExchangeAppointments
//
// There are two parts:
// * the first section contains the (type-only) definitions for each step
// * the second section contains the implementations for each step
//   and the implementation of the overall workflow
// ======================================================

let fetchFolderCalendarItems: FetchFolderCalendarItems = fun config query ->

    (*
        let operation = buildFindOperation
        let xml = performOperation operation
        let dto = deserialize<Dto> xml
    *)
    (Ok List.empty)

let fetchFullCalendarItems: FetchFullCalendarItems = fun config calendarItems ->
    (Ok List.empty)

let toAppointment: ToAppointment = fun item ->
    let appointment = {
        Id = item.Id
        TextBody = item.Body
        Subject = item.Subject
        Location = item.Location
    }

    appointment


// ========================
// * Fetch workflow itself
// ========================
let fetch : Fetch = fun config query ->

    let fetchFolder = (fetchFolderCalendarItems config query)
    let fetchFullCalendar = (fetchFullCalendarItems config)
    let toAppointments = (List.map toAppointment)

    fetchFolder
    |> bind fetchFullCalendar // binds the input (witch is a result) with the Ok value or red-track
    |> map toAppointments // toAppointments not return result; so we map it to make a result if Ok or red-track
