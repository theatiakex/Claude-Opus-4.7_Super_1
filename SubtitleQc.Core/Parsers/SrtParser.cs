using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// SubRip parser. Blocks are separated by blank lines; each block has an
/// index, a "HH:MM:SS,mmm --> HH:MM:SS,mmm" timing line, and 1+ text lines.
/// Index lines are tolerated but not relied on; cue ids come from the
/// parser instead so downstream code is index-agnostic.
/// </summary>
public sealed class SrtParser : ISubtitleParser
{
    private static readonly Regex TimingLine = new(
        @"^\s*(?<start>\S+)\s*-->\s*(?<end>\S+)",
        RegexOptions.Compiled);

    public string FormatName => "SRT";

    public SubtitleDocument Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        IReadOnlyList<string[]> blocks = SplitBlocks(source);
        var cues = new List<Cue>(blocks.Count);
        int ordinal = 1;
        foreach (string[] block in blocks)
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

    private static IReadOnlyList<string[]> SplitBlocks(string source)
    {
        string normalized = source.Replace("\r\n", "\n").Replace('\r', '\n');
        string[] raw = normalized.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        var blocks = new List<string[]>(raw.Length);
        foreach (string chunk in raw)
        {
            blocks.Add(chunk.Split('\n'));
        }
        return blocks;
    }

    private static Cue? ParseBlock(string[] lines, int ordinal)
    {
        int timingIndex = LocateTimingLine(lines);
        if (timingIndex < 0)
        {
            return null;
        }
        Match m = TimingLine.Match(lines[timingIndex]);
        TimeSpan start = TimecodeParser.Parse(m.Groups["start"].Value);
        TimeSpan end = TimecodeParser.Parse(m.Groups["end"].Value);
        var textLines = new List<string>();
        for (int i = timingIndex + 1; i < lines.Length; i++)
        {
            textLines.Add(lines[i]);
        }
        return new Cue($"srt-{ordinal}", start, end, textLines);
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
