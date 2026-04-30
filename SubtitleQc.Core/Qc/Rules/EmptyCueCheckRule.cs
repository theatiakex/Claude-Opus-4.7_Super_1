using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class EmptyCueCheckRule : IQcRule
{
    public string Name => "EmptyCueCheck";

    public IReadOnlyList<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        var results = new List<QcResult>(cues.Count);
        foreach (Cue cue in cues)
        {
            results.Add(EvaluateCue(cue));
        }
        return results;
    }

    private QcResult EvaluateCue(Cue cue)
    {
        bool failed = IsEffectivelyEmpty(cue);
        string? message = failed ? "Cue contains no visible text." : null;
        return new QcResult(cue.Id, Name, failed ? QcStatus.Failed : QcStatus.Passed, message: message);
    }

    private static bool IsEffectivelyEmpty(Cue cue)
    {
        foreach (string line in cue.Lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                return false;
            }
        }
        return true;
    }
}
