using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using ProductivityPlanner.Desktop.Models;

namespace ProductivityPlanner.Desktop
{
    public partial class DashboardWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseApiUrl = "https://localhost:7269/api/tasks";
        private readonly int _userId;
        private List<TaskModel> _allTasks = new List<TaskModel>();

        public DashboardWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTasks();
        }

        private async Task LoadTasks()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/user/{_userId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    _allTasks = JsonConvert.DeserializeObject<List<TaskModel>>(json) ?? new List<TaskModel>();
                    GridTasks.ItemsSource = _allTasks;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd pobierania zadań: " + ex.Message);
            }
        }

        private async void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNewTitle.Text))
            {
                MessageBox.Show("Tytuł zadania nie może być pusty!", "Walidacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newTask = new TaskModel
            {
                Title = TxtNewTitle.Text,
                Category = TxtNewCategory.Text,
                Priority = (TaskPriority)CmbNewPriority.SelectedIndex,
                Status = Models.TaskStatus.New,
                UserId = _userId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var json = JsonConvert.SerializeObject(newTask);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    TxtNewTitle.Clear();
                    await LoadTasks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message);
            }
        }

        private async void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is TaskModel selectedTask)
            {
                selectedTask.Status = Models.TaskStatus.Completed;

                try
                {
                    var json = JsonConvert.SerializeObject(selectedTask);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"{_baseApiUrl}/{selectedTask.Id}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        await LoadTasks();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd edycji: " + ex.Message);
                }
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is TaskModel selectedTask)
            {
                var result = MessageBox.Show($"Czy na pewno usunąć zadanie: {selectedTask.Title}?", "Potwierdzenie", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/{selectedTask.Id}");
                        if (response.IsSuccessStatusCode)
                        {
                            await LoadTasks();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd usuwania: " + ex.Message);
                    }
                }
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var searchText = TxtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                GridTasks.ItemsSource = _allTasks;
            }
            else
            {
                GridTasks.ItemsSource = _allTasks.Where(t => t.Title.ToLower().Contains(searchText)).ToList();
            }
        }

        private void BtnExportCsv_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Pliki CSV (*.csv)|*.csv",
                FileName = "RaportZadan.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                StringBuilder csvContent = new StringBuilder();
                csvContent.AppendLine("ID;Tytul;Kategoria;Priorytet;Status");

                foreach (var task in _allTasks)
                {
                    csvContent.AppendLine($"{task.Id};{task.Title};{task.Category};{task.Priority};{task.Status}");
                }

                File.WriteAllText(saveFileDialog.FileName, csvContent.ToString(), Encoding.UTF8);
                MessageBox.Show("Pomyślnie wygenerowano raport CSV!", "Raport", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}