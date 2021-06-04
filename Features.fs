namespace FsharpBDDComparison.CrontabTests.TickSpec

open System.Diagnostics
open Xunit
open FsharpBDDComparison.Framework.TickSpec

type Features() =
    static let source = AssemblyStepDefinitionsSource(System.Reflection.Assembly.GetExecutingAssembly())
    static let scenarios resourceName = source.ScenariosFromEmbeddedResource resourceName |> MemberData.ofScenarios

    [<Theory; MemberData("scenarios", "FsharpBDDComparison.Features.CrontabTests.feature")>]
    member this.CrontabTests (scenario : XunitSerializableScenario) = 
        Debug.WriteLine(scenario.Name)
        source.ScenarioAction(scenario).Invoke()
   