using System;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    class CoinDAO
    {
        public Coin GetCoinByUserid(MySqlConnection conn, int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from coin where userid = @userid", conn);
                cmd.Parameters.AddWithValue("userid", userId);
                reader = cmd.ExecuteReader();
                Coin coin = null;
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    int coinNum = reader.GetInt32("coinnum");
                    coin = new Coin(id, userId, coinNum);
                }
                else
                {
                    //这个数据不在数据库中
                    coin = new Coin(-1, userId, 0);
                }
                return coin;
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
        public void UpdateOrAddCoin(MySqlConnection conn, Coin coin)
        {
            MySqlCommand cmd = null;
            try
            {
                if (coin.Id == -1)
                {
                    cmd = new MySqlCommand("insert into coin set coinnum=@coinnum,userid=@userid", conn);
                }
                else
                {
                    cmd = new MySqlCommand("update coin set coinnum=@coinnum where userid=@userid", conn);
                }
                cmd.Parameters.AddWithValue("coinnum", coin.CoinNum);
                cmd.Parameters.AddWithValue("userid", coin.UserId);
                cmd.ExecuteNonQuery();
                //第一遍插入之后修改client持有的coin，之后只用更新即可
                if (coin.Id == -1)
                {
                    Coin temp = GetCoinByUserid(conn, coin.UserId);
                    coin.Id = temp.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在更新coin的时候出现异常" + e);
            }
        }
    }
}
