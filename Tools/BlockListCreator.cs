namespace IPv64IPScanner;

public class BlockListCreator
{
    public static async Task Create()
    {
        IPList ip = new IPList();

        List<IPList> ipList = await ip.LoadAll(true);

        bool isCreated = await CreateFile();

        if (isCreated)
            await WriteLine(ipList);
    }

    private static async Task<bool> CreateFile()
    {
        //Create Database File
        if (IsExists) File.Delete(GetFilePath);
        Directory.CreateDirectory(Path.GetDirectoryName(GetFilePath)!);
        await File.WriteAllTextAsync(GetFilePath, "");

        return true;
    }

    private static async Task WriteLine(List<IPList> ipList)
    {
        using (var writer = new StreamWriter(GetFilePath, append: true))
            foreach (var ip in ipList)
                await writer.WriteLineAsync(ip.IP_Address);
    }

    public static string GetFilePath
    {
        get
        {
            var filePath = Path.Combine($"{GetLocationsOf.App}/data", "IPv64Blocklist_extended.txt");
            return filePath;
        }
    }

    public static bool IsExists
    {
        get
        {
            return File.Exists(GetFilePath);
        }
    }

    public static class GetLocationsOf
    {
        public static readonly string? App =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}