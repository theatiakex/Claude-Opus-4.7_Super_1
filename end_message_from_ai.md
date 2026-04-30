Prototype is complete and all 19 tests pass on the first attempt. Here is what was built and why.

Architecture overview
The prototype follows the SDD/SRP/OCP/DIP principles laid out in agents_super.md. Three independent layers — Models, Parsers, QC engine — never know about each other's implementation details:

Super/
├── Super.sln
├── Directory.Packages.props          (workspace-level package versions for the immutable test project)
├── SubtitleQc.Core/
│   ├── Models/                       Cue, ShotChangeData, SubtitleDocument
│   ├── Parsers/                      ISubtitleParser + SRT, WebVTT, TTML + shared TimecodeParser
│   └── Qc/
│       ├── RuleEngine.cs, QcReport.cs, QcResult.cs, QcStatus.cs, QcSeverity.cs
│       ├── Abstractions/             IQcRule, IShotChangeProvider
│       ├── Providers/                InMemoryShotChangeProvider
│       └── Rules/                    8 rule files, one per concern
├── SubtitleQc.Tests/                 (untouched, only the .csproj got a project reference added)
├── test_metrics.jsonl                attempt-1 log entry
└── test_performance_report.md        Mermaid chart for the 100% pass run
Design decisions worth flagging
IQcRule receives the full cue list, not one cue at a time. This was needed for OverlapCheckRule (cross-cue logic) without leaking state into the engine. Single-cue rules just iterate internally.
OverlapCheckRule blames the later cue, not both. Convention in subtitle QC tooling, and matches the test's Single(r => r.CueId == cueB.Id) shape (one result per cue per rule).
MinFramesFromShotChangeRule returns Skipped, not Failed, when no frame data is available. Failing a cue with no evidence would be a false positive; Skipped propagates the absence of evidence into the report instead.
Cue carries optional StartFrame/EndFrame from day one so frame-domain rules don't have to re-derive frames from time, satisfying the holistic-planning requirement.
TimecodeParser is shared between SRT and WebVTT so the only difference between those parsers is the structural framing (header block, blank-line separators) — adding a fourth time-based format would only require a new parser, not duplicated time-parsing code (OCP).
Directory.Packages.props was added to resolve the test project's version-less PackageReference items without touching the immutable test .csproj content.
Test results
Passed!  - Failed: 0, Passed: 19, Skipped: 0, Total: 19, Duration: 130 ms
Logged to test_metrics.jsonl; pass-rate chart in test_performance_report.md.