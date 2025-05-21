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

    public async Task<List<IPList>> LoadAll(bool onlyBlocked = false, bool onlyQueue = false)
    {
        string filter = "";

        if (onlyBlocked)
            filter = "where ip.IP_Blocked = 1";
        else if (onlyQueue)
            filter = "where ip.IP_Queue = 1";

        IPList ipList = new IPList();
        dynamic data = await ipList.SelectFromSql($"select * from IPList ip {filter}");
        var jsonData = JsonConvert.SerializeObject(data);
        List<IPList> ipLists = JsonConvert.DeserializeObject<List<IPList>>(jsonData);
        return ipLists;
    }
}