namespace IPv64IPScanner;

public static class Env
{
    public static string DB_IP
    {
        get
        {
            return Environment.GetEnvironmentVariable("DB_IP") ?? "";
        }
    }
    
    public static string DB_NAME
    {
        get
        {
            return Environment.GetEnvironmentVariable("DB_NAME") ?? "IPv64IpScanner";
        }
    }
    
    public static string DB_USER
    {
        get
        {
            return Environment.GetEnvironmentVariable("DB_USER") ?? "IPv64IpScanner";
        }
    }
    
    public static string DB_PW
    {
        get
        {
            return Environment.GetEnvironmentVariable("DB_PW") ?? "";
        }
    }
    
    public static int DB_PORT
    {
        get
        {
            string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
            return int.Parse(port);
        }
    }
    
    public static int IP_TASK_COUNT
    {
        get
        {
            string count = Environment.GetEnvironmentVariable("IP_TASK_COUNT") ?? "10";
            return int.Parse(count);
        }
    }
    
    public static int IP_TASK_INTERVAL
    {
        get
        {
            string interval = Environment.GetEnvironmentVariable("IP_TASK_INTERVAL") ?? "5";
            return int.Parse(interval);
        }
    }
}