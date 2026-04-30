using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// TTML parser using System.Xml.Linq. We deliberately accept any XML
/// namespace so the parser remains tolerant of TTML 1, TTML 2, and IMSC
/// profiles that all use 'p' elements with begin/end attributes.
/// Line breaks in TTML are encoded as 'br' elements; we split on those to
/// reconstruct the multi-line internal model.
/// </summary>
public sealed class TtmlParser : ISubtitleParser
{
    public string FormatName => "TTML";

    public SubtitleDocument Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        XDocument doc = XDocument.Parse(source);
        IEnumerable<XElement> paragraphs = SelectParagraphs(doc);
        var cues = new List<Cue>();
        int ordinal = 1;
        foreach (XElement p in paragraphs)
        {
            Cue? parsed = ParseParagraph(p, ordinal);
            if (parsed is not null)
            {
                cues.Add(parsed);
                ordinal++;
            }
        }
        return new SubtitleDocument(FormatName, cues);
    }

    private static IEnumerable<XElement> SelectParagraphs(XDocument doc)
    {
        return doc.Descendants().Where(e => e.Name.LocalName == "p");
    }

    private static Cue? ParseParagraph(XElement p, int ordinal)
    {
        string? begin = p.Attribute("begin")?.Value;
        string? end = p.Attribute("end")?.Value;
        if (begin is null || end is null)
        {
            return null;
        }
        TimeSpan start = TimecodeParser.Parse(begin);
        TimeSpan stop = TimecodeParser.Parse(end);
        IReadOnlyList<string> lines = ExtractLines(p);
        return new Cue($"ttml-{ordinal}", start, stop, lines);
    }

    private static IReadOnlyList<string> ExtractLines(XElement p)
    {
        var lines = new List<string>();
        var current = new System.Text.StringBuilder();
        foreach (XNode node in p.Nodes())
        {
            AppendNode(node, current, lines);
        }
        lines.Add(current.ToString());
        return lines;
    }

    private static void AppendNode(XNode node, System.Text.StringBuilder current, List<string> lines)
    {
        if (node is XElement element && element.Name.LocalName == "br")
        {
            lines.Add(current.ToString());
            current.Clear();
            return;
        }
        if (node is XText text)
        {
            current.Append(text.Value);
        }
        else if (node is XElement el)
        {
            current.Append(el.Value);
        }
    }
}
