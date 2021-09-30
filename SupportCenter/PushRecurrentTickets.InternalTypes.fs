module internal SupportCenter.PushRecurrentTickets.InternalTypes

open Common.Types

// ========================================
// * Define workflow dependencies
// ========================================

// Checks if ticket exists in remote/local service (dependency)
// Those dependencies will make the workflow PURE and SIMPLER
// because the are passed as params and can be backed

type FetchTicket = TicketIdentification -> Result<CommitedTicket, FetchTicketError>
type PushTicket = UncommitedTicket -> Result<CommitedTicket, PushTicketError>

// =======================================
// * Internal utilities definition
type CommunicationErrorToWorkflowError = CommunicationError -> WorkflowError