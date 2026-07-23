using SecondChance.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(UserRegisterDto request);
    }
}
