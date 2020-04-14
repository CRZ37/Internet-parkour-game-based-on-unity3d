using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Model
{
    class ItemPrice
    {
        public ItemPrice(int id, int shopId, int itemId, int price, string itemName)
        {
            Id = id;
            ShopId = shopId;
            ItemId = itemId;
            Price = price;
            ItemName = itemName;
        }

        public int Id { get; set; }
        public int ShopId { get; set; }
        public int ItemId { get; set; }
        public int Price { get; set; }
        public string ItemName { get; set; }
    }
}
