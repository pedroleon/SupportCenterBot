[<RequireQualifiedAccess>]
module SupportCenter.PushRecurrentTickets.Api

open Common.Types
open InternalTypes

// ===================
// workflow public methods
// ===================

type CommitTicketApi = UncommitedTicketDto -> Result<CommitedTicket, PushRecurrentTicketsError>


// Dummy implementation of fetch
let internal fetch : FetchTicket =
    fun productId ->
        match productId with
            | Id id -> (Error (FetchTicketError.NotFoundError productId))
            | External id ->  (Error (FetchTicketError.NotFoundError productId)) // dummy implementation

// Dummy implementation of push
let internal push : PushTicket =
    fun uncommitedTicket ->

        let (ExternalId ticketId ) = uncommitedTicket.Id
        (Error (PushTicketError.CommunicationError (ConnectionError (WorkflowError.withDetail "PushTicket to TMS implementation" (sprintf "This service is event not connected! so is very dificult to push ticket with external_id=%s " ticketId)))))


// ======================================
// workflow Error transformation helper ???
// ======================================
let toWorkflowError:  ToWorkflorError =

    fun error -> Implementation.toWorkflowError error

    
// ======================================
// workflow itself
// ======================================
    
let commitTicket : CommitTicketApi =

    fun uncommitedTicketDto ->

        let uncommitedTicket = UncommitedTicketDto.toDomain(uncommitedTicketDto)
        let workflow =
            Implementation.commitTicket
                fetch
                push
                
        workflow uncommitedTicket
