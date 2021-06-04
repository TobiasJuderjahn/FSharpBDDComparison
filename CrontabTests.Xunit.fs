module FsharpBDDComparison.CrontabTests.Xunit

open System
open Xunit
open FsharpBDDComparison
open FsharpBDDComparison.Framework

let parseExamples =
    [|
        [| "0 0 2 * *" |]
        [| "0 0 2 * * *"  |]
    |]
    
[<Theory; MemberData("parseExamples")>]
let ``Crontab Parsing`` (crontabExpression:string) =
    //When crontab expression <CrontabExpression> is parsed
    let crontabSchedule = Crontab.parse crontabExpression
    
    //Then the CrontabSchedule has the same string representation
    Assert.True(crontabSchedule.ToString() = crontabExpression, "Parsed crontabSchedule.ToString() not equal to original crontab expression")

let isRunningExamples =
    [|
        [| DateTime.ParseUtcIso("2020-10-21T00:00:00Z") :> obj; false :> obj |]
        [| DateTime.ParseUtcIso("2020-10-21T08:00:00Z"); true |]
    |]

[<Theory; MemberData("isRunningExamples")>]
let ``Running/Not Running Interval is identified successfully`` (pointInTime:DateTime) (isRunningExpected:bool) =
    //When isRunning is called at <PointInTime> with the following parameters
    let isRunningActual = 
        Crontab.isRunning
            "0 7 * * 1-5"
            "0 16 * * 1-5"
            72.0
            72.0
            pointInTime
    
    //Then the isRunning result is <Result>
    Assert.Equal(isRunningExpected, isRunningActual)

let getLastRunOnExamples =
    [|
        [| DateTime.ParseUtcIso("2020-10-21T00:00:00Z") :> obj; DateTime.ParseUtcIso("2020-10-20T02:00:00Z") :> obj |]
        [| DateTime.ParseUtcIso("2020-10-21T08:00:00Z"); DateTime.ParseUtcIso("2020-10-21T02:00:00Z") |]
    |]
    
[<Theory; MemberData("getLastRunOnExamples")>]
let ``Last Run DateTime is retrieved successfully`` (pointInTime:DateTime) (lastRunOnExpected:DateTime) =
    //When getLastRunOn is called at <PointInTime> with the following parameters
    let lastRunOnActual =
        Crontab.getLastRunOn
            "0 0 2 * * *"
            24.0
            pointInTime
    
    //Then the getLastRunOn result is <Result>
    Assert.Equal(lastRunOnExpected, lastRunOnActual)
