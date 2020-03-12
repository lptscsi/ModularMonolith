﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shop.Framework.Interfaces.Exceptions;
using Shop.Framework.Interfaces.Messaging;
using Shop.Identity.Contract.Identity.UserDetails;
using Shop.Order.Contract.Orders.Dto;
using Shop.Order.Contract.Orders.Messages.GetOrder;
using Shop.Order.DomainServices.Interfaces;
using Shop.Order.Infrastructure.Interfaces.DataAccess;

namespace Shop.Order.UseCases.Orders.Queries
{
    internal class GetOrderMessageHandler : MessageHandler<GetOrderRequestMessage>
    {
        private readonly IOrderDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IOrdersService _orderService;
        private readonly IMessageDispatcher _messageDispatcher;

        public GetOrderMessageHandler(IOrderDbContext dbContext, 
            IMapper mapper, IOrdersService orderService, 
            IMessageBroker messageBroker,
            IMessageDispatcher messageDispatcher)
            : base(messageBroker)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _orderService = orderService;
            _messageDispatcher = messageDispatcher;
        }

        protected override async Task Handle(GetOrderRequestMessage message)
        {
            var order = await _dbContext.Orders.AsNoTracking()
                .Include(x => x.Items).ThenInclude(x => x.Product)
                .SingleOrDefaultAsync(x => x.Id == message.Id);

            if (order == null) throw new EntityNotFoundException();

            var request = new UserDetailsRequestMessage ();
            var userDetails = await _messageDispatcher.SendMessageAsync<UserDetailsResponseMessage>(request);
            if (userDetails.UserDetailsDto.LockoutEnd.HasValue && userDetails.UserDetailsDto.LockoutEnd > DateTime.Now)
                throw new InvalidOperationException("User locked");

            var result = _mapper.Map<OrderDto>(order);
            result.Price = _orderService.GetPrice(order);

            var resultMessage = new GetOrderResponseMessage {Order = result};
            await MessageBroker.PublishAsync(resultMessage);            
        }
    }
}
