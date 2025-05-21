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
                var result = await mxToolbox.CheckIp(ip.IP_Address);
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

                ip.IP_Changed = DateTime.UtcNow;
                ip.IP_Queue = false;
                await ip.Update();
                Thread.Sleep(1000);
            }

            await BlockListCreator.Create();
        });
    }
}