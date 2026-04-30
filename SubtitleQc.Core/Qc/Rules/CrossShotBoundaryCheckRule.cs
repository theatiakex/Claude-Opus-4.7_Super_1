using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// A cue fails when at least one cut occurs strictly inside its interval.
/// Cuts that coincide with a boundary are treated as aligned, not crossed.
/// </summary>
public sealed class CrossShotBoundaryCheckRule : IQcRule
{
    private readonly IShotChangeProvider _shotChanges;

    public CrossShotBoundaryCheckRule(IShotChangeProvider shotChanges)
    {
        ArgumentNullException.ThrowIfNull(shotChanges);
        _shotChanges = shotChanges;
    }

    public string Name => "CrossShotBoundaryCheck";

    public IReadOnlyList<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        IReadOnlyList<TimeSpan> cuts = _shotChanges.GetShotChangeTimestamps();
        var results = new List<QcResult>(cues.Count);
        foreach (Cue cue in cues)
        {
            results.Add(EvaluateCue(cue, cuts));
        }
        return results;
    }

    private QcResult EvaluateCue(Cue cue, IReadOnlyList<TimeSpan> cuts)
    {
        TimeSpan? offender = FindInteriorCut(cue, cuts);
        bool failed = offender.HasValue;
        string? message = failed ? $"Cut at {offender:c} falls inside the cue interval." : null;
        return new QcResult(cue.Id, Name, failed ? QcStatus.Failed : QcStatus.Passed, message: message);
    }

    private static TimeSpan? FindInteriorCut(Cue cue, IReadOnlyList<TimeSpan> cuts)
    {
        foreach (TimeSpan cut in cuts)
        {
            if (cut > cue.Start && cut < cue.End)
            {
                return cut;
            }
        }
        return null;
    }
}
