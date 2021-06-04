# FSharpBDDComparison

Demonstrates BDD-style tests using Xunit, Xunit.Gherkin.Quick, GherkinProvider and TickSpec

Notes:

1. CrontabTests.feature is added 2 times to fsproj because GherkinProvider requires it as Content, and TickSpec requires it as EmbeddedResource
1. GherkinProvider requires AssemblyInfo.fs with assembly attributes disabling test parallelization (due to validation if all scenarios/steps have been visited) ...