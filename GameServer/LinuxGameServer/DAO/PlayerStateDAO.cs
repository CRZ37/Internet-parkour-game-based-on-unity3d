using GameServer.Model;
using MySql.Data.MySqlClient;
using System;

namespace GameServer.DAO
{
    class PlayerStateDAO
    {
        public PlayerState GetPlayerStateByUserid(MySqlConnection conn, int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from playerstate where userid = @userid", conn);
                cmd.Parameters.AddWithValue("userid", userId);
                reader = cmd.ExecuteReader();
                PlayerState playerState = null;
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    Console.WriteLine(id);
                    int health = reader.GetInt32("health");
                    float skillTime = reader.GetFloat("skilltime");
                    string roleSelect = reader.GetString("roleselect");
                    playerState = new PlayerState(id, userId, health,skillTime,roleSelect);
                }
                else
                {
                    //默认生命值为6，技能时间为2秒,角色选择为第1个
                    playerState = new PlayerState(-1, userId, 6,2,"1");
                }
                return playerState;
            }
            catch (Exception e)
            {
                Console.WriteLine("在查询金币的时候出现异常" + e);
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
        public void UpdateOrAddPlayerState(MySqlConnection conn, PlayerState playerState)
        {
            MySqlCommand cmd = null;
            try
            {
                if (playerState.Id == -1)
                {
                    cmd = new MySqlCommand("insert into playerstate set health=@health,skilltime=@skilltime,roleselect=@roleselect,userid=@userid", conn);
                }
                else
                {
                    cmd = new MySqlCommand("update playerstate set health=@health,skilltime=@skilltime,roleselect=@roleselect where userid=@userid", conn);
                }
                cmd.Parameters.AddWithValue("health", playerState.Health);
                cmd.Parameters.AddWithValue("skilltime", playerState.SkillTime);
                cmd.Parameters.AddWithValue("roleselect", playerState.RoleSelect);
                cmd.Parameters.AddWithValue("userid", playerState.UserId);
                cmd.ExecuteNonQuery();
                //第一遍插入之后修改client持有的playerState，之后只用更新即可
                if (playerState.Id == -1)
                {
                    PlayerState temp = GetPlayerStateByUserid(conn, playerState.UserId);
                    playerState.Id = temp.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在更新playerstate的时候出现异常" + e);
            }
        }
    }
}
