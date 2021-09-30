open System

open Common.Types
open SupportCenter.PushRecurrentTickets

[<EntryPoint>]
let main argv =

    printfn "======================="

    let ticketId = "Thisis-and-external-id-that-doesnot-exists" 
    let uncommitedTicket : UncommitedTicketDto = {
        Id = None
        ExternalId = ticketId
        Subject = "Dummy ticket!"
        TextBody= "Data about tasks"
        Location = "Dummy Dtp."
    }
    let commitResult = Api.commitTicket uncommitedTicket

    match commitResult with
        | Ok ticket ->
            let (TicketId id) = ticket.Id
            printfn "[Ok] Ticket %i was successfully commited" id 
        | Error err ->
            Api.toWorkflowError err
                |> WorkflowError.format
                |> printfn "[Error] %s"
    0