using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MinDurationRule : IQcRule
{
    private readonly TimeSpan _threshold;

    public MinDurationRule(TimeSpan threshold)
    {
        _threshold = threshold;
    }

    public string Name => "MinDuration";

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
        bool failed = cue.Duration < _threshold;
        string? message = failed
            ? $"Cue duration {cue.Duration.TotalSeconds:F3}s is below threshold {_threshold.TotalSeconds:F3}s."
            : null;
        return new QcResult(cue.Id, Name, failed ? QcStatus.Failed : QcStatus.Passed, message: message);
    }
}
