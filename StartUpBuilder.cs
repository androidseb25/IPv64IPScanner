namespace IPv64IPScanner;

public class StartUpBuilder : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                DatabaseCreator dbCreator = new DatabaseCreator();
                dbCreator.Init();
            });
            next(builder);
        };
    }
}