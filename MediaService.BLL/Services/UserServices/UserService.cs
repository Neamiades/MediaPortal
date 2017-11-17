﻿#region usings

using MediaService.BLL.DTO;
using MediaService.BLL.Interfaces;
using MediaService.DAL.Entities;
using MediaService.DAL.Interfaces;

#endregion

namespace MediaService.BLL.Services.UserServices
{
    public class UserService : Service<UserDto, User, string>, IUserService
    {
        public UserService(IUnitOfWork uow) : base(uow)
        {
            Repository = uow.Users;
        }
    }
}