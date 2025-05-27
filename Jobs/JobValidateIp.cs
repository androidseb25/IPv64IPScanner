using System.Net;
using System.Numerics;
using System.Security.Cryptography;

namespace IPv64IPScanner;

public class JobValidateIp
{
    public async Task Execute()
    {
        List<IPList> ipAllLists = await new IPList().LoadAll(false, true);
        int range = ipAllLists.Count < Env.IP_TASK_COUNT ? ipAllLists.Count : Env.IP_TASK_COUNT;
        List<IPList> ipLists = ipAllLists.GetRange(0, range);

        MxToolbox mxToolbox = new MxToolbox();
        await mxToolbox.GetApiKey();

        await Task.Run(async () =>
        {
            foreach (IPList ip in ipLists)
            {
                List<IPAddress> ipList = GenerateIPs(ip.IP_Address, 10);

                foreach (IPAddress ipaddr in ipList)
                {
                    //Skip other IP's because one found in the blocklists
                    if (ip.IP_Blocked && ip.IP_ExtendedInfos.Length > 0)
                        continue;
                    
                    var result = await mxToolbox.CheckIp(ipaddr.ToString());
                    if (result != null)
                    {
                        if (result.ListedBlacklists.Count > 0)
                        {
                            ip.IP_Blocked = true;
                            string blocklists = "";

                            foreach (var block in result.ListedBlacklists)
                            {
                                var find = result.ResultDS.SubActions.Find(x => x.SubActionID == block.ToString());
                                if (find != null)
                                    if (blocklists.Length > 0)
                                        blocklists += $"; {find.Name}";
                                    else
                                        blocklists = $"{find.Name}";
                            }

                            ip.IP_ExtendedInfos = blocklists;
                        }
                    }
                    Thread.Sleep(1000);
                }

                ip.IP_Changed = DateTime.UtcNow;
                ip.IP_Queue = false;
                await ip.Update();
            }

            await BlockListCreator.Create();
        });
    }
    
    public static List<IPAddress> GenerateIPs(string ipaddress, int count)
    {
        var parts = ipaddress.Split('/');
        var baseIp = IPAddress.Parse(parts[0]);
        int prefix = int.Parse(parts[1]);

        if (baseIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            return GenerateRandomIPv4(baseIp, prefix, count);
        else
            return GenerateRandomIPv6(baseIp, prefix, count);
    }
    
    public static List<IPAddress> GenerateRandomIPv4(IPAddress baseIp, int prefix, int count)
    {
        uint baseInt = IpToUInt(baseIp);
        uint mask = ~(uint.MaxValue >> prefix);
        uint network = baseInt & mask;
        uint broadcast = network | ~mask;
        uint total = broadcast - network + 1;

        if (prefix == 32) return new List<IPAddress> { baseIp };

        var random = new Random();
        var results = new HashSet<IPAddress>();

        while (results.Count < count && results.Count < total - 2)
        {
            uint offset = (uint)random.Next(1, (int)(total - 1)); // skip .0 and .broadcast
            results.Add(UIntToIp(network + offset));
        }

        return results.ToList();
    }

    public static List<IPAddress> GenerateRandomIPv6(IPAddress baseIp, int prefix, int count)
    {
        var baseBytes = baseIp.GetAddressBytes().Reverse().ToArray();
        BigInteger baseInt = new BigInteger(baseBytes);
        BigInteger total = BigInteger.One << (128 - prefix);

        var results = new HashSet<IPAddress>();

        using var rng = RandomNumberGenerator.Create();

        while (results.Count < count && total > 0)
        {
            byte[] offsetBytes = new byte[16];
            rng.GetBytes(offsetBytes);
            offsetBytes[0] &= (byte)(0xFF >> (prefix % 8)); // apply host bits mask

            BigInteger offset = new BigInteger(offsetBytes);
            if (offset < 0) offset = BigInteger.Negate(offset);
            offset %= total;

            BigInteger candidate = baseInt + offset;
            byte[] candidateBytes = candidate.ToByteArray();

            // ensure 16 bytes
            if (candidateBytes.Length < 16)
                candidateBytes = candidateBytes.Concat(new byte[16 - candidateBytes.Length]).ToArray();

            results.Add(new IPAddress(candidateBytes.Take(16).Reverse().ToArray()));
        }

        return results.ToList();
    }

    public static uint IpToUInt(IPAddress ip)
    {
        byte[] bytes = ip.GetAddressBytes();
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    public static IPAddress UIntToIp(uint val)
    {
        byte[] bytes = BitConverter.GetBytes(val);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return new IPAddress(bytes);
    }
}