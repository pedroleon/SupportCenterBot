module internal SupportCenter.PushRecurrentTickets.Implementation

open Common.Types
open InternalTypes



let internal communicationErrorToWorkflowError : CommunicationErrorToWorkflowError =
    fun error ->
        match error with
            | AuthorizationError wError -> wError
            | ConnectionError wError -> wError
            | DeserializationError wError-> wError


// ========================
// Error decapsulation utils
// ========================
let toWorkflowError : ToWorkflorError =
    fun error ->

        match error with
            | FetchError error ->
                match error with
                    | FetchTicketError.CommunicationError error -> communicationErrorToWorkflowError error                         
                    | NotFoundError ticketId ->
                        match ticketId with
                            | External (ExternalId externalId) -> WorkflowError.withDetail "Ticket not found in service" (sprintf "Ticket ExternalId = %s" externalId)
                            | Id (TicketId ticketId) -> WorkflowError.withDetail "Ticket not found in service" (sprintf "Ticket = %i" ticketId)
            | PushError error ->
                match error with
                    | PushTicketError.AlreadyCommitedError error -> error
                    | PushTicketError.CommunicationError error -> communicationErrorToWorkflowError error

// ========================
// overall Workflow 
// ========================

let commitTicket
    (fetch : FetchTicket) // dependency
    (push : PushTicket) // dependency
    : CommitTicket =

    fun uncommitedTicket ->

        let externalTicketId = (TicketIdentification.External uncommitedTicket.Id)
        let ticketResult = fetch externalTicketId

        match ticketResult with
            | Ok fetchedTicket -> (Ok fetchedTicket)
            | Error error ->
                match error with
                    | FetchTicketError.NotFoundError ticketId -> push uncommitedTicket |> Result.mapError PushRecurrentTicketsError.PushError
                    | FetchTicketError.CommunicationError error -> (Error (PushRecurrentTicketsError.FetchError (FetchTicketError.CommunicationError error)))
