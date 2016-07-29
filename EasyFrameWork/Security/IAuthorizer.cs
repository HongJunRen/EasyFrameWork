﻿using Easy.Models;
using Easy.Modules.User.Models;

namespace Easy.Security
{
    public interface IAuthorizer
    {
        bool Authorize(string permission);
        bool Authorize(string permission, IUser user);
    }
}