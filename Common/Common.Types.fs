namespace Common

module Types =

    type ValidationError = {
        Attribute: string
        Errors: string list
    }

    type BasicError = {
        Error: string
    }
    type DetailedError = {
        Error: string
        Detail: string
    }
    type DebugError = {
        Error: string
        Detail: string 
        Debug: string 
    }

    type WorkflowError =
    | BasicError of BasicError
    | DetailedError of DetailedError
    | DebugError of DebugError

    [<RequireQualifiedAccess>]
    module WorkflowError = 

        let withError msg : WorkflowError = (BasicError { Error=msg })
        let withDetail msg detail : WorkflowError = (DetailedError { Error=msg; Detail= detail})
        let withDebug msg detail debug : WorkflowError = (DebugError { Error=msg; Detail= detail; Debug=debug })

        let format (workflowError: WorkflowError) =
            match workflowError with
                | DebugError error -> sprintf "[%s] %s\n%s" error.Error error.Detail error.Debug
                | DetailedError error -> sprintf "[%s] %s" error.Error error.Detail
                | BasicError error -> sprintf "[%s]" error.Error