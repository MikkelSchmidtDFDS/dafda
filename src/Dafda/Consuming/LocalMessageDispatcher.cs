using Dafda.Consuming.Interfaces;
using Polly;
using Polly.Registry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dafda.Consuming
{
    internal class LocalMessageDispatcher
    {
        private readonly MessageHandlerRegistry _messageHandlerRegistry;
        private readonly IHandlerUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IUnconfiguredMessageHandlingStrategy _fallbackHandler;
        private readonly IResiliencePipelineProvider _resiliencePipelineProvider;

        public LocalMessageDispatcher(
            MessageHandlerRegistry messageHandlerRegistry,
            IHandlerUnitOfWorkFactory handlerUnitOfWorkFactory,
            IUnconfiguredMessageHandlingStrategy fallbackHandler,
            IResiliencePipelineProvider resiliencePipelineProvider)
        {
            _messageHandlerRegistry =
                messageHandlerRegistry
                ?? throw new ArgumentNullException(nameof(messageHandlerRegistry));
            _unitOfWorkFactory =
                handlerUnitOfWorkFactory
                ?? throw new ArgumentNullException(nameof(handlerUnitOfWorkFactory));
            _fallbackHandler =
                fallbackHandler
                ?? throw new ArgumentNullException(nameof(fallbackHandler));
            _resiliencePipelineProvider = 
                resiliencePipelineProvider
                ?? throw new ArgumentNullException(nameof(resiliencePipelineProvider));
        }

        private MessageRegistration GetMessageRegistrationFor(MessageResult messageResult)
        {
            return _messageHandlerRegistry.GetRegistrationFor(messageResult.Topic, messageResult.Message.Metadata.Type)
            ?? _fallbackHandler.GetFallback(messageResult.Message.Metadata.Type);
        }

        public async Task Dispatch(MessageResult messageResult, CancellationToken cancellationToken)
        {
            var registration = GetMessageRegistrationFor(messageResult);

            var unitOfWork = _unitOfWorkFactory.CreateForHandlerType(registration.HandlerInstanceType);
            if (unitOfWork == null)
            {
                throw new UnableToResolveUnitOfWorkForHandlerException($"Error! Unable to create unit of work for handler type \"{registration.HandlerInstanceType.FullName}\".");
            }

            var message = messageResult.Message;
            var messageInstance = message.ReadDataAs(registration.MessageInstanceType);
            var context = new MessageHandlerContext(message.Metadata);
            await unitOfWork.Run(async (handler, cancellationToken) =>
            {
                if (handler == null)
                {
                    throw new InvalidMessageHandlerException($"Error! Message handler of type \"{registration.HandlerInstanceType.FullName}\" not instantiated in unit of work and message instance type of \"{registration.MessageInstanceType}\" for message type \"{registration.MessageType}\" can therefor not be handled.");
                }

                var pipelineName = $"{registration.HandlerInstanceType.FullName}-{registration.Topic}-{registration.MessageType}";

                var resiliencePipeline = _resiliencePipelineProvider.GetPipelineFor(registration);

                await resiliencePipeline.ExecuteAsync(async ct => await ExecuteHandler((dynamic) messageInstance, (dynamic) handler, context, ct), cancellationToken);
            }, cancellationToken);
        }

        private static async Task<object> ExecuteHandler<TMessage>(
            TMessage message,
            IMessageHandler<TMessage> handler,
            MessageHandlerContext context,
            CancellationToken cancellationToken)
        {
            await handler.Handle(message, context, cancellationToken);
            return null;
        }
    }
}