using System;
using System.Collections.Generic;

namespace SubtitleQc.Core.Models;

/// <summary>
/// Snapshot of external shot-change information.
/// Time-based and frame-based representations are kept independently because
/// upstream tools may report only one of the two.
/// </summary>
public sealed class ShotChangeData
{
    public ShotChangeData(IReadOnlyList<TimeSpan> cutTimestamps, IReadOnlyList<int> cutFrames)
    {
        CutTimestamps = cutTimestamps;
        CutFrames = cutFrames;
    }

    public IReadOnlyList<TimeSpan> CutTimestamps { get; }
    public IReadOnlyList<int> CutFrames { get; }
}
