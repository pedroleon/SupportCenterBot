namespace Exchange

open Types
open Common.Types
open Types.Dto

open System


module Ews =

    let fetchAppointmentsBetween (startDate: DateTime) (endDate:DateTime) (config:Config) : Result<AppointmentDto list, WorkflowError> =
        let xmlPayload = FindItem.configOperation startDate endDate
                            |> Helpers.performSoapOperation config

        let dtoListResult = xmlPayload
                            |> Result.bind Soap.deserializeXml<Soap.Types.Envelope<AppointmentDto>>
                            |> Result.bind (fun (envelope) -> (Ok (Soap.Helpers.unpackEnvelopeItems envelope)))
                            |> Result.bind (fun (itemsArray) -> (Ok (Array.toList itemsArray)))

        dtoListResult

        
        
    let fetchTodayAppointments (config: Config) = 
        let today = DateTime.Now 
        let tomorrow = today.AddDays(1.0)

        fetchAppointmentsBetween today tomorrow config 

    
    let fetchWeeklyAppointments (config: Config) = 
        let today = DateTime.Now 
        let tomorrow = today.AddDays(7.0)

        fetchAppointmentsBetween today tomorrow config 
