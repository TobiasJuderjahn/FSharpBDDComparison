module FsharpBDDComparison.CrontabTests.XunitGherkinQuick

open System
open Gherkin.Ast
open NCrontab
open Xunit
open Xunit.Gherkin.Quick
open FsharpBDDComparison
open FsharpBDDComparison.Framework

module Table =
    let getValueByKey key (dt:DataTable) : string =
        dt.Rows
        |> Seq.find (fun x ->
            let cells = x.Cells |> List.ofSeq
            cells.[0].Value = key)
        |> (fun r -> r.Cells)
        |> Seq.last
        |> (fun c -> c.Value)

[<Xunit.Gherkin.Quick.FeatureFile("./CrontabTests.feature")>]
type CrontabTests() =
    inherit Feature()

    let mutable ctxCrontabExpression = None
    let mutable ctxCrontabSchedule : CrontabSchedule option = None

    let mutable isRunningActual: bool option = None
   
    let mutable lastRunOnActual: DateTime option = None
    
    [<When("crontab expression (.+) is parsed")>]
    member this.``When crontab expression <CrontabExpression> is parsed`` (crontabExpression:string) =
        ctxCrontabExpression <- crontabExpression |> Some
        ctxCrontabSchedule <- Crontab.parse crontabExpression |> Some

    [<Then("the CrontabSchedule has the same string representation")>]
    member this.``Then the CrontabSchedule has the same string representation`` () =
        Assert.True(ctxCrontabSchedule.Value.ToString() = ctxCrontabExpression.Value, "Parsed crontabSchedule.ToString() not equal to original crontab expression")
        
    [<When("isRunning is called at (.*) with the following parameters")>]
    member this.``When isRunning is called at <PointInTime> with the following parameters`` (pointInTime:string) (datatable:DataTable) =
        isRunningActual <- 
            Crontab.isRunning
                (Table.getValueByKey "startScheduleCrontab" datatable |> string)
                (Table.getValueByKey "stopScheduleCrontab" datatable |> string)
                (Table.getValueByKey "maxStartedDurationHours" datatable |> Convert.ToDouble)
                (Table.getValueByKey "maxStoppedDurationHours" datatable |> Convert.ToDouble)
                (pointInTime |> DateTime.ParseUtcIso)
            |> Some
    
    [<Then("the isRunning result is (.*)")>]
    member this.``Then the isRunning result is <Result>`` (isRunningExpected:bool) =
        Assert.Equal(isRunningExpected, isRunningActual.Value)

    [<When("getLastRunOn is called at (.*) with the following parameters")>]
    member this.``When getLastRunOn is called at <PointInTime> with the following parameters`` (pointInTime:string) (datatable:DataTable) =
        lastRunOnActual <-
            Crontab.getLastRunOn
                (Table.getValueByKey "runScheduleCrontab" datatable |> string)
                (Table.getValueByKey "maxRunIntervalHours" datatable |> Convert.ToDouble)
                (pointInTime |> DateTime.ParseUtcIso)
            |> Some
    
    [<Then("the getLastRunOn result is (.*)")>]
    member this.``Then the getLastRunOn result is <Result>`` (lastRunOnExpected:string) =
        Assert.Equal(lastRunOnExpected |> DateTime.ParseUtcIso, lastRunOnActual.Value)
