using Newtonsoft.Json;

namespace IPv64IPScanner;

public class IPList : DBBase
{
    public int IP_ID { get; set; }
    public string IP_Address { get; set; }
    public bool IP_Blocked { get; set; }
    public bool IP_Queue { get; set; }
    public string IP_ExtendedInfos { get; set; }
    public DateTime IP_Added { get; set; }
    public DateTime IP_Changed { get; set; }
    public int IP_MethodType { get; set; } = (int)BlockMethods.HTTP_S;
    public int IP_ReportCount { get; set; }

    public async Task<List<IPList>> LoadAll(bool onlyBlocked = false, bool onlyQueue = false, int method = -1)
    {
        string filter = "";

        if (onlyBlocked)
            filter = "where ip.IP_Blocked = 1";
        else if (onlyQueue)
            filter = "where ip.IP_Queue = 1";

        if (method > -1)
            filter = $"{filter} and ip.IP_MethodType = {method}";

        IPList ipList = new IPList();
        dynamic data = await ipList.SelectFromSql($"select * from IPList ip {filter} ORDER BY INET6_ATON(ip.IP_Address)");
        var jsonData = JsonConvert.SerializeObject(data);
        List<IPList> ipLists = JsonConvert.DeserializeObject<List<IPList>>(jsonData);
        return ipLists;
    }

    public async Task<IPList?> LoadByIP(string ip)
    {
        IPList ipList = new IPList();
        dynamic data = await ipList.SelectFromSql($"select * from IPList ip where ip.IP_Address = '{ip}'");
        var jsonData = JsonConvert.SerializeObject(data);
        List<IPList> ipLists = JsonConvert.DeserializeObject<List<IPList>>(jsonData);
        return ipLists.FirstOrDefault();
    }
}