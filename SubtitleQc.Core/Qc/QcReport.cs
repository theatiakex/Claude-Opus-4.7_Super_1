using System.Collections.Generic;
using System.Linq;

namespace SubtitleQc.Core.Qc;

/// <summary>
/// Aggregated, JSON-serializable evaluation output. Intentionally a flat
/// list so consumers can group/filter by cue or rule without prior knowledge
/// of the engine's internal ordering.
/// </summary>
public sealed class QcReport
{
    public QcReport(IReadOnlyList<QcResult> results)
    {
        Results = results;
    }

    public IReadOnlyList<QcResult> Results { get; }

    public bool AllPassed => Results.All(r => r.Status != QcStatus.Failed);
}
