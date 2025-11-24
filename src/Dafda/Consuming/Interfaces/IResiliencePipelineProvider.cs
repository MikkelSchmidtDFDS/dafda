using Polly;

namespace Dafda.Consuming.Interfaces;

/// <summary>
/// 
/// </summary>
public interface IResiliencePipelineProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="registration"></param>
    /// <returns></returns>
    ResiliencePipeline GetPipelineFor(MessageRegistration registration);
}
