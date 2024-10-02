using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Products.Interfaces;

[assembly: FunctionsStartup(typeof(PDV.ElectronicBill.Function.Startup))]

namespace PDV.ElectronicBill.Function;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddHttpClient();
        builder.Services.AddLogging();

        builder.Services.AddSingleton<IElectronicBill, Domain.ElectronicBill.ElectronicBill>();
    }
}

