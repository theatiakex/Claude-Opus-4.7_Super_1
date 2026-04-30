using System;
using System.Globalization;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// Centralized HH:MM:SS[.,]fff parser shared by SRT and WebVTT.
/// Extracted to avoid duplicating regex/format logic in each parser.
/// </summary>
internal static class TimecodeParser
{
    public static TimeSpan Parse(string token)
    {
        string normalized = token.Trim().Replace(',', '.');
        string[] formats =
        {
            @"hh\:mm\:ss\.fff",
            @"hh\:mm\:ss\.ff",
            @"hh\:mm\:ss\.f",
            @"hh\:mm\:ss",
            @"mm\:ss\.fff",
            @"mm\:ss"
        };
        if (TimeSpan.TryParseExact(normalized, formats, CultureInfo.InvariantCulture, out TimeSpan value))
        {
            return value;
        }
        throw new FormatException($"Unrecognized timecode token: '{token}'.");
    }
}
