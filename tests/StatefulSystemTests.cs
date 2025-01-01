using System.Net;
using System.Text;

namespace tests;

[TestClass]
//Test which are effected or effect system state
public class StatefulSystemTests
{
    public TestContext TestContext { get; set; } = default!;


    [TestMethod]
    public async Task GetState_ToApi_ShouldReturnOkayInitial_WhenSystemInInitState()
    {
        var serviceHost = Environment.GetEnvironmentVariable("CICD_APP_SERVICE_HOSTNAME") ?? "localhost";

        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.GetAsync("/state");

        TestContext.WriteLine($"Response: {response.StatusCode}");

        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.IsTrue((await response.Content.ReadAsStringAsync()) == "INIT");
    }

    [TestMethod]
    public async Task GetRequest_ToApi_ShouldReturnOkay_WhenSystemIsRunning()
    {
        var serviceHost = Environment.GetEnvironmentVariable("CICD_APP_SERVICE_HOSTNAME") ?? "localhost";

        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.GetAsync("/request");

        TestContext.WriteLine($"Response: {response.StatusCode}");

        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public async Task PostLogin_ToMainPagePort_ShouldReturnOkay()
    {
        var serviceHost = Environment.GetEnvironmentVariable("CICD_APP_SERVICE_HOSTNAME") ?? "localhost";

        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8198/")
        };

        var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:admin"));
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthString);

        var response = await httpClient.PostAsync("/login", null);

        TestContext.WriteLine($"Response: {response.StatusCode}");

        Assert.IsTrue(response.IsSuccessStatusCode);
    }
}