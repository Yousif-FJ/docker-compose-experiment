using System.Net;
using System.Text;
using TUnit.Core.Logging;

namespace tests;

//Test which are effected or effect system state
public class StatefulSystemTests
{
    private readonly string serviceHost = Environment.GetEnvironmentVariable("CICD_APP_SERVICE_HOSTNAME") ?? "localhost";
    private readonly DefaultLogger logger = new();
    [Test]
    public async Task PostStateInit_ToApi_ShouldReturnOkay()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.PutAsync("/state", new StringContent("INIT"));

        logger.LogInformation($"Response: {response.StatusCode}");

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
    }


    [Test]
    [DependsOn(nameof(PostStateInit_ToApi_ShouldReturnOkay))]
    public async Task GetState_ToApi_ShouldReturnOkayInitial_WhenSystemInInitState()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.GetAsync("/state");

        logger.LogInformation($"Response: {response.StatusCode}");

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
        Assert.Equals(await response.Content.ReadAsStringAsync(), "INIT");
    }

    [Test]
    [DependsOn(nameof(GetState_ToApi_ShouldReturnOkayInitial_WhenSystemInInitState))]
    public async Task PostLogin_ToMainPagePort_ShouldReturnOkay()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8198/")
        };

        var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:admin"));
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthString);

        var response = await httpClient.PostAsync("/login", null);

        logger.LogInformation($"Response: {response.StatusCode}");

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
    }

    [Test]
    [DependsOn(nameof(PostLogin_ToMainPagePort_ShouldReturnOkay))]
    public async Task GetRequest_ToApi_ShouldReturnOkay_WhenSystemIsRunning()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.GetAsync("/request");

        logger.LogInformation($"Response: {response.StatusCode}");

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
    }


    [Test]
    [DependsOn(nameof(GetRequest_ToApi_ShouldReturnOkay_WhenSystemIsRunning))]
    public async Task PostStatePaused_ToApi_ShouldReturnOkay()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.PutAsync("/state", new StringContent("PAUSED"));

        logger.LogInformation($"Response: {response.StatusCode}");
        logger.LogInformation($"Body: { await response.Content.ReadAsStringAsync()}");

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
    }

    [Test]
    [DependsOn(nameof(PostStatePaused_ToApi_ShouldReturnOkay))]
    public async Task GetRequest_ToApi_ShouldReturn503_WhenSystemIsPaused()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.GetAsync("/request");

        logger.LogInformation($"Response: {response.StatusCode}");

        Assert.Equals(response.StatusCode, HttpStatusCode.ServiceUnavailable);
    }   
}