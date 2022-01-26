module FsharpBDDComparison.Framework

open System
open System.Globalization

[<AutoOpen>]
module TypeExtensions =
    type DateTime with
        /// Returns 2020-04-08T11:55:00 for both "2020-04-08T11:55:00+02" and "2020-04-08T11:55:00Z"
        static member ParseUtcIso(dateTimeString:string) =
            DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)

        static member ToStringIso(dt:DateTime) =
            dt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")


module Table =
    module VTable = 
        let getValueByKey key (rows: string[][]) = 
            rows
            |> Array.tryFind (fun x -> x.[0] = key)
            |> function
            | Some value -> value.[1]
            | None -> failwithf $"Could not find value of key %s{key}!"

        let getValueByKeyOrNone key (rows: string[][]) =
            rows
            |> Array.tryFind (fun x -> x.[0] = key)
            |> Option.map (fun x -> x.[1])

        let getValueByKeyOrDefault key defaultValue (rows: string[][]) =
            rows
            |> getValueByKeyOrNone key
            |> Option.defaultValue defaultValue
            
        let replacePlaceholders (table: string[][]) (stringWithPlaceholders:string) : string =
            table
            |> Array.fold (fun (state:string) (rowValues:string[]) ->
                state.Replace(
                    $"{{%s{rowValues.[0]}}}",
                    (getValueByKey rowValues.[0] table))) stringWithPlaceholders
