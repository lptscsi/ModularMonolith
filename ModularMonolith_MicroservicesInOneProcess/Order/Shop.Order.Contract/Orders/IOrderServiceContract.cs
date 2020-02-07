﻿using System.Threading.Tasks;
using Shop.Order.Contract.Orders.Dto;

namespace Shop.Order.Contract.Orders
{
    public interface IOrderServiceContract
    {
        Task<OrderDto> GetOrderAsync(int id);
        Task<int> CreateOrderAsync(CreateOrderDto createOrderDto);        
    }
}
