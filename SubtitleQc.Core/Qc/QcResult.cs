namespace SubtitleQc.Core.Qc;

/// <summary>
/// JSON-serializable verdict produced by a single rule for a single cue.
/// Carries both the rule identity and the human-readable message so the
/// consumer can render structured reports without consulting the rule again.
/// </summary>
public sealed class QcResult
{
    public QcResult(
        string cueId,
        string ruleName,
        QcStatus status,
        QcSeverity severity = QcSeverity.Error,
        string? message = null)
    {
        CueId = cueId;
        RuleName = ruleName;
        Status = status;
        Severity = severity;
        Message = message;
    }

    public string CueId { get; }
    public string RuleName { get; }
    public QcStatus Status { get; }
    public QcSeverity Severity { get; }
    public string? Message { get; }
}
