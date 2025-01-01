using System.Net;
using System.Text;

namespace tests;

[TestClass]
//Test which are effected or effect system state
public class StatefulSystemTests
{
    public TestContext TestContext { get; set; } = default!;
    private readonly string serviceHost = Environment.GetEnvironmentVariable("CICD_APP_SERVICE_HOSTNAME") ?? "localhost";

    [TestMethod]
    [TestProperty("ExecutionOrder", "1")]
    public async Task PostStateInit_ToApi_ShouldReturnOkay()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.PutAsync("/state", new StringContent("INIT"));

        TestContext.WriteLine($"Response: {response.StatusCode}");

        Assert.IsTrue(response.IsSuccessStatusCode);
    }


    [TestMethod]
    [TestProperty("ExecutionOrder", "2")]
    public async Task GetState_ToApi_ShouldReturnOkayInitial_WhenSystemInInitState()
    {
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
    [TestProperty("ExecutionOrder", "3")]
    public async Task PostLogin_ToMainPagePort_ShouldReturnOkay()
    {
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

    [TestMethod]
    [TestProperty("ExecutionOrder", "4")]
    public async Task GetRequest_ToApi_ShouldReturnOkay_WhenSystemIsRunning()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.GetAsync("/request");

        TestContext.WriteLine($"Response: {response.StatusCode}");

        Assert.IsTrue(response.IsSuccessStatusCode);
    }

}