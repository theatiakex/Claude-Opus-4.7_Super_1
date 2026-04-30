using System;
using System.Collections.Generic;

namespace SubtitleQc.Core.Models;

/// <summary>
/// Format-agnostic representation of a single subtitle cue.
/// Optional frame indices allow rules that operate in the frame domain
/// (e.g. shot-change proximity) to work without re-deriving frames from time.
/// </summary>
public sealed class Cue
{
    public Cue(
        string id,
        TimeSpan start,
        TimeSpan end,
        IReadOnlyList<string> lines,
        int? startFrame = null,
        int? endFrame = null)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(lines);
        if (end < start)
        {
            throw new ArgumentException("Cue end must not precede start.", nameof(end));
        }

        Id = id;
        Start = start;
        End = end;
        Lines = lines;
        StartFrame = startFrame;
        EndFrame = endFrame;
    }

    public string Id { get; }
    public TimeSpan Start { get; }
    public TimeSpan End { get; }
    public IReadOnlyList<string> Lines { get; }
    public int? StartFrame { get; }
    public int? EndFrame { get; }

    public TimeSpan Duration => End - Start;
}
