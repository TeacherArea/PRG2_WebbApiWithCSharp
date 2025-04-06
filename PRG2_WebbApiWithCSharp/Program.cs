using System.Net.Http.Json;

namespace MyApp
{
    internal class Program
    {
        static string url = "http://localhost:2051/lbs/c-api/users.php";
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }
        static async Task Main(string[] args)
        {
            await GetUsers();
            Console.WriteLine("- - - - - - - - - ");
            await PostNewUser();
            await GetUsers();
            Console.WriteLine("- - - - - - - - - ");
            //await UpdateUser();
            //await GetUsers();
            Console.WriteLine("- - - - - - - - - ");
            //await DeleteUser();
            await GetUsers();
        }

        private static async void ShowUsers(List<User>? usersFromApi)
        {
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
                var usersFromApi = await client.GetFromJsonAsync<List<User>>(url);
                ShowUsers(usersFromApi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Något blev fel vidAPI-anrop: {ex.Message}");
            }
        }

        private static async Task PostNewUser()
        {
            var newUser = new User { Id = 3, Name = "Pelle", Age = 17 };
            var client = new HttpClient();
            string addedUrl = $"{url}?Id=3";
            await client.PostAsJsonAsync(addedUrl, newUser );
        }

        /* Hur det borde se ut:
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
            string updatedUrl = $"?_method=PUT&id=2\\";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, updatedUrl)
            {
                Content = JsonContent.Create(updatedUser)
            };
            await client.SendAsync(request);
        }

        /* Hur det borde se ut:
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
            // Console.WriteLine($"DELETE status: {response.StatusCode}");
        }
    }
}