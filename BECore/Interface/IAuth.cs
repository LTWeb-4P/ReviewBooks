using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BECore.Dto;
using BECore.Models;

namespace BECore.Interface
{
    public interface IAuth
    {
        public Task<List<User>> GetUsersAsync();
    }
}