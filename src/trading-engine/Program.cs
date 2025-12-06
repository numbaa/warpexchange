using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace trading_engine;

public class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<AssetService>();
    }
}
