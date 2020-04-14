using GameServer.Model;
using MySql.Data.MySqlClient;
using System;

namespace GameServer.DAO
{
    class RoleShopStateDAO
    {
        public RoleShopState GetRoleShopStateByUserId(MySqlConnection conn,int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from roleshopstate where userid = @userid", conn);
                cmd.Parameters.AddWithValue("userid", userId);
                reader = cmd.ExecuteReader();
                RoleShopState roleShopState = null;
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string rolebuy = reader.GetString("rolebuy");
                    roleShopState = new RoleShopState(id, userId, rolebuy);
                }
                else
                {
                    //默认只买了第一个
                    roleShopState = new RoleShopState(-1, userId, "1,0,0");
                }
                return roleShopState;
            }
            catch (Exception e)
            {
                Console.WriteLine("在查询角色商店信息的时候出现异常" + e);
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
        public void UpdateOrAddRoleShopState(MySqlConnection conn, RoleShopState roleShopState)
        {
            MySqlCommand cmd = null;
            try
            {
                if (roleShopState.Id == -1)
                {
                    cmd = new MySqlCommand("insert into roleShopstate set userid=@userid,rolebuy=@rolebuy", conn);
                }
                else
                {
                    cmd = new MySqlCommand("update roleShopstate set userid=@userid,rolebuy=@rolebuy", conn);
                }
                cmd.Parameters.AddWithValue("rolebuy", roleShopState.RoleBuy);
                cmd.Parameters.AddWithValue("userid", roleShopState.UserId);
                cmd.ExecuteNonQuery();
                //第一遍插入之后修改client持有的playerState，之后只用更新即可
                if (roleShopState.Id == -1)
                {
                    RoleShopState temp = GetRoleShopStateByUserId(conn, roleShopState.UserId);
                    roleShopState.Id = temp.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在更新roleshopstate的时候出现异常" + e);
            }
        }
    } 
}
