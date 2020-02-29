using MySql.Data.MySqlClient;
using System;
namespace GameServer.Tool
{
    class ConnHelper
    {
        //Database=parkour;Data Source=172.21.0.13;port=3306;User Id=root;Password=password_root;
        //Database=parkour;Data Source=127.0.0.1;port=3306;User Id=root;Password=crzz;
        public const string ConnString = "Database=parkour;Data Source=127.0.0.1;port=3306;User Id=root;Password=crzz;";
        public static MySqlConnection Connect()
        {
            MySqlConnection conn = new MySqlConnection(ConnString);
            try 
            {               
                conn.Open();
                Console.WriteLine("已连接数据库");
                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine("连接数据库的时候出现异常" + e);
            }
            return null;
        }

        public static void CloseConnection(MySqlConnection conn)
        {
            if(conn != null)
            {
                conn.Close();
            }
            else
            {
                Console.WriteLine("连接为空");
            }
        }
    }
}
