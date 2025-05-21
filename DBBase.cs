using System.Reflection;
using Dapper;
using MySqlConnector;

namespace IPv64IPScanner;

public abstract class DBBase
{
    private string? _connString = null;

    public DBBase()
    {
    }

    private void CreateConnection()
    {
        if (Env.DB_IP.Length == 0 || Env.DB_USER.Length == 0 || Env.DB_PW.Length == 0 || Env.DB_NAME.Length == 0)
        {
            Console.WriteLine(
                "Database ConnectionString can't be create because one or more Environment variables are empty!");
            return;
            throw new ApplicationException(
                "Database ConnectionString can't be create because one or more Environment variables are empty!");
        }

        _connString =
            $"server={Env.DB_IP};port={Env.DB_PORT};database={Env.DB_NAME};user={Env.DB_USER};password={Env.DB_PW};CHARSET=utf8mb4;";
    }

    public async Task<dynamic> SelectFromSql(string sql)
    {
        CreateConnection();
        dynamic? items = null;
        using (MySqlConnection connection = new MySqlConnection())
        {
            connection.ConnectionString = _connString;
            await connection!.OpenAsync(); //vs  connection.Open();
            items = await connection.QueryAsync(sql);
            //Console.WriteLine(items);
            connection.Close();
        }

        return items;
    }

    public async Task<int> Insert()
    {
        CreateConnection();
        string sql = CreateInsertString();
        int insertedItem = 0;

        using (MySqlConnection connection = new MySqlConnection())
        {
            connection.ConnectionString = _connString;
            await connection!.OpenAsync(); //vs  connection.Open();
            insertedItem = await connection.ExecuteScalarAsync<int>(sql, this);
            //Console.WriteLine(insertedItem);
            connection.Close();
        }

        return insertedItem;
    }

    private string CreateInsertString()
    {
        string classname = GetType().Name;
        PropertyInfo[] propertyInfoList = GetType().GetProperties();
        string columns = "";
        string values = "";
        string pkColumn = "";
        int count = 0;

        foreach (PropertyInfo info in propertyInfoList)
        {
            if (info.PropertyType == typeof(int) && count == 0)
            {
                pkColumn = info.Name;
            }

            if (columns.Length == 0)
                columns += info.Name;
            else
                columns += $", {info.Name}";

            if (values.Length == 0)
                values += $"@{info.Name}";
            else
                values += $", @{info.Name}";

            count++;
        }

        return
            $"INSERT INTO {classname}({columns}) Values({values}); select * from {classname} where {pkColumn} = LAST_INSERT_ID();";
        ;
    }

    public async Task<bool> Update()
    {
        CreateConnection();
        string sql = CreateUpdateString();
        bool isSuccess = false;
        
        using (MySqlConnection connection = new MySqlConnection())
        {
            connection.ConnectionString = _connString;
            await connection!.OpenAsync(); //vs  connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var affectedRows = connection.Execute(sql, this, transaction: transaction);
                transaction.Commit();
                if (affectedRows > 0)
                    isSuccess = true;
            }

            connection.Close();
        }

        return isSuccess;
    }

    private string CreateUpdateString()
    {
        string classname = GetType().Name;
        PropertyInfo[] propertyInfoList = GetType().GetProperties();
        string columnValue = "";
        string pkColumn = "";
        string pkValue = "";
        int count = 0;

        foreach (PropertyInfo info in propertyInfoList)
        {
            if (info.PropertyType == typeof(int) && count == 0)
            {
                pkColumn = info.Name;
                pkValue = $"@{info.Name}";
            }

            if (columnValue.Length == 0)
                columnValue += $"{info.Name} = @{info.Name}";
            else
                columnValue += $", {info.Name} = @{info.Name}";

            count++;
        }

        return $"UPDATE {classname} SET {columnValue} WHERE {pkColumn} = {pkValue};";
        ;
    }

    public async Task<bool> Delete(string pk, int? id)
    {
        CreateConnection();
        string sql = CreateDeleteString(pk, id);
        bool isSuccess = false;
        
        using (MySqlConnection connection = new MySqlConnection())
        {
            connection.ConnectionString = _connString;
            await connection!.OpenAsync(); //vs  connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                var affectedRows = connection.Execute(sql, this, transaction: transaction);
                transaction.Commit();
                if (affectedRows > 0)
                    isSuccess = true;
            }

            connection.Close();
        }

        return isSuccess;
    }

    private string CreateDeleteString(string pk, int? id)
    {
        string classname = GetType().Name;
        return $"DELETE FROM {classname} WHERE {pk} = {id};";
    }

    public async Task<dynamic> ExecuteSQL(string sql)
    {
        CreateConnection();
        dynamic? items = null;
        
        using (MySqlConnection connection = new MySqlConnection())
        {
            connection.ConnectionString = _connString;
            await connection!.OpenAsync(); //vs  connection.Open();
            items = await connection.QueryAsync(sql);
            //Console.WriteLine(items);
            connection.Close();
        }
        
        return items;
    }
}