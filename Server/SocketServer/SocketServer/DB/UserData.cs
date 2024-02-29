using MySql.Data.MySqlClient;
using SocketGameProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.DB
{
    class UserData
    {
        private MySqlConnection _mysqlConnect;
        private string _connectStr = "database=sockettest;data source=localhost;user=root;password=admin123;" +
            "pooling=false;charset=utf8mb4;port=3306";
        public UserData()
        {
            ConnectMysql();
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        private void ConnectMysql()
        {
            try
            {
                // 连接数据库并运行
                _mysqlConnect = new MySqlConnection(_connectStr);
                _mysqlConnect.Open();
            }
            catch (Exception ex)
            {
                // 防止卡住进程抛出异常
                Console.WriteLine("连接数据库失败" + ex.Message);
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public bool Register(MainPack pack)
        {
            string userName = pack.RegisterPack.UserName;
            string password = pack.RegisterPack.Password;
            string sql = "SELECT * FROM sockettest.userdata where username=@userName";
            using(MySqlCommand command = new MySqlCommand(sql, _mysqlConnect))
            {
                command.Parameters.AddWithValue("@userName", userName);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("输入的用户名已存在");
                        return false;
                    }
                }
            }

            sql = "INSERT INTO userdata (username, password) VALUES (@userName, @password)";
            using (MySqlCommand command = new MySqlCommand(sql, _mysqlConnect))
            {
                // 添加参数
                command.Parameters.AddWithValue("@userName", userName);
                command.Parameters.AddWithValue("@password", password);
                // 执行插入
                command.ExecuteNonQuery();
                return true;
            }
        }
        public bool Login(MainPack pack)
        {
            // ... 有可能让一个账号被两个客户端登录 待解决
            string userName = pack.RegisterPack.UserName;
            string password = pack.RegisterPack.Password;
            string query = "SELECT * FROM userdata WHERE username = @username " +
                "AND password = @password";
            using (MySqlCommand command = new MySqlCommand(query, _mysqlConnect))
            {
                // 设置参数
                command.Parameters.AddWithValue("@username", userName);
                command.Parameters.AddWithValue("@password", password);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // 账号密码都正确则返回登录成功
                    if (reader.HasRows)
                    {
                        Console.WriteLine("用户登录成功，登录账号："+userName);
                        return true;
                    }
                    else
                        return false;
                }
            }
        }
    }
}
