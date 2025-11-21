using Dafda.Consuming;
using Dafda.Tests.TestDoubles;
using Polly;
using Polly.Registry;

namespace Dafda.Tests.Builders
{
    internal class LocalMessageDispatcherBuilder
    {
        private MessageHandlerRegistry _messageHandlerRegistry;
        private IHandlerUnitOfWorkFactory _handlerUnitOfWorkFactory;
        private ResiliencePipelineProviderStub _resiliencePipelineProvider;

        public LocalMessageDispatcherBuilder()
        {
            _messageHandlerRegistry = new MessageHandlerRegistry();
            _resiliencePipelineProvider = new ResiliencePipelineProviderStub();
        }

        public LocalMessageDispatcherBuilder WithMessageHandlerRegistry(MessageHandlerRegistry messageHandlerRegistry)
        {
            _messageHandlerRegistry = messageHandlerRegistry;
            return this;
        }

        public LocalMessageDispatcherBuilder WithHandlerUnitOfWorkFactory(IHandlerUnitOfWorkFactory handlerUnitOfWorkFactory)
        {
            _handlerUnitOfWorkFactory = handlerUnitOfWorkFactory;
            return this;
        }

        public LocalMessageDispatcherBuilder WithHandlerUnitOfWork(IHandlerUnitOfWork unitOfWork)
        {
            return WithHandlerUnitOfWorkFactory(new HandlerUnitOfWorkFactoryStub(unitOfWork));
        }

        public LocalMessageDispatcherBuilder WithResiliencePipeline(MessageRegistration registration, ResiliencePipeline resiliencePipeline)
        {
            _resiliencePipelineProvider.AddPipeline(registration, resiliencePipeline);
            return this;
        }

        public LocalMessageDispatcher Build()
        {
            return new LocalMessageDispatcher(
                _messageHandlerRegistry,
                _handlerUnitOfWorkFactory,
                new RequireExplicitHandlers(),
                _resiliencePipelineProvider);
        }
    }
}
