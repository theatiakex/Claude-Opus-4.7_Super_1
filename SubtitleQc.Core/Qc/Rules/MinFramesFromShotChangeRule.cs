using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Validates that a cue starts at least N frames away from the nearest cut.
/// Cues lacking a known start frame are skipped rather than failed: the rule
/// has no evidence to act on and producing a false positive would be worse
/// than emitting a Skipped status that downstream tooling can surface.
/// </summary>
public sealed class MinFramesFromShotChangeRule : IQcRule
{
    private readonly IShotChangeProvider _shotChanges;
    private readonly int _thresholdFrames;

    public MinFramesFromShotChangeRule(IShotChangeProvider shotChanges, int thresholdFrames)
    {
        ArgumentNullException.ThrowIfNull(shotChanges);
        _shotChanges = shotChanges;
        _thresholdFrames = thresholdFrames;
    }

    public string Name => "MinFramesFromShotChange";

    public IReadOnlyList<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        IReadOnlyList<int> cutFrames = _shotChanges.GetShotChangeFrames();
        var results = new List<QcResult>(cues.Count);
        foreach (Cue cue in cues)
        {
            results.Add(EvaluateCue(cue, cutFrames));
        }
        return results;
    }

    private QcResult EvaluateCue(Cue cue, IReadOnlyList<int> cutFrames)
    {
        if (cue.StartFrame is null || cutFrames.Count == 0)
        {
            return new QcResult(cue.Id, Name, QcStatus.Skipped, message: "No frame data available.");
        }
        int distance = NearestCutDistance(cue.StartFrame.Value, cutFrames);
        bool failed = distance < _thresholdFrames;
        string? message = failed ? $"Cue starts {distance} frame(s) from cut (threshold: {_thresholdFrames})." : null;
        return new QcResult(cue.Id, Name, failed ? QcStatus.Failed : QcStatus.Passed, message: message);
    }

    private static int NearestCutDistance(int frame, IReadOnlyList<int> cutFrames)
    {
        int best = int.MaxValue;
        foreach (int cut in cutFrames)
        {
            int delta = Math.Abs(frame - cut);
            if (delta < best)
            {
                best = delta;
            }
        }
        return best;
    }
}
