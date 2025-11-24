using Polly;
using System;

namespace Dafda.Polly;

/// <summary>
/// The options for the <see cref="ContinueOnErrorStrategy"/>.
/// </summary>
public sealed class ContinueOnErrorStrategyOptions : ResilienceStrategyOptions
{
    /// <summary>
    /// Gets or sets the delegate that is executed when an exception is swallowed.
    /// </summary>
    /// <value>
    /// The default value is a delegate that does nothing.
    /// </value>
    public Action<Exception> OnError = ex => { };

    /// <summary>
    /// Gets or sets a predicate that determines whether the OnError should be executed for a given exception.
    /// </summary>
    /// <value>
    /// The default value is a predicate that falls back on any exception except <see cref="OperationCanceledException"/>.
    /// </value>
    public Predicate<Exception> ShouldHandle = ex => ex is not OperationCanceledException;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContinueOnErrorStrategyOptions"/> class.
    /// </summary>
    public ContinueOnErrorStrategyOptions()
    {
        Name = "ContinueOnError";
    }
}
