namespace SupportCenter.PushRecurrentTickets

open System
open Common.Types

// ========================================
// * Workflow Input declarations
// ========================================

type ZammadConfig = {
    Endpoint: Uri
    AuthToken: string
    BotUserAgent: string
    TicketsOwnerEmail: string
}


type Config =
    | Zammad of ZammadConfig

// ========================================
// * Workflow Output declarations if success
// ========================================

type TicketId = TicketId of int
type ExternalId = ExternalId of string

type TicketIdentification =
    | Id of TicketId
    | External of ExternalId

type TicketData = {
    Subject: string
    Location: string
    TextBody: string
}

// Uncommited are those tickets which are not sync with the Ticketing system.
// They do not have TicketId because are not sync!
// Because of that needs a noteId (works as external reference) to allow checking their existance in some way
// before pushing them in Ticketing System
type UncommitedTicket = {
    Id: ExternalId
    Data: TicketData
}

type CommitedTicket = {
    Id: TicketId
    Data: TicketData
    Note: string
}

type Ticket =
    | Commited of CommitedTicket
    | Uncommited of UncommitedTicket


// ========================================
// * Error types declarations
// ========================================


type CommunicationError =
    | ConnectionError of WorkflowError
    | DeserializationError of WorkflowError
    | AuthorizationError of WorkflowError

type FetchTicketError =
    | NotFoundError of TicketIdentification
    | CommunicationError of CommunicationError
    
type PushTicketError =
    | AlreadyCommitedError of WorkflowError
    | CommunicationError of CommunicationError
    

type PushRecurrentTicketsError =
    | FetchError of FetchTicketError
    | PushError of PushTicketError

// ========================================
// * Workflows available
// ========================================

type CommitTicket = UncommitedTicket -> Result<CommitedTicket, PushRecurrentTicketsError>
type ToWorkflorError = PushRecurrentTicketsError -> WorkflowError