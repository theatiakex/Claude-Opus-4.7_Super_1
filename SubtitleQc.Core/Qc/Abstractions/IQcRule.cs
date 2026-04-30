using System.Collections.Generic;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Qc.Abstractions;

/// <summary>
/// Open/Closed extension point for QC checks. Each rule receives the full
/// cue collection so cross-cue checks (e.g. overlap) work without leaking
/// state into the engine. Rules must be pure with respect to their input.
/// </summary>
public interface IQcRule
{
    string Name { get; }
    IReadOnlyList<QcResult> Evaluate(IReadOnlyList<Cue> cues);
}
