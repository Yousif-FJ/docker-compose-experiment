using System.Net;
using System.Text;

namespace tests;

[TestClass]
//Tests which always return the same results regardless of the system state
public class StatelessSystemTest
{
    public TestContext TestContext { get; set; } = default!;

    [TestMethod]
    public async Task PostLoginRequest_ToApiPort_ShouldReturnForbidden()
    {
        var serviceHost = Environment.GetEnvironmentVariable("CICD_APP_SERVICE_HOSTNAME") ?? "localhost";

        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8197/")
        };

        var response = await httpClient.PostAsync("/login", null);

        TestContext.WriteLine($"Response: {response.StatusCode}");

        Assert.IsTrue(response.StatusCode == HttpStatusCode.Forbidden);
    }

    [TestMethod]
    public async Task GetRequestWithAuthentication_ToRootPage_ShouldReturnOkay()
    {
        var serviceHost = Environment.GetEnvironmentVariable("CICD_APP_SERVICE_HOSTNAME") ?? "localhost";

        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri($"http://{serviceHost}:8198/")
        };
        var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:admin"));
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthString);

        var response = await httpClient.GetAsync("/");

        TestContext.WriteLine($"Response: {response.StatusCode}");

        Assert.IsTrue(response.IsSuccessStatusCode);
    }
}