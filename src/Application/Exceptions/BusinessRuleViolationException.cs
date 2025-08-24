namespace Application.Exceptions;

public sealed class BusinessRuleViolationException : Exception
{
    public string RuleName { get; }
    public object? RuleData { get; }

    public BusinessRuleViolationException(string ruleName, string message) 
        : base(message)
    {
        RuleName = ruleName;
    }

    public BusinessRuleViolationException(string ruleName, string message, object? ruleData) 
        : base(message)
    {
        RuleName = ruleName;
        RuleData = ruleData;
    }

    public BusinessRuleViolationException(string ruleName, string message, Exception innerException) 
        : base(message, innerException)
    {
        RuleName = ruleName;
    }
}
