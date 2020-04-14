using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Model
{
    class RoleShopState
    {
        public RoleShopState(int id,int userId,string roleBuy) {
            Id = id;
            UserId = userId;
            RoleBuy = roleBuy;
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RoleBuy { get; set; }
    }
}
