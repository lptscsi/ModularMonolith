﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Shop.Framework.Interfaces.Messaging;
using Shop.Order.Contract.Orders.Messages.CreateOrder;
using Shop.Order.UseCases.Orders.Commands.CancelOrder;

namespace Shop.Order.UseCases
{
    public class OrdersExceptionMessageHandler : INotificationHandler<ExceptionMessage>
    {
        private readonly IMessageStore _messageStore;
        private readonly IMessageBroker _messageBroker;

        public OrdersExceptionMessageHandler(IMessageStore messageStore, IMessageBroker messageBroker)
        {
            _messageStore = messageStore;
            _messageBroker = messageBroker;
        }
        public async Task Handle(ExceptionMessage message, CancellationToken cancellationToken)
        {
            var messages = await _messageStore.AllAsync(message.CorrelationId);

            var cancelMessage = messages.FirstOrDefault(x => x is CancelOrderMesage);
            if (cancelMessage != null) return; // already canceled

            var orderCreatedMessage = (CreateOrderResponseMessage) messages.FirstOrDefault(x => x is CreateOrderResponseMessage);
            if (orderCreatedMessage != null)
            {
                var cancelOrderMessage = new CancelOrderMesage
                {
                    CorrelationId = message.CorrelationId, 
                    OrderId = orderCreatedMessage.OrderId
                };
                await _messageBroker.PublishAsync(cancelOrderMessage);
            }
        }
    }
}
