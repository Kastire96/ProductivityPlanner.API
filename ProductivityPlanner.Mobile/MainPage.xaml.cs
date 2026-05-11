using ProductivityPlanner.Mobile.Services;

namespace ProductivityPlanner.Mobile
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiService _apiService;

        public MainPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            ErrorLabel.IsVisible = false;

            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ErrorLabel.Text = "Wprowadź e-mail oraz hasło!";
                ErrorLabel.IsVisible = true;
                return;
            }

            var user = await _apiService.LoginAsync(email, password);

            if (user != null)
            {
                Preferences.Default.Set("LoggedUserId", user.Id);
                Application.Current.MainPage = new NavigationPage(new TasksPage());
            }
            else
            {
                ErrorLabel.Text = "Błędny e-mail lub hasło!";
                ErrorLabel.IsVisible = true;
            }
        }
    }
}