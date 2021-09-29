namespace SupportCenter.PushRecurrentTickets

open SupportCenter.PushRecurrentTickets

module Dto =

    type RecurrentTicketDto = {
        ExternalId: string
        Location: string
        Subject: string
        Body: string
    }

    module internal RecurrentTicketDto =

        let toUnvalidatedRecurrentTicket (dto:RecurrentTicketDto) : UnvalidatedRecurrentTicket =
            let domainObj : UnvalidatedRecurrentTicket = {
                    ExternalId = dto.ExternalId
                    Location = dto.Location
                    Subject = dto.Subject
                    Body = dto.Body
                }
        domainObj