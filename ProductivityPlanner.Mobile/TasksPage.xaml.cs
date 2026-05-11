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

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            string title = NewTaskEntry.Text?.Trim();

            if (string.IsNullOrEmpty(title))
            {
                await DisplayAlert("Błąd", "Wpisz treść zadania!", "OK");
                return;
            }

            var success = await _apiService.AddTaskAsync(title);

            if (success)
            {
                NewTaskEntry.Text = string.Empty;
                await LoadTasksAsync();
            }
            else
            {
                await DisplayAlert("Błąd", "Nie udało się dodać zadania.", "OK");
            }
        }

        private async void OnCompleteTaskClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var task = (TaskModel)button.CommandParameter;

            if (task != null)
            {
                var success = await _apiService.DeleteTaskAsync(task.Id);

                if (success)
                {
                    await LoadTasksAsync();
                }
                else
                {
                    await DisplayAlert("Błąd", "Nie udało się zakończyć zadania.", "OK");
                }
            }
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Default.Remove("LoggedUserId");
            Application.Current.MainPage = new MainPage();
        }
    }
}