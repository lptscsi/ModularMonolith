﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Shop.Communication.Contract;
using Shop.Framework.UseCases.Interfaces.Services;
using Shop.Order.DataAccess.Interfaces;

namespace Shop.Order.UseCases.Orders.Commands.CreateOrder
{
    internal class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest, int>
    {
        private readonly IOrderDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICommunicationContract _communicationContract;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUrlHelper _urlHelper;

        public CreateOrderRequestHandler(
            IOrderDbContext dbContext, 
            IMapper mapper, 
            ICommunicationContract communicationContract,
            ICurrentUserService currentUserService,
            IUrlHelper urlHelper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _communicationContract = communicationContract;
            _currentUserService = currentUserService;
            _urlHelper = urlHelper;
        }

        public async Task<int> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Entities.Order>(request.CreateOrderDto);
            order.CreationDate = DateTime.UtcNow;
            order.UserId = _currentUserService.Id;

            _dbContext.Orders.Add(order);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var orderDetailsUrl = _urlHelper.GetOrderDetails(order.Id);
            await _communicationContract.ScheduleOrderCreatedEmailAsync(_currentUserService.Email, order.Id, orderDetailsUrl, cancellationToken);

            return order.Id;
        }
    }
}
