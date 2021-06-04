module FsharpBDDComparison.CrontabTests.CrontabTests.TickSpec

open System
open NCrontab
open Xunit
open TickSpec
open FsharpBDDComparison
open FsharpBDDComparison.Framework
open FsharpBDDComparison.Framework.Table

type ParseContext = {
    CrontabExpression : string
    ParsedCrontabSchedule : CrontabSchedule
} 

let [<When>] ``crontab expression (.*) is parsed`` (crontabExpression:string) =
    let crontabSchedule = Crontab.parse crontabExpression
    
    {
        CrontabExpression = crontabExpression
        ParsedCrontabSchedule = crontabSchedule
    }
    
let [<Then>] ``the CrontabSchedule has the same string representation`` (ctx:ParseContext) =
    Assert.True(ctx.ParsedCrontabSchedule.ToString() = ctx.CrontabExpression, "Parsed crontabSchedule.ToString() not equal to original crontab expression")

let [<When>] ``isRunning is called at (.*) with the following parameters`` pointInTime (parameters:Table) =
    Crontab.isRunning
        (VTable.getValueByKey "startScheduleCrontab" parameters.Rows)
        (VTable.getValueByKey "stopScheduleCrontab" parameters.Rows)
        (VTable.getValueByKey "maxStartedDurationHours" parameters.Rows |> float)
        (VTable.getValueByKey "maxStoppedDurationHours" parameters.Rows |> float)
        (pointInTime |> DateTime.ParseUtcIso)
    
let [<Then>] ``the isRunning result is (.*)`` (isRunningExpected:string) (isRunningActual:bool) =
    Assert.Equal(isRunningExpected |> bool.Parse, isRunningActual)

let [<When>] ``getLastRunOn is called at (.*) with the following parameters`` pointInTime (parameters:Table) =
    Crontab.getLastRunOn
        (VTable.getValueByKey "runScheduleCrontab" parameters.Rows)
        (VTable.getValueByKey "maxRunIntervalHours" parameters.Rows |> float)
        (pointInTime |> DateTime.ParseUtcIso)
    
let [<Then>] ``the getLastRunOn result is (.*)`` (lastRunOnExpected:string) (lastRunOnActual:DateTime) =
    Assert.Equal(lastRunOnExpected |> DateTime.ParseUtcIso, lastRunOnActual)
    

