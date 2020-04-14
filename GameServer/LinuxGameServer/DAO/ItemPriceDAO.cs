using System;
using System.Collections.Generic;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    class ItemPriceDAO
    {
        public Dictionary<string,ItemPrice> GetItemPriceByShopId(MySqlConnection conn, int shopId)
        {
            MySqlDataReader reader = null;
            try
            {
                Dictionary<string,ItemPrice> itemPrices = new Dictionary<string, ItemPrice>();
                MySqlCommand cmd = new MySqlCommand("select * from itemprice where shopid = @shopid", conn);
                cmd.Parameters.AddWithValue("shopid", shopId);
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("id");
                        int itemId = reader.GetInt32("itemid");
                        int price = reader.GetInt32("price");
                        string itemName = reader.GetString("itemname");                     
                        itemPrices.Add(itemName,new ItemPrice(id, shopId, itemId, price, itemName));
                    }
                }
                else
                {
                    //这个数据不在数据库中
                    itemPrices.Add("noItem",new ItemPrice(-1, -1, -1, -1, "noItem"));
                }
                foreach (var item in itemPrices.Values)
                {
                    Console.WriteLine(item.Price);
                }
                return itemPrices;
            }
            catch (Exception e)
            {
                Console.WriteLine("在查询商品价格表的时候出现异常" + e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return null;
        }
    }
}
