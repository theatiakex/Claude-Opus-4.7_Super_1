using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// Abstraction for any subtitle file parser. Inputs are raw strings to keep
/// IO concerns out of the parser layer; callers (e.g. file readers) decide
/// how the bytes get to the parser.
/// </summary>
public interface ISubtitleParser
{
    string FormatName { get; }
    SubtitleDocument Parse(string source);
}
