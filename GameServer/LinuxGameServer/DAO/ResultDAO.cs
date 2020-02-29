using System;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    class ResultDAO
    {
        public Result GetResultByUserid(MySqlConnection conn, int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from result where userid = @userid", conn);
                cmd.Parameters.AddWithValue("userid", userId);
                reader = cmd.ExecuteReader();
                Result result = null;
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    int totalCount = reader.GetInt32("totalcount");
                    int winCount = reader.GetInt32("wincount");
                    result = new Result(id, userId, totalCount, winCount);
                }
                else
                {
                    //这个数据不在数据库中
                    result = new Result(-1, userId, 0, 0);
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("在查询战绩的时候出现异常" + e);
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
        public void UpdateOrAddResult(MySqlConnection conn, Result result)
        {
            MySqlCommand cmd = null;
            try
            {
                if(result.Id == -1)
                {
                    cmd = new MySqlCommand("insert into result set totalcount=@totalcount,wincount=@wincount,userid=@userid", conn);
                }
                else
                {
                    cmd = new MySqlCommand("update result set totalcount=@totalcount,wincount=@wincount where userid=@userid", conn);
                }                
                cmd.Parameters.AddWithValue("totalcount", result.TotalCount);                
                cmd.Parameters.AddWithValue("wincount", result.WinCount);                
                cmd.Parameters.AddWithValue("userid", result.UserId);
                cmd.ExecuteNonQuery();
                //第一遍插入之后修改client持有的result，之后只用更新即可
                if (result.Id == -1)
                {
                    Result temp = GetResultByUserid(conn, result.UserId);
                    result.Id = temp.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在更新战绩的时候出现异常" + e);
            }
        }
    }
}
