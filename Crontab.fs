module FsharpBDDComparison.Crontab

open System
open NCrontab

/// Parses 5 (standard) or 6 (with seconds) component crontab expressions
let parse (crontabExpression:string) : CrontabSchedule =
    let crontabComponentCount = crontabExpression.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length
    CrontabSchedule.Parse (crontabExpression, CrontabSchedule.ParseOptions ( IncludingSeconds = (crontabComponentCount = 6) ))

/// For start and stop crontabs, e.g. start every day in the morning and stop every day in the evening
let isRunning
    startScheduleCrontab
    stopScheduleCrontab
    maxStartedDurationHours
    maxStoppedDurationHours
    (pointInTime:DateTime)
    =
    let startSchedule = parse startScheduleCrontab
    let stopSchedule = parse stopScheduleCrontab

    let lastStart = startSchedule.GetNextOccurrences(pointInTime.AddHours(-1.0 * maxStartedDurationHours), pointInTime) |> Seq.tryLast
    let lastStop = stopSchedule.GetNextOccurrences(pointInTime.AddHours(-1.0 * maxStoppedDurationHours), pointInTime) |> Seq.tryLast
    //let nextStart = startSchedule.GetNextOccurrences(pointInTime, pointInTime.AddHours(maxStoppedDurationHours)) |> Seq.tryHead
    //let nextStop = stopSchedule.GetNextOccurrences(pointInTime, pointInTime.AddHours(maxStartedDurationHours)) |> Seq.tryHead

    match lastStart, lastStop with
    | Some lastStart, Some lastStop ->
        lastStart > lastStop 
    | None, _ ->
        failwithf "LastStart could not be found using startScheduleCrontab %s and maxStartedDurationHours %f" startScheduleCrontab maxStartedDurationHours
    | _, None ->
        failwithf "LastStop could not be found using stopScheduleCrontab %s and maxStoppedDurationHours %f" stopScheduleCrontab maxStoppedDurationHours

/// For single execution crontabs, e.g. once daily
let getLastRunOn
    runScheduleCrontab
    maxRunIntervalHours
    (pointInTime:DateTime)
    : DateTime    
    =
    let runSchedule = parse runScheduleCrontab
    let lastRunOn = runSchedule.GetNextOccurrences(pointInTime.AddHours(-1.0 * maxRunIntervalHours), pointInTime) |> Seq.tryHead
    match lastRunOn with
    | Some lastRunOn ->
        lastRunOn 
    | None ->
        failwithf "LastRun could not be found using runScheduleCrontab %s and maxRunIntervalHours %f" runScheduleCrontab maxRunIntervalHours
