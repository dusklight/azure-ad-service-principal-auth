using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string tenant = "[Your Azure Active Directory TenantID]";
            string aadInstance = "https://login.microsoftonline.com/";
            string authority = aadInstance + tenant;

            var authContext = new AuthenticationContext(authority);

            // This must match the "Audiences" setup on the API/server application side.  See Startup.cs on the ServerDemo project.
			string resourceId = "[Your API/Server Application's ID or the URI Setup in Azure]";

            var clientCredential = new ClientCredential("[Your Application ID]", "[Your Application Key (Password)]");

            // Authenticate with Azure Active Directory and retrieve the Access Token (JWT).
            AuthenticationResult result = await authContext.AcquireTokenAsync(resourceId, clientCredential);

            Console.WriteLine($"Access Token (JWT): {result.AccessToken}");

            // Call the API
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            string apiUrl = "http://[Your Domain Name For API]/api/ServerDateTime";

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Success!");
                Console.WriteLine("Content: " + await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }

            Console.ReadLine();
        }
    }
}
