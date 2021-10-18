
open Common.Types
open SupportCenter.PushRecurrentTickets
open SupportCenterBot.PushRecurrentTickets
open SupportCenterBot.Intranet
open ServicesFetcher.FetchExchangeAppointments
open SupportCenterBot.ParseCommandLineOptions

open System
open System.IO


let zammadConfig : ZammadConfig = {
    Endpoint = Uri("https://soporte.wearestrings.com/")
    AuthToken = "NzEtzYThEYCBTVC-iEQEOdLhgn_x3HhYE9n1X57i9aouxne4WpyboCtvHKs99f3c" // "G5q5dF-3efan98EnEcavJ9QW5Lp3S8YPRky3TpV75k5Jdzl_Rt2SJp-J5OiSbHJm"
    TicketsOwnerEmail = "bot@wearestrings.com"
    BotUserAgent = "Strings Bot"
}

let exchangeConfig : Config = {
    Endpoint = Uri("https://correo.aytogetafe.es/EWS/exchange.asmx")
    User = "zammad"
    Password = "6w7y1,s5T,U"
}

let toUncommitedTicketDto (agentName: string) (appointment: Appointment) : UncommitedTicketDto =
    let uncommitedTicket : UncommitedTicketDto = {
          Id = None
          ExternalId = appointment.Id
          Subject = appointment.Subject
          TextBody= sprintf "%s\n\n\n Ticket enviado por @Getage_CSBot desde host '%s'" appointment.TextBody agentName
          Location = appointment.Location
      }
    uncommitedTicket

type WorkflowAggratedError =
    | CommitRecurrentTicketsError of PushRecurrentTicketsError
    | FetchAppointmentsError of FetchError

let checkDiskSpace () =
    printfn "[*] Checking disc space...."
    let allDrives = DriveInfo.GetDrives();
    for drive in allDrives do
        let info = sprintf " Drive %s at %s" drive.Name drive.VolumeLabel
        let extInfo =
            match drive.IsReady with
            | true ->
                let percentage =
                    if drive.AvailableFreeSpace > (int64 0) then
                        ((float (drive.TotalSize - drive.AvailableFreeSpace)) / (float drive.TotalSize)) * 100.0
                    else
                        (float 0.0)

                sprintf "%s : available %f percent of (%i/%i) bytes" info percentage drive.AvailableFreeSpace drive.TotalSize 
            | false -> info

        printfn "%s" extInfo

    ()

(*



    let todayQuery : Query = Api.buildTodayQuery()

    // let fetchAppointments = fetchTodayAppointments exchangeConfig
    let toUncommitedTicketDtoWithHostname = (toUncommitedTicketDto hostName)
    let commitWorkflow = Zammad.Implementation.commitWorkflowWithZammad zammadConfig
    let fetchAppointmentsWorkflow =
        fun query ->
            Api.fetchAppointments exchangeConfig query
            |> Result.mapError FetchAppointmentsError

    let isOk result =
        match result with
            | Ok _ -> true
            | Error _ -> false

    match commitWorkflow with
        | Ok commitWorkflow ->

            let fetchAndPushtResult =
                fetchAppointmentsWorkflow todayQuery
                |> Result.bind (fun appointments -> (Ok (List.map toUncommitedTicketDtoWithHostname appointments)))
                |> Result.bind (fun tickets -> (Ok (List.map commitWorkflow tickets )))
                       

            match fetchAndPushtResult with
                | Ok ticketsResults ->
                    let format = "dd-MM-yyyy HH:mm"
                    let startDate = todayQuery.StartDate.ToString(format)
                    let endDate = todayQuery.EndDate.ToString(format)
                    printfn "Found %i tickets in user %s's Calendar between %s and %s" (List.length ticketsResults) exchangeConfig.User startDate endDate 
                    printfn "Commited %i tickets to Zammad" (List.length (List.filter isOk ticketsResults))
                    for ticketCommited in ticketsResults do 
                        match ticketCommited with
                            | Ok ticket ->
                                let (TicketId id ) = ticket.Id
                                printfn "[Ok] Ticket %i was pushed" id
                            | Error error ->
                                
                                printfn "[Error] Ticket could not be pushed. %s" ((Api.toWorkflowError error) |> WorkflowError.format)                    
                | Error error ->
                    match error with
                        | CommitRecurrentTicketsError error ->
                            printfn "[Error] Commit workflow failed. %s" ((Api.toWorkflowError error) |> WorkflowError.format)
                        | FetchAppointmentsError error ->
                            
                            printfn "[Error] Fetch workflow failed"
        | Error err ->
            err
            |> WorkflowError.format
            |> printfn "[Could not start zammad workflow configuration] %s"

*)
[<EntryPoint>]
let main argv =


    printfn "GETAFE BOT: Croneable Support Center Utilities"
    let hostName = HostName.create Environment.MachineName    
    let runOptions = Implementation.parseCommandLine hostName argv

    match runOptions.Help with
        | Print -> printfn "%s" (Implementation.help hostName)
        | Skip ->

            printfn "%s" (Implementation.Options.toString runOptions)

            match runOptions.CheckDrive with
                | [] -> ()
                | drives -> checkDiskSpace()
    
            match runOptions.PushIntranet with
                | Ignore -> ()
                | Push -> queryNews()
        
    0