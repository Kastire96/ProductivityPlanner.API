using ProductivityPlanner.Mobile.Services;

namespace ProductivityPlanner.Mobile
{
    public partial class TasksPage : ContentPage
    {
        private readonly ApiService _apiService;

        public TasksPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTasksAsync();
        }

        private async Task LoadTasksAsync()
        {
            var tasks = await _apiService.GetTasksAsync();
            TasksCollectionView.ItemsSource = tasks;
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Default.Remove("LoggedUserId");
            Application.Current.MainPage = new MainPage();
        }
    }
}