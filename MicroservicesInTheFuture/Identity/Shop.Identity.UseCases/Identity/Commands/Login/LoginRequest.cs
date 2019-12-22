﻿using MediatR;
using Shop.Identity.Contract.Identity.Dto;

namespace Shop.Identity.UseCases.Identity.Commands.Login
{
    internal class LoginRequest : IRequest
    {
        public LoginDto LoginDto { get; set; }
    }
}