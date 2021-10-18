namespace SupportCenterBot.ParseCommandLineOptions

module internal Implementation =


    let rec private parseCommandLineOptions (args: string list) (options: Options)  =

        match args with
            | [] -> options
            | "--publish-intranet"::xs ->
                let optionsWithPush = { options with PushIntranet = Push; Help = Skip }
                parseCommandLineOptions xs optionsWithPush

            | "--help"::xs ->
                let optionsWithHelp = { options with Help = Print}
                parseCommandLineOptions xs optionsWithHelp

            | "--hostname"::xs ->
                match xs with
                    | "--"::xss ->
                        eprintfn "Value for option '--hostname' is not provided, using: %s" (HostName.value options.HostName)
                        parseCommandLineOptions xs options
                    | hostname::xss ->
                        let optionsWithHostName = {options with HostName = (HostName.create hostname); Help = Skip }
                        parseCommandLineOptions xss optionsWithHostName
                    | [] ->
                        eprintfn "Value for option '--hostname' is not provided, using: %s" (HostName.value options.HostName)
                        parseCommandLineOptions xs options
                    
            | x::xs ->
                       eprintfn "Option '%s' is unrecognized" x
                       parseCommandLineOptions xs options


    let parseCommandLine : ParseCommandLineOptions =

        fun hostName argv ->

            let defaultOptions = {
                Help = Print
                CheckDrive = []
                PushIntranet = Ignore
                HostName = hostName
            }

            parseCommandLineOptions (List.ofArray argv) defaultOptions

    let help (hostName: HostName) =
        sprintf """usage: csbot
  --help: If present prints available parameters and exit
  --hostname: $HOSTNAME Specify a hostname to report in Tickets. Default: '%s' (queried to SO).
  --publish-intranet: If present will find for open tickets in group GROUP_ID and push to Intranet as Ticket new if not already published
  --check-drive-space: If present will find for space in drives and push to Zammad as Ticket new if not already pushed

        """ (HostName.value hostName)
        
    module PushToIntranet =
        let toString (flag: PushToIntranet) =
            match flag with
                | Ignore -> "Off"
                | Push -> "On"

    module Options =


        let private listSwitchtoString (options: 'a list) =
            match options with
                | [] -> "Off"
                | _ -> "On"

        let toString (options: Options) =
                
                sprintf "  --hostname: %s" (HostName.value options.HostName)
                |> sprintf "%s\n%s" (sprintf "  --check-drives: %s" (listSwitchtoString options.CheckDrive)) 
                |> sprintf "%s\n%s" (sprintf "  --push-intranet: %s" (PushToIntranet.toString options.PushIntranet))
                |> sprintf "csbot running options:\n%s"