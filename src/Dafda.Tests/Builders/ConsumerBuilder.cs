using Dafda.Consuming;
using Dafda.Consuming.Interfaces;
using Dafda.Consuming.MessageFilters;
using Dafda.Tests.TestDoubles;
using Moq;
using Polly;

namespace Dafda.Tests.Builders
{
    internal class ConsumerBuilder
    {
        private IHandlerUnitOfWorkFactory _unitOfWorkFactory;
        private IConsumerScopeFactory _consumerScopeFactory;
        private MessageHandlerRegistry _registry;
        private IUnconfiguredMessageHandlingStrategy _unconfiguredMessageStrategy;
        private IResiliencePipelineProvider _resiliencePipelineProvider;
        private readonly Mock<IResiliencePipelineProvider> _mockResiliencePipelineProvider;

        private bool _enableAutoCommit;
        private MessageFilter _messageFilter = MessageFilter.Default;

        public ConsumerBuilder()
        {
            _unitOfWorkFactory = new HandlerUnitOfWorkFactoryStub(null);
            _consumerScopeFactory = new ConsumerScopeFactoryStub(new ConsumerScopeStub(new MessageResultBuilder().Build()));
            _registry = new MessageHandlerRegistry();
            _unconfiguredMessageStrategy = new RequireExplicitHandlers();
            _mockResiliencePipelineProvider = new Mock<IResiliencePipelineProvider>();
            _mockResiliencePipelineProvider.Setup(m => m.GetPipelineFor(It.IsAny<MessageRegistration>())).Returns(ResiliencePipeline.Empty);
            _resiliencePipelineProvider = _mockResiliencePipelineProvider.Object;
        }

        public ConsumerBuilder WithUnitOfWork(IHandlerUnitOfWork unitOfWork)
        {
            return WithUnitOfWorkFactory(new HandlerUnitOfWorkFactoryStub(unitOfWork));
        }

        public ConsumerBuilder WithUnitOfWorkFactory(IHandlerUnitOfWorkFactory unitofWorkFactory)
        {
            _unitOfWorkFactory = unitofWorkFactory;
            return this;
        }

        public ConsumerBuilder WithConsumerScopeFactory(IConsumerScopeFactory consumerScopeFactory)
        {
            _consumerScopeFactory = consumerScopeFactory;
            return this;
        }

        public ConsumerBuilder WithMessageHandlerRegistry(MessageHandlerRegistry registry)
        {
            _registry = registry;
            return this;
        }

        public ConsumerBuilder WithEnableAutoCommit(bool enableAutoCommit)
        {
            _enableAutoCommit = enableAutoCommit;
            return this;
        }

        public void WithMessageFilter(MessageFilter messageFilter)
        {
            _messageFilter = messageFilter;
        }

        public ConsumerBuilder WithUnconfiguredMessageStrategy(
            IUnconfiguredMessageHandlingStrategy strategy)
        {
            _unconfiguredMessageStrategy = strategy;
            return this;
        }

        public ConsumerBuilder WithResiliencePipeline(MessageRegistration registration, ResiliencePipeline resiliencePipeline)
        {
            _mockResiliencePipelineProvider.Setup(m => m.GetPipelineFor(registration)).Returns(resiliencePipeline);
            return this;
        }

        public Consumer Build() =>
            new Consumer(
                _registry,
                _unitOfWorkFactory,
                _consumerScopeFactory,
                _unconfiguredMessageStrategy,
                _resiliencePipelineProvider,
                _messageFilter,
                _enableAutoCommit);
    }
}
