namespace IPv64IPScanner;

public class BlockListCreator
{
    public static async Task Create()
    {
        IPList ip = new IPList();
        
        List<BlockMethods> methods = Enum.GetValues(typeof(BlockMethods)).Cast<BlockMethods>().ToList();

        foreach (var method in methods)
        {
            List<IPList> ipList = await ip.LoadAll(true, false, (int)method);

            bool isCreated = await CreateFile((int)method);

            if (isCreated)
                await WriteLine(ipList, (int)method);
        }
    }

    private static async Task<bool> CreateFile(int method = -1)
    {
        //Create Database File
        if (IsExists(method)) File.Delete(GetFilePath(method));
        Directory.CreateDirectory(Path.GetDirectoryName(GetFilePath(method))!);
        await File.WriteAllTextAsync(GetFilePath(method), "");

        return true;
    }

    private static async Task WriteLine(List<IPList> ipList, int method = -1)
    {
        using (var writer = new StreamWriter(GetFilePath(method), append: true))
            foreach (var ip in ipList)
                await writer.WriteLineAsync(ip.IP_Address);
    }

    public static string GetFilePath(int method = -1)
    {
        string file = "";
        var filePath = Path.Combine($"{GetLocationsOf.App}/data", $"{GetMethod(method)}");
        return filePath;
    }

    public static string GetMethod(int method = -1)
    {
        switch (method)
        {
            case (int)BlockMethods.SSH:
                return "IPv64Blocklist_ssh.txt";
            case (int)BlockMethods.HTTP_S:
                return "IPv64Blocklist_http_s.txt";
            case (int)BlockMethods.Mail:
                return "IPv64Blocklist_mail.txt";
            case (int)BlockMethods.FTP:
                return "IPv64Blocklist_ftp.txt";
            case (int)BlockMethods.ICMP:
                return "IPv64Blocklist_icmp.txt";
            case (int)BlockMethods.DoS:
                return "IPv64Blocklist_dos.txt";
            case (int)BlockMethods.DDoS:
                return "IPv64Blocklist_ddos.txt";
            case (int)BlockMethods.Flooding:
                return "IPv64Blocklist_flooding.txt";
            case (int)BlockMethods.Web:
                return "IPv64Blocklist_web.txt";
            case (int)BlockMethods.Malware:
                return "IPv64Blocklist_maleware.txt";
            case (int)BlockMethods.Bots:
                return "IPv64Blocklist_bots.txt";
            case (int)BlockMethods.TCP:
                return "IPv64Blocklist_tcp.txt";
            case (int)BlockMethods.UDP:
                return "IPv64Blocklist_udp.txt";
            case (int)BlockMethods.GeoIP:
                return "IPv64Blocklist_geoip.txt";
            case (int)BlockMethods.VPN:
                return "IPv64Blocklist_vpn.txt";
            default:
                return "IPv64Blocklist.txt";
        }
    }

    public static bool IsExists(int method = -1)
    {
        return File.Exists(GetFilePath(method));
    }

    public static class GetLocationsOf
    {
        public static readonly string? App =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}