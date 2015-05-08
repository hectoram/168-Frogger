using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

// Ceci
using System.Security.Cryptography;
using System.Globalization;


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

        string sqlSalt = "SELECT salt from Login where username like '" + username + "'";
        SQLiteCommand retrieveSalt = new SQLiteCommand(sqlSalt, m_dbConnection);
        var salt = retrieveSalt.ExecuteScalar();
        string passSalt = salt.ToString();


        string sql = "SELECT COUNT(*) from Login where username like '" + username + "' AND " + "password like '" + HashPassword(password, passSalt) + "'";
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
            string salt = GenerateSaltValue();

            string sql = "insert into Login (username, password, salt) values ";
            string temp = "('" + username + "', " + "'" + HashPassword(password, salt) + "', " + "'" + salt + "')";
            sql += temp;
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            return true;
        }
        return false;
    }
    

        //Ceci's
    private static string GenerateSaltValue()
    {
        UnicodeEncoding utf16 = new UnicodeEncoding();

        if (utf16 != null)
        {
            // Create a random number object seeded from the value
            // of the last random seed value. This is done
            // interlocked because it is a static value and we want
            // it to roll forward safely.

            Random random = new Random(unchecked((int)DateTime.Now.Ticks));

            if (random != null)
            {
                // Create an array of random values.

                byte[] saltValue = new byte[20];

                random.NextBytes(saltValue);

                // Convert the salt value to a string. Note that the resulting string
                // will still be an array of binary values and not a printable string. 
                // Also it does not convert each byte to a double byte.

                string saltValueString = utf16.GetString(saltValue);

                // Return the salt value as a string.

                return saltValueString;
            }
        }

        return null;
    }


    // For hashing passwords
    private static string HashPassword(string clearData, string saltValue)
    {
        // Convert clearData and salt value into a byte array
        byte[] clearDataBytes = Encoding.UTF8.GetBytes(clearData);
        byte[] saltValueBytes = Encoding.UTF8.GetBytes(saltValue);

        // Allocate new array which will hold both
        byte[] clearDataSaltBytes = new byte[clearDataBytes.Length + saltValueBytes.Length];
        for (int i = 0; i < clearDataBytes.Length; i++)
        {
            clearDataSaltBytes[i] = clearDataSaltBytes[i];
        }
        for (int i = 0; i < saltValueBytes.Length; i++)
        {
            clearDataSaltBytes[clearDataBytes.Length + i] = clearDataSaltBytes[i];
        }

        // Define hash algorithm
        HashAlgorithm hash = new SHA1Managed();

        // Compute hash value
        byte[] hashBytes = hash.ComputeHash(clearDataSaltBytes);

        // Convert result into a base64-encoded string    
        string hashedPassword = Convert.ToBase64String(hashBytes);
        return hashedPassword;
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
