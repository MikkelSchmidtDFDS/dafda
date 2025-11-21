using Dafda.Consuming;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dafda.Tests.TestDoubles;
internal class ResiliencePipelineProviderStub : ResiliencePipelineProvider<string>
{
    private Dictionary<string, ResiliencePipeline> _pipelines = [];

    public void AddPipeline(MessageRegistration registration, ResiliencePipeline pipeline)
    {
        var key = $"{registration.HandlerInstanceType.FullName}-{registration.Topic}-{registration.MessageType}";
        _pipelines[key] = pipeline;
    }

    public override bool TryGetPipeline(string key, [NotNullWhen(true)] out ResiliencePipeline pipeline)
    {
        return _pipelines.TryGetValue(key, out pipeline);
    }

    public override bool TryGetPipeline<TResult>(string key, [NotNullWhen(true)] out ResiliencePipeline<TResult> pipeline)
    {
        throw new NotImplementedException();
    }
}
