using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// WebVTT parser. The WEBVTT signature line and any header metadata are
/// skipped, then cues are parsed similarly to SRT but with '.' as the
/// millisecond separator. Inline cue settings (after the timing arrow)
/// are intentionally discarded for the prototype.
/// </summary>
public sealed class WebVttParser : ISubtitleParser
{
    private static readonly Regex TimingLine = new(
        @"^\s*(?<start>\S+)\s*-->\s*(?<end>\S+)",
        RegexOptions.Compiled);

    public string FormatName => "WebVTT";

    public SubtitleDocument Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        string[] blocks = SplitBlocks(source);
        var cues = new List<Cue>(blocks.Length);
        int ordinal = 1;
        foreach (string block in blocks)
        {
            Cue? parsed = ParseBlock(block, ordinal);
            if (parsed is not null)
            {
                cues.Add(parsed);
                ordinal++;
            }
        }
        return new SubtitleDocument(FormatName, cues);
    }

    private static string[] SplitBlocks(string source)
    {
        string normalized = source.Replace("\r\n", "\n").Replace('\r', '\n');
        return normalized.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static Cue? ParseBlock(string block, int ordinal)
    {
        string[] lines = block.Split('\n');
        if (IsHeaderBlock(lines))
        {
            return null;
        }
        int timingIndex = LocateTimingLine(lines);
        if (timingIndex < 0)
        {
            return null;
        }
        return BuildCue(lines, timingIndex, ordinal);
    }

    private static Cue BuildCue(string[] lines, int timingIndex, int ordinal)
    {
        Match m = TimingLine.Match(lines[timingIndex]);
        TimeSpan start = TimecodeParser.Parse(m.Groups["start"].Value);
        TimeSpan end = TimecodeParser.Parse(m.Groups["end"].Value);
        var text = new List<string>();
        for (int i = timingIndex + 1; i < lines.Length; i++)
        {
            text.Add(lines[i]);
        }
        return new Cue($"vtt-{ordinal}", start, end, text);
    }

    private static bool IsHeaderBlock(string[] lines)
    {
        return lines.Length > 0 && lines[0].StartsWith("WEBVTT", StringComparison.Ordinal);
    }

    private static int LocateTimingLine(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (TimingLine.IsMatch(lines[i]))
            {
                return i;
            }
        }
        return -1;
    }
}
