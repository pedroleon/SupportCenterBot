namespace SupportCenter.PushRecurrentTickets

open SupportCenter.PushRecurrentTickets

// ======================================================
// This file contains the logic for working with data transfer objects (DTOs)
//
// Each type of DTO is defined using primitive, serializable types
// and then there are `toDomain` and `fromDomain` functions defined for each DTO.s
// ======================================================


type UncommitedTicketDto = {
    Id: string option
    ExternalId: string
    Location: string
    Subject: string
    TextBody: string
}


module internal UncommitedTicketDto =

    let toDomain (dto: UncommitedTicketDto) : UncommitedTicket =
        let data : TicketData = {
           Location = dto.Location
           Subject = dto.Subject
           TextBody = dto.TextBody
        }
        let obj : UncommitedTicket = {
            Id = (ExternalId dto.ExternalId)
            Data = data
        }

        obj