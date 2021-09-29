module ServicesFetcher.FetchExchangeAppointments.Api

// ======================================================
// * This file contains:
// * - The workflow API spec
// * - The complete fetch workflow implementation
// ======================================================


// public method to access the workflow
let fetch : Fetch = fun config query -> Implementation.fetch config query
    