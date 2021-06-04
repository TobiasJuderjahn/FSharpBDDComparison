module FsharpBDDComparison.CrontabTests.GherkinProvider

open System
open Xunit
open FSharp.Data.Gherkin
open GherkinProvider.Validation
open Xunit.Extensions.Ordering
open FsharpBDDComparison
open FsharpBDDComparison.Framework

type TestFeature = GherkinProvider<const(__SOURCE_DIRECTORY__ + "/Features/CrontabTests.feature")>
let feature = TestFeature.CreateFeature()

let assertScenarioInSync (scenario:TestFeature.TestFeature_ScenarioBase) =
    Assert.True(scenario.Visited)
    scenario.Steps |> Array.iter (fun x -> Assert.True(x.Visited))

let parseExamples =
    feature.Scenarios.``Crontab Parsing``.Examples
    |> Array.map (fun x -> [|x.CrontabExpression.Value|])

[<Theory; MemberData("parseExamples")>]
let ``Parsing of Crontabs works`` (crontabExpression:string) =
    let scenario = feature.Scenarios.``Crontab Parsing``

    scenario.``0 When crontab expression <CrontabExpression> is parsed`` |> ignore
    let crontabSchedule = FsharpBDDComparison.Crontab.parse crontabExpression
    
    scenario.``1 Then the CrontabSchedule has the same string representation`` |> ignore
    Assert.True(crontabSchedule.ToString() = crontabExpression, "Parsed crontabSchedule.ToString() not equal to original crontab expression")
    
    assertScenarioInSync scenario

let isRunningExamples =
    feature.Scenarios.``Running_Not Running Interval is identified successfully``.Examples
    |> Array.map (fun x -> [|x.PointInTime.Value; x.Result.Value|])

[<Theory; MemberData("isRunningExamples")>]
let ``Running/Not Running Interval is identified successfully`` (pointInTime:string) (isRunningExpected:string) =
    let scenario = feature.Scenarios.``Running_Not Running Interval is identified successfully``
    
    scenario.``0 When isRunning is called at <PointInTime> with the following parameters`` |> ignore
    let getValueByKey key =
        scenario.``0 When isRunning is called at <PointInTime> with the following parameters``.Argument
        |> Array.filter (fun i -> i.Parameter.Value = key)
        |> Array.head
        |> (fun x -> x.Value.Value)
        
    let isRunningActual = 
        Crontab.isRunning
            (getValueByKey "startScheduleCrontab" |> string)
            (getValueByKey "stopScheduleCrontab" |> string)
            (getValueByKey "maxStartedDurationHours" |> Convert.ToDouble)
            (getValueByKey "maxStoppedDurationHours" |> Convert.ToDouble)
            (pointInTime |> DateTime.ParseUtcIso)
    
    scenario.``1 Then the isRunning result is <Result>`` |> ignore
    Assert.Equal(isRunningExpected |> bool.Parse, isRunningActual)
    
    assertScenarioInSync scenario

let getLastRunOnExamples =
    feature.Scenarios.``Last Run DateTime is retrieved successfully``.Examples
    |> Array.map (fun x -> [|x.PointInTime.Value; x.Result.Value|])

[<Theory; MemberData("getLastRunOnExamples")>]
let ``Last Run DateTime is retrieved successfully`` (pointInTime:string) (lastRunOnExpected:string) =
    let scenario = feature.Scenarios.``Last Run DateTime is retrieved successfully``

    scenario.``0 When getLastRunOn is called at <PointInTime> with the following parameters`` |> ignore
    let getValueByKey key =
        scenario.``0 When getLastRunOn is called at <PointInTime> with the following parameters``.Argument
        |> Array.filter (fun i -> i.Parameter.Value = key)
        |> Array.head
        |> (fun x -> x.Value.Value)    
    
    let lastRunOnActual =
        Crontab.getLastRunOn
            (getValueByKey "runScheduleCrontab" |> string)
            (getValueByKey "maxRunIntervalHours" |> Convert.ToDouble)
            (pointInTime |> DateTime.ParseUtcIso)
        |> DateTime.ToStringIso
    
    scenario.``1 Then the getLastRunOn result is <Result>`` |> ignore
    Assert.Equal(lastRunOnExpected |> DateTime.ParseUtcIso, lastRunOnActual |> DateTime.ParseUtcIso)

    assertScenarioInSync scenario
    
[<Fact; Order(Int32.MaxValue)>]
let validateFeatureVisited () =
    let validator = FeatureValidator()
    match validator.Validate feature with
    | None -> ()
    | Some report -> failwith(report.Summary)


