using Dafda.Consuming.Interfaces;
using Polly;
using Polly.Registry;
using System.Collections.Generic;

namespace Dafda.Consuming;
internal class MessageRegistrationResiliencePipelineProvider(ResiliencePipelineProvider<string> resiliencePipelineProvider, ResiliencePipelineBuilder builder) : IResiliencePipelineProvider
{
    private readonly Dictionary<string, ResiliencePipeline> _defaultResiliencePipelines = [];
    public ResiliencePipeline GetPipelineFor(MessageRegistration registration)
    {
        if (!resiliencePipelineProvider.TryGetPipeline(registration.ResiliencePipelineGroup, out var resiliencePipeline))
        {
            if (!_defaultResiliencePipelines.TryGetValue(registration.ResiliencePipelineGroup, out resiliencePipeline))
            {
                resiliencePipeline = builder.Build();
                _defaultResiliencePipelines[registration.ResiliencePipelineGroup] = resiliencePipeline;
            }
        }

        return resiliencePipeline;
    }
}
