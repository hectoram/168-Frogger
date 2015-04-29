using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;


namespace FroggerServer
{
    class DataBase
    {


    private static DataBase instance = null;
    private static readonly object padlock = new object();
    private string dbname = "MyDatabase.sqlite";
    private bool dbSet = false;
    private SQLiteConnection m_dbConnection;

    DataBase()
    {

    }

    //Check to see if the user/password combo exits in the DB
    private bool userExists(string username, string password) 
    {
        if (!open())
        {
            return false;
        }

        string sql = "SELECT COUNT(*) from Login where username like '" + username + "'";
        SQLiteCommand checkForUser = new SQLiteCommand(sql,m_dbConnection);

        var userCount = checkForUser.ExecuteScalar();
        userCount.ToString();
        int count = Convert.ToInt32(userCount);

        if (count > 0)
            return true;
        else
            return false;
    }

    private bool checkCredentials(string username, string password)
    {
        if (!open())
        {
            return false;
        }

        string sql = "SELECT COUNT(*) from Login where username like '" + username + "' AND " + "password like '" + password + "'";
        SQLiteCommand checkForUser = new SQLiteCommand(sql, m_dbConnection);

        var userCount = checkForUser.ExecuteScalar();
        userCount.ToString();
        int count = Convert.ToInt32(userCount);

        if (count > 0)
            return true;
        else
            return false;
    }

    public bool login(string username, string password) 
    {
        if (open() && checkCredentials(username, password))
            return true;
        else
            return false;
    }

    public bool registerUser(string username, string password)
    {
        if (open() && !userExists(username, password))
        {
            string sql = "insert into Login (username, password) values ";
            string temp = "('" + username + "', " + "'" + password + "')";
            sql += temp;
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            return true;
        }
        return false;
    }
    
    private bool open()
    {
        if (!File.Exists(dbname))
        {
            SQLiteConnection.CreateFile(dbname);
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();

            string sql = "create table Login (username varchar(20), password varchar(20))";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            return true;
        }
        else 
        {
            //Edit me if you need to rename Database.
            if (!dbSet)
            {
                m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                dbSet = !dbSet;
                m_dbConnection.Open();
            }
            return true;

        }
    }
    

    public static DataBase Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new DataBase();
                }
                return instance;
            }
        }
    }
  }
}
