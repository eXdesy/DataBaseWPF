using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mysqlx.Cursor;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Windows.Controls;

namespace DataBaseWPF.database
{
    public class Sql
    {
        public static MySqlConnection? con;
        private static bool isConnected = false;

        public static Boolean Connect(string database, string user, string password)
        {
            if (!isConnected)
            {
                con = new MySqlConnection("server=localhost;port=3306;database=" + database + ";user=" + user + ";password=" + password);
                try
                {
                    // La conexión está abierta
                    con.Open();
                    isConnected = true;
                    Console.WriteLine("Connection established");
                }
                catch (SqlException e)
                {
                    throw new ApplicationException("Error establishing connection: " + e.Message);
                }
            }
            return true;
        }

        public static void Close()
        {
            if (isConnected && con != null)
            {
                try
                {
                    // La conexión está cerrada
                    con.Close();
                    isConnected = false; 
                    Console.WriteLine("Connection closed");
                }
                catch (SqlException e)
                {
                    throw new ApplicationException("Error closing connection: " + e.Message);
                }
            }
        }
    }
}