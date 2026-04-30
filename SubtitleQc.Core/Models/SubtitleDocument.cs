using System.Collections.Generic;

namespace SubtitleQc.Core.Models;

/// <summary>
/// Aggregate root for a parsed subtitle file. Carries the parsed cues plus
/// optional source format metadata so downstream consumers can stay agnostic
/// to the original on-disk representation.
/// </summary>
public sealed class SubtitleDocument
{
    public SubtitleDocument(string sourceFormat, IReadOnlyList<Cue> cues)
    {
        SourceFormat = sourceFormat;
        Cues = cues;
    }

    public string SourceFormat { get; }
    public IReadOnlyList<Cue> Cues { get; }
}
