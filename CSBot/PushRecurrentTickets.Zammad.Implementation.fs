module internal SupportCenterBot.PushRecurrentTickets.Zammad.Implementation

open Common.Types
open SupportCenter.PushRecurrentTickets
open SupportCenter.PushRecurrentTickets.Dependencies

open Zammad.Client
open Zammad.Client.Resources

let ticketMatcher (id: TicketIdentification ) (ticket: Ticket ): bool =
    match id with
        | External (ExternalId id ) -> ticket.Note = id
        | Id (TicketId id ) -> ticket.Id = id

let toDomain (t: Ticket) : CommitedTicket =
    let data: TicketData = {
            Subject = t.Title
            Location = "Lo tenemos en el cuerpo, amigo"
            TextBody = "No existe aquí"
    }
    let obj: CommitedTicket = {
        Id = (TicketId t.Id)
        Note = t.Note
        Data = data
    }

    obj





type PushZammadTicket = ZammadConfig -> PushTicket
type CacheZammadTickets = ZammadConfig -> Result<FetchTicket, WorkflowError>

let private instanceClient config =
    let account = ZammadAccount.CreateTokenAccount (config.Endpoint, config.AuthToken)
    account.CreateTicketClient()


let internal pushTicketOperation : PushZammadTicket  =
    fun config -> 

        let client = instanceClient config
        fun ticket ->

            let data = ticket.Data
            let (ExternalId externalId) = ticket.Id

            let t = new Ticket();
            t.Title <- data.Subject
            t.Note <- externalId
            t.CustomerId <- 17
            t.OwnerId <- 17
            t.GroupId <- 1

            let ta = new TicketArticle();
            ta.Body <- sprintf "Lugar: %s\nDescripción:%s" data.Location data.TextBody
            ta.Type <- "note"
            ta.Subject <- "Anotaciones sobre ubicación y asunto"
            

            let createTicket = client.CreateTicketAsync(t, ta) |> Async.AwaitTask

            try 
                let ticket = Async.RunSynchronously createTicket
                (Ok (toDomain ticket))
            with
                | ex -> (Error (PushTicketError.CommunicationError (CommunicationError.ConnectionError (WorkflowError.withDetail "Error pushing ticket" ex.Message ))))


let internal cacheZammadTickets : CacheZammadTickets =

    fun config -> 
        let client = instanceClient config
        let lastDayQuery =  "created_at:<now AND created_at:>now-24h"
        let maxItems = 1000
        try
            let search = client.SearchTicketAsync(lastDayQuery,maxItems);
            let fetchTickets = search |> Async.AwaitTask
            let tickets = Async.RunSynchronously fetchTickets

            let backedFetch = fun ticketId ->

                       let findFunction = (ticketMatcher ticketId)
                       let ticket = Seq.tryFind findFunction tickets

                       match ticket with
                           | Some ticket -> (Ok (toDomain ticket))
                           | None -> (Error (FetchTicketError.NotFoundError ticketId))

            (Ok backedFetch)
        with
         | ex ->
            let endpoint = config.Endpoint.ToString()
            let errorMsg = sprintf "Error while initial fetch and cache of tickets from zammad at '%s'" endpoint

            (Error (WorkflowError.withDetail errorMsg ex.Message))


let commitWorkflowWithZammad (config: ZammadConfig) : Result<Api.CommitTicketApi, WorkflowError> =

    // Will return a function with cached tickets if Ok
    let fetchOperation =  cacheZammadTickets config
      
    match fetchOperation with
        | Ok fetchOperation ->

            let pushOperation = pushTicketOperation config
            (Ok (Api.composeCommitTicket fetchOperation pushOperation))

        | Error error -> (Error error)
            