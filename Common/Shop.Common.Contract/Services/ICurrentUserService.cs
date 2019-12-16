﻿namespace Shop.Common.Contract.Services
{
    public interface ICurrentUserService
    {
        int Id { get; }
        bool IsAuthenticated { get; }
        string Email { get; set; }
    }
}
