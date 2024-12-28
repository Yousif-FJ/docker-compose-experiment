namespace tests;

using System.Text;

[TestClass]
public class ApiTests
{
    public TestContext TestContext {get; set; } = default!;

    [TestMethod]
    public async Task MakeRequestToRestApi_ShouldReturnOkay()
    {
        var httpClient = new HttpClient(){
            BaseAddress = new Uri("http://docker:8198/")
        };
        var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:admin"));
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthString);
        
        var response = await httpClient.GetAsync("/");

        TestContext.WriteLine($"Response: {response.StatusCode}");

        Assert.IsTrue(response.IsSuccessStatusCode);
    }
}