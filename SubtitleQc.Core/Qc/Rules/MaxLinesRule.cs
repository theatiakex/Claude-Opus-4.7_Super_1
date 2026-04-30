using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MaxLinesRule : IQcRule
{
    private readonly int _threshold;

    public MaxLinesRule(int threshold)
    {
        _threshold = threshold;
    }

    public string Name => "MaxLines";

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
        bool failed = cue.Lines.Count > _threshold;
        string? message = failed
            ? $"Cue has {cue.Lines.Count} lines (threshold: {_threshold})."
            : null;
        return new QcResult(cue.Id, Name, failed ? QcStatus.Failed : QcStatus.Passed, message: message);
    }
}
