using System.Net;
using System.Text;

namespace tests;

//Tests which always return the same results regardless of the system state
public class StatelessSystemTest
{
    private readonly string serviceHost = Environment.GetEnvironmentVariable("CICD_APP_SERVICE_HOSTNAME") ?? "localhost";

    [Test]
    public async Task PostLoginRequest_ToApiPort_ShouldReturnForbidden()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.PostAsync("/login", null);

        Console.WriteLine($"Response: {response.StatusCode}");

        await Assert.That(response.StatusCode == HttpStatusCode.Forbidden).IsTrue();
    }

    [Test]
    public async Task GetRequestWithAuthentication_ToRootPage_ShouldReturnOkay()
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8198/")
        };
        var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:admin"));
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthString);

        var response = await httpClient.GetAsync("/");

        Console.WriteLine($"Response: {response.StatusCode}");

        await Assert.That(response.IsSuccessStatusCode).IsTrue();
    }
}