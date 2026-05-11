using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using ProductivityPlanner.Desktop.Models;

namespace ProductivityPlanner.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        // Zmień port na ten, który wyświetla Twoje uruchomione API!
        private readonly string _apiUrl = "https://localhost:7269/api/auth/login";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            LblError.Visibility = Visibility.Collapsed;

            var loginData = new
            {
                Email = TxtEmail.Text,
                Password = TxtPassword.Password
            };

            if (string.IsNullOrEmpty(loginData.Email) || string.IsNullOrEmpty(loginData.Password))
            {
                ShowError("Uzupełnij wszystkie pola.");
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserModel>(responseString);

                    if (user != null)
                    {
                        DashboardWindow dashboard = new DashboardWindow(user.Id);
                        dashboard.Show();
                        this.Close();
                    }
                }
                else
                {
                    ShowError("Nieprawidłowy email lub hasło.");
                }
            }
            catch (Exception)
            {
                ShowError("Brak połączenia z API. Upewnij się, że projekt API działa!");
            }
        }

        private void ShowError(string message)
        {
            LblError.Text = message;
            LblError.Visibility = Visibility.Visible;
        }
    }
}