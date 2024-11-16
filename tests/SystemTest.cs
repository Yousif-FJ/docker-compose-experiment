namespace tests;

using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Common;
using Ductus.FluentDocker.Services;

[TestClass]
public class SystemTest
{
    private static ICompositeService composeAdapter = default!;

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        var rootPath = Directory.GetCurrentDirectory().Split("tests")[0];

        var composeFilePath = Path.Combine(rootPath, "docker-compose.yml");

        composeAdapter = new Builder()
                            .UseContainer()
                            .UseCompose()
                            .FromFile(composeFilePath)
                            .ServiceName("devops-automated-system-test")
                            .RemoveOrphans()
                            .Build()
                            .Start();

        var semaphore = new SemaphoreSlim(1);

        composeAdapter.StateChange += (obj, eventArgs) =>{
            switch (eventArgs.State)
            {
                case ServiceRunningState.Running:
                    testContext.WriteLine("Services are running");
                    semaphore.Release();
                    break;

                case ServiceRunningState.Stopped:
                    semaphore.Release();
                    break;
                default:
                    break;
            }
        };
        
        testContext.WriteLine("Waiting for our services to start");
        semaphore.Wait(TimeSpan.FromMinutes(3));
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        composeAdapter.Stop();
        composeAdapter.Remove();
    }
}