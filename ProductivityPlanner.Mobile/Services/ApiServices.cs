using System.Net.Http.Json;

namespace ProductivityPlanner.Mobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://10.0.2.2:7269/api";

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
        }

        public async Task<UserModel> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/Auth/login", new { email, password });
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserModel>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd połączenia z API: {ex.Message}");
            }
            return null;
        }

        public async Task<List<TaskModel>> GetTasksAsync()
        {
            try
            {
                int userId = Preferences.Default.Get("LoggedUserId", 0);
                if (userId == 0) return new List<TaskModel>();

                var response = await _httpClient.GetAsync($"{_apiUrl}/Tasks/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TaskModel>>() ?? new List<TaskModel>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd pobierania zadań: {ex.Message}");
            }
            return new List<TaskModel>();
        }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public bool IsCompleted { get; set; }
        public int UserId { get; set; }
    }
}