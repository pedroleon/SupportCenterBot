namespace SupportCenterBot.ParseCommandLineOptions

// ============================
// Include model safe types
// ============================

type Help = Print | Skip
type HostName = private HostName of string

type DriveName = DriveName of string
type Percentage = Percentage of int 
type Space = {
    Drive: DriveName
    Limit: Percentage 
}
type CheckDrive = Space

type PushToIntranet =
    | Ignore
    | Push
    
type Options = {
    HostName: HostName
    CheckDrive: CheckDrive list
    PushIntranet: PushToIntranet 
    Help: Help
}

module HostName =
    let create name = (HostName name)
    let value (HostName name) = name

// ===========================
// Include workflow definition
// ===========================

type ParseCommandLineOptions = HostName -> string[] -> Options

