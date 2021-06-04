# FSharpBDDComparison

Demonstrates BDD-style tests using Xunit, Xunit.Gherkin.Quick, GherkinProvider and TickSpec

Blog post: https://dev.to/deyanp/bdd-like-testing-in-f-with-xunit-gherkin-gherkinprovider-and-tickspec-11d9

Notes:

1. CrontabTests.feature is added 2 times to fsproj because GherkinProvider requires it as Content, and TickSpec requires it as EmbeddedResource
1. GherkinProvider requires AssemblyInfo.fs with assembly attributes disabling test parallelization (due to validation if all scenarios/steps have been visited) ...
