using GameServer.Model;
using MySql.Data.MySqlClient;
using System;

namespace GameServer.DAO
{
    class ShopStateDAO
    {
        public ShopState GetShopStateByUserid(MySqlConnection conn, int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from shopstate where userid = @userid", conn);
                cmd.Parameters.AddWithValue("userid", userId);
                reader = cmd.ExecuteReader();
                ShopState ShopState = null;
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    int healthTime = reader.GetInt32("healthtime");
                    int bigHealthTime = reader.GetInt32("bighealthtime");
                    int skillTimeTime = reader.GetInt32("skilltimetime");
                    int bigSkillTimeTime = reader.GetInt32("bigskilltimetime");
                    ShopState = new ShopState(id, userId, healthTime, bigHealthTime, skillTimeTime, bigSkillTimeTime);
                }
                else
                {
                    //默认全部为后买0次
                    ShopState = new ShopState(-1, userId, 0, 0, 0, 0);
                }
                return ShopState;
            }
            catch (Exception e)
            {
                Console.WriteLine("在查询用户商店信息的时候出现异常" + e);
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
        public void UpdateOrAddShopState(MySqlConnection conn, ShopState shopState)
        {
            MySqlCommand cmd = null;
            try
            {
                if (shopState.Id == -1)
                {
                    cmd = new MySqlCommand("insert into shopstate set healthtime=@healthtime,bighealthtime=@bighealthtime,skilltimetime=@skilltimetime,bigskilltimetime=@bigskilltimetime,userid=@userid", conn);
                }
                else
                {
                    cmd = new MySqlCommand("update shopstate set healthtime=@healthtime,bighealthtime=@bighealthtime,skilltimetime=@skilltimetime,bigskilltimetime=@bigskilltimetime where userid=@userid", conn);
                }
                cmd.Parameters.AddWithValue("healthtime", shopState.HealthTime);
                cmd.Parameters.AddWithValue("bighealthtime", shopState.BigHealthTime);
                cmd.Parameters.AddWithValue("skilltimetime", shopState.SkillTimeTime);
                cmd.Parameters.AddWithValue("bigskilltimetime", shopState.BigSkillTimeTime);
                cmd.Parameters.AddWithValue("userid", shopState.UserId);
                cmd.ExecuteNonQuery();
                //第一遍插入之后修改client持有的playerState，之后只用更新即可
                if (shopState.Id == -1)
                {
                    ShopState temp = GetShopStateByUserid(conn, shopState.UserId);
                    shopState.Id = temp.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在更新shopstate的时候出现异常" + e);
            }
        }
    }
}
