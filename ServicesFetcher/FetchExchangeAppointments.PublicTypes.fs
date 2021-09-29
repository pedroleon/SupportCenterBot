namespace ServicesFetcher.FetchExchangeAppointments

open Common.Types
open System

// ========================================
// * Workflow Input declarations
// ========================================

type Config = {
    Endpoint: Uri
    User: string
    Password: string
}

type Query = {

    StartDate: DateTime
    EndDate: DateTime
}

// ========================================
// * Workflow Output declarations if success
// ========================================

type Appointment = {
    Id: string
    Subject: string
    Location: string
    TextBody: string
}


// ========================================
// * Error types declarations
// ========================================

type DeserializationError = DeserializationError of WorkflowError
type RequestError = RequestError of WorkflowError
type AuthenticationError = AuthenticationError of WorkflowError

type FetchError =
    | AuthenticationError of WorkflowError
    | RequestError of WorkflowError
    | DeserializationError of WorkflowError
    | ValidationError of ValidationError



// ========================================
// * Workflows available
// ========================================
type Fetch = Config -> Query -> Result<Appointment list, FetchError>