using Dafda.Consuming;
using Dafda.Consuming.Interfaces;
using Dafda.Tests.TestDoubles;
using Moq;
using Polly;

namespace Dafda.Tests.Builders
{
    internal class LocalMessageDispatcherBuilder
    {
        private MessageHandlerRegistry _messageHandlerRegistry;
        private IHandlerUnitOfWorkFactory _handlerUnitOfWorkFactory;
        private readonly Mock<IResiliencePipelineProvider> _mockResiliencePipelineProvider;
        private IResiliencePipelineProvider _resiliencePipelineProvider;

        public LocalMessageDispatcherBuilder()
        {
            _messageHandlerRegistry = new MessageHandlerRegistry();
            _mockResiliencePipelineProvider = new Mock<IResiliencePipelineProvider>();
            _mockResiliencePipelineProvider.Setup(m => m.GetPipelineFor(It.IsAny<MessageRegistration>())).Returns(ResiliencePipeline.Empty);
            _resiliencePipelineProvider = _mockResiliencePipelineProvider.Object;
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
