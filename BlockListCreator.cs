namespace IPv64IPScanner;

public class BlockListCreator
{
    public static async Task Create()
    {
        IPList ip = new IPList();
        
        List<IPList> ipList = await ip.LoadAll(true);

        bool isCreated = await CreateFile($"{GetLocationsOf.App}/data");

        if (isCreated)
            await WriteLine($"{GetLocationsOf.App}/data", ipList);
    }
    
    private static async Task<bool> CreateFile(string path)
    {
        //Create Database File
        var filePath = Path.Combine(path, "IPv64Blocklist_extended.txt");
        var isFileExists = File.Exists(filePath);

        if (isFileExists) File.Delete(filePath);
        
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await File.WriteAllTextAsync(filePath, "");
        
        return true;
    }

    private static async Task WriteLine(string path, List<IPList> ipList)
    {
        var filePath = Path.Combine(path, "IPv64Blocklist_extended.txt");
        
        using (var writer = new StreamWriter(filePath, append: true))
            foreach (var ip in ipList)
                await writer.WriteLineAsync(ip.IP_Address);
    }
    
    public static class GetLocationsOf
    {
        public static readonly string? App =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}