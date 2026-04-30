using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MaxCpsRule : IQcRule
{
    private readonly double _threshold;

    public MaxCpsRule(int threshold)
    {
        _threshold = threshold;
    }

    public string Name => "MaxCps";

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
        double seconds = cue.Duration.TotalSeconds;
        if (seconds <= 0)
        {
            return new QcResult(cue.Id, Name, QcStatus.Skipped, message: "Cue has non-positive duration.");
        }
        int charCount = CountCharacters(cue);
        double cps = charCount / seconds;
        bool failed = cps > _threshold;
        string? message = failed ? $"Reading speed is {cps:F2} cps (threshold: {_threshold})." : null;
        return new QcResult(cue.Id, Name, failed ? QcStatus.Failed : QcStatus.Passed, message: message);
    }

    private static int CountCharacters(Cue cue)
    {
        int total = 0;
        foreach (string line in cue.Lines)
        {
            total += line?.Length ?? 0;
        }
        return total;
    }
}
