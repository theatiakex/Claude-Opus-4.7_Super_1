using System.Collections.Generic;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MaxCplRule : IQcRule
{
    private readonly int _threshold;

    public MaxCplRule(int threshold)
    {
        _threshold = threshold;
    }

    public string Name => "MaxCpl";

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
        int longest = cue.Lines.Count == 0 ? 0 : cue.Lines.Max(l => l?.Length ?? 0);
        bool failed = longest > _threshold;
        string? message = failed
            ? $"Longest line is {longest} characters (threshold: {_threshold})."
            : null;
        return new QcResult(cue.Id, Name, failed ? QcStatus.Failed : QcStatus.Passed, message: message);
    }
}
