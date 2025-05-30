using System.Diagnostics;
using Dapper;
using MySqlConnector;

namespace IPv64IPScanner;

public class DatabaseCreator
{
    public DatabaseCreator()
    {
    }

    private string _Connstring = "";

    public async void Init()
    {
        Process currentProcess = Process.GetCurrentProcess();
        CreateDatabaseString();
        Console.WriteLine($"Connection to Database...   ");
        bool isConnectionSuccess = await CheckConnection(currentProcess);
        if (!isConnectionSuccess)
        {
            Console.WriteLine($"Failed!", "Please check you're database connection data and restart again");
            return;
        }

        Console.WriteLine($"Success!");
        Console.WriteLine($"Database exists...   ");
        bool isDataBaseExists = await CheckIfDatabaseExists();
        if (!isDataBaseExists)
        {
            Console.WriteLine($"Not Found!");
            Console.WriteLine($"Creating Database...   ");
            bool isDatabseCreated = await CreateCompleteDatabase();
            if (!isDatabseCreated)
            {
                Console.WriteLine($"Failed!", "");
                return;
            }

            Console.WriteLine($"Success!");
        }
        else
        {
            Console.WriteLine($"Exist!");
        }

        CreateDatabaseString(true);
        await CreateAllTables();
    }

    #region CreateDatabaseString

    private void CreateDatabaseString(bool withDBName = false)
    {
        if (Env.DB_IP.Length == 0 || Env.DB_USER.Length == 0 || Env.DB_PW.Length == 0 || Env.DB_NAME.Length == 0)
        {
            throw new ApplicationException(
                "Database ConnectionString can't be create because one or more Environment variables are empty!");
        }

        if (!withDBName)
            _Connstring =
                $"server={Env.DB_IP};port={Env.DB_PORT};user={Env.DB_USER};password={Env.DB_PW};CHARSET=utf8mb4;";
        else
            _Connstring =
                $"server={Env.DB_IP};port={Env.DB_PORT};database={Env.DB_NAME};user={Env.DB_USER};password={Env.DB_PW};CHARSET=utf8mb4;";
    }

    #endregion

    #region CheckConnection

    public async Task<bool> CheckConnection(Process currentProcess)
    {
        try
        {
            using (var mySqlConnection = new MySqlConnection())
            {
                mySqlConnection.ConnectionString = _Connstring;
                await mySqlConnection.OpenAsync();
                mySqlConnection.Close();
                return true;
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine("",
                $"Error: can't connect to database!\n\n{exception.Message}\n\n{exception.StackTrace}");
            Console.ResetColor();
            // currentProcess.Kill(true);
            return false;
        }
    }

    #endregion

    #region CheckIfDatabaseExists

    public async Task<bool> CheckIfDatabaseExists()
    {
        try
        {
            string DB_NAME = Env.DB_NAME;
            //Console.WriteLine($"SHOW DATABASES LIKE '{DB_NAME}'");
            using (var mySqlConnection = new MySqlConnection())
            {
                dynamic? items = null;
                mySqlConnection.ConnectionString = _Connstring;
                items = await mySqlConnection.QueryAsync($"SHOW DATABASES LIKE '{DB_NAME}'");
                mySqlConnection.Close();
                return items.Count > 0;
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine("",
                $"Error: can't connect to database!\n\n{exception.Message}\n\n{exception.StackTrace}");
            Console.ResetColor();
            return false;
        }
    }

    #endregion

    #region CreateCompleteDatabase

    public async Task<bool> CreateCompleteDatabase()
    {
        await CreateDB();
        return true;
    }

    #endregion

    #region CreateDB

    private async Task CreateDB()
    {
        string DB_NAME = Env.DB_NAME;

        try
        {
            using (var mySqlConnection = new MySqlConnection())
            {
                mySqlConnection.ConnectionString = _Connstring;
                await mySqlConnection.QueryAsync($"CREATE DATABASE {DB_NAME}");
                mySqlConnection.Close();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine("",
                $"Error: can't connect to database!\n\n{exception.Message}\n\n{exception.StackTrace}");
            Console.ResetColor();
        }
    }

    #endregion

    #region CreateTables

    private async Task CreateAllTables()
    {
        try
        {
            using (var mySqlConnection = new MySqlConnection())
            {
                mySqlConnection.ConnectionString = _Connstring;
                await CreateTable(mySqlConnection, "IPList");
                mySqlConnection.Close();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine("",
                $"Error: can't connect to database!\n\n{exception.Message}\n\n{exception.StackTrace}");
            Console.ResetColor();
        }
    }

    private async Task CreateTable(MySqlConnection mySqlConnection, string table)
    {
        Console.WriteLine($"Exist {table}...   ");
        bool isTableExist = await CheckIfTableExists(mySqlConnection, table);
        if (isTableExist)
        {
            Console.WriteLine($"Exist!");
        }
        else
        {
            Console.WriteLine($"Not Found!");
            bool success = await CreateTableCustom(mySqlConnection, table);
            if (!success)
                throw new ApplicationException();
        }

        Console.ResetColor();
    }

    private async Task<bool> CheckIfTableExists(MySqlConnection connection, string table)
    {
        dynamic? items = null;
        items = await connection.QueryAsync($"SHOW TABLES LIKE '{table}'");
        return items.Count > 0;
    }

    private async Task<bool> CreateTableCustom(MySqlConnection connection, string table)
    {
        string sql = "";

        Console.WriteLine($"Table {table}...   ");

        switch (table)
        {
            case "IPList":
                sql =
                    $@"CREATE TABLE IF NOT EXISTS IPList (IP_ID int NOT NULL AUTO_INCREMENT, IP_Address text NOT NULL, IP_Blocked int(1) NOT NULL, IP_Queue int(1) NOT NULL, IP_ExtendedInfos text NOT NULL, IP_Added timestamp default current_timestamp() NOT NULL, IP_Changed timestamp default current_timestamp() NOT NULL, IP_MethodType int(11) default 1 not null, IP_ReportCount int(11) not null, PRIMARY KEY (IP_ID), index(IP_ID));";
                break;
        }

        try
        {
            await connection.QueryAsync(sql);
            Console.WriteLine($"Created!");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed!", e.StackTrace!);
            return false;
        }
    }

    #endregion
}