using System;
using System.Collections.Generic;

namespace SubtitleQc.Core.Qc.Abstractions;

/// <summary>
/// DIP boundary for external shot-change data. Implementations may load data
/// from disk, a service, or memory; rules depend only on this abstraction.
/// </summary>
public interface IShotChangeProvider
{
    IReadOnlyList<TimeSpan> GetShotChangeTimestamps();
    IReadOnlyList<int> GetShotChangeFrames();
}
