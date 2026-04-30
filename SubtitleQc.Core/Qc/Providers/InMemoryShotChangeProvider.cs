using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Providers;

/// <summary>
/// Trivial provider seeded from an in-memory <see cref="ShotChangeData"/>
/// snapshot. Useful for tests and for piping pre-deserialized JSON payloads
/// into the rule engine without touching disk.
/// </summary>
public sealed class InMemoryShotChangeProvider : IShotChangeProvider
{
    private readonly ShotChangeData _data;

    public InMemoryShotChangeProvider(ShotChangeData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        _data = data;
    }

    public IReadOnlyList<TimeSpan> GetShotChangeTimestamps() => _data.CutTimestamps;
    public IReadOnlyList<int> GetShotChangeFrames() => _data.CutFrames;
}
