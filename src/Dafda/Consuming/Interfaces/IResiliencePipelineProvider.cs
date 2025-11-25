using Polly;

namespace Dafda.Consuming.Interfaces;

/// <summary>
/// Provides resilience pipelines for message handling based on message registration configuration.
/// </summary>
public interface IResiliencePipelineProvider
{
    /// <summary>
    /// Gets the resilience pipeline configured for the specified message registration.
    /// </summary>
    /// <param name="registration">The message registration containing the resilience pipeline group configuration.</param>
    /// <returns>A <see cref="ResiliencePipeline"/> configured for the message registration.</returns>
    ResiliencePipeline GetPipelineFor(MessageRegistration registration);
}
