using System.Net.Http.Json;
using System.Text.Json;

namespace PRG2_WebbApiWithCSharp
{
    internal class Program
    {
        static string url = "http://localhost:2051/c-api/users.php";
        static async Task Main(string[] args)
        {
            await GetUsers();
            Console.WriteLine("- - - - - - - - - ");
            await PostNewUser();
            await GetUsers();
            Console.WriteLine("- - - - - - - - - ");
            await UpdateUser();
            await GetUsers();
            Console.WriteLine("- - - - - - - - - ");
            await DeleteUser();
            await GetUsers();
        }
        private static async void ShowUsers(List<User>? usersFromApi)
        {
            if(usersFromApi == null)
            {
                Console.WriteLine("Responsen från api:et var null: " + usersFromApi);
                return;
            }
            foreach (var user in usersFromApi)
            {
                Console.WriteLine($"{user.Id}: {user.Name} {user.Age}");
            }
        }

        private static async Task GetUsers()
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Respons från API: " + content);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var usersFromApi = JsonSerializer.Deserialize<List<User>>(content, options);

                ShowUsers(usersFromApi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid API-anrop (GET): {ex.Message}");
            }
        }


        private static async Task PostNewUser()
        {
            var newUser = new User { Id = 3, Name = "Oscar", Age = 18 };
            var client = new HttpClient();
            string addedUrl = $"{url}?Id=3";
            await client.PostAsJsonAsync(addedUrl, newUser );
        }

        /* Hur det ungefär ser ut om inte XAMPP används:
        private static async Task UpdateUser()
        {
            var updatedUser = new User { Id = 2, Name = "Anna", Age = 23 };
            string addedUrl = $"{url}?Id=2";
            var client = new HttpClient();
            await client.PutAsJsonAsync(addedUrl, updatedUser);
        }
        */
        private static async Task UpdateUser()
        {
            var updatedUser = new User { Name = "Anna Andersson", Age = 23 };
            string updatedUrl = $"{url}?_method=PUT&id=2";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, updatedUrl)
            {
                Content = JsonContent.Create(updatedUser)
            };
            await client.SendAsync(request);
        }

        /* Hur det ungefär ser ut om inte XAMPP används:
        private static async Task DeleteUser()
        {
            string addedUrl = $"{url}?Id=1";
            var client = new HttpClient();
            await client.DeleteAsync(addedUrl);
        }
        */

        private static async Task DeleteUser()
        {
            var client = new HttpClient();
            string deleteUrl = $"{url}?_method=DELETE&id=1";

            var request = new HttpRequestMessage(HttpMethod.Post, deleteUrl);

            var response = await client.SendAsync(request);
            Console.WriteLine($"DELETE status: {response.StatusCode}");
        }
    }
}