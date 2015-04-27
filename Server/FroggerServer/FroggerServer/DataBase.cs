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
    SQLiteConnection m_dbConnection;

    DataBase()
    {
    }

    public bool open()
    {
        if (!File.Exists(dbname))
        {
            SQLiteConnection.CreateFile(dbname);
            return true;
        }
        else 
        {
            //Edit me if you need to rename Database.
            if (!dbSet)
            {
                m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                dbSet = !dbSet;
            }
            m_dbConnection.Open();
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
