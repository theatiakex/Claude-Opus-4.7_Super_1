using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Flags a cue when an earlier-starting cue is still on screen at its
/// own start. Only the "later" cue is failed: the earlier cue had no way
/// to know an overlap would later be introduced, so attributing blame to
/// the cue that begins inside another cue's interval is the conventional
/// approach in subtitle QC tooling.
/// </summary>
public sealed class OverlapCheckRule : IQcRule
{
    public string Name => "OverlapCheck";

    public IReadOnlyList<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        var results = new List<QcResult>(cues.Count);
        foreach (Cue cue in cues)
        {
            results.Add(EvaluateCue(cue, cues));
        }
        return results;
    }

    private QcResult EvaluateCue(Cue cue, IReadOnlyList<Cue> all)
    {
        Cue? offender = FindEarlierOverlap(cue, all);
        bool failed = offender is not null;
        string? message = failed ? $"Overlaps with cue {offender!.Id}." : null;
        return new QcResult(cue.Id, Name, failed ? QcStatus.Failed : QcStatus.Passed, message: message);
    }

    private static Cue? FindEarlierOverlap(Cue cue, IReadOnlyList<Cue> all)
    {
        foreach (Cue other in all)
        {
            if (ReferenceEquals(other, cue) || other.Id == cue.Id)
            {
                continue;
            }
            if (other.Start < cue.Start && other.End > cue.Start)
            {
                return other;
            }
        }
        return null;
    }
}
