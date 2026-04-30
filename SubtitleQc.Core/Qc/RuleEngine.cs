using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc;

/// <summary>
/// Coordinator that applies every registered rule to the cue collection.
/// The engine is intentionally unaware of any specific rule's semantics,
/// satisfying the Open/Closed Principle: adding a new rule requires no
/// engine modification.
/// </summary>
public sealed class RuleEngine
{
    private readonly IReadOnlyList<IQcRule> _rules;

    public RuleEngine(IEnumerable<IQcRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
        _rules = rules.ToArray();
    }

    public QcReport Evaluate(IEnumerable<Cue> cues)
    {
        ArgumentNullException.ThrowIfNull(cues);
        IReadOnlyList<Cue> snapshot = cues.ToArray();
        var aggregated = new List<QcResult>(_rules.Count * snapshot.Count);
        foreach (IQcRule rule in _rules)
        {
            aggregated.AddRange(rule.Evaluate(snapshot));
        }
        return new QcReport(aggregated);
    }
}
