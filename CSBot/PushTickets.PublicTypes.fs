namespace SupportCenterBot.PushTickets

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