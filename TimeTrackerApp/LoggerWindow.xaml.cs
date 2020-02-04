using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HrApp;
using HrApp.Services;
using HrApp.Domain;
using HrApp.Entities;
using HrApp.Repositories;
using System.Threading.Tasks;
using TimeTrackerApp.Extra;

namespace TimeTrackerApp
{
    /// <summary>
    /// Interaction logic for LoggerWindow.xaml
    /// </summary>
    public partial class LoggerWindow : Window
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        List<TimeInfo> DataGridList = new List<TimeInfo>();

        EmployeeEntity Employee;
        List<Commit> Commits = new List<Commit>();
        List<ProjectEntity> AllProjects = new List<ProjectEntity>();
        List<ProjectEntity> LoggedProjects = new List<ProjectEntity>(); 

        DateTime StartTime;
        TimeSpan elapsed;
        int StartClicks = 1;
        string message = "";

        public LoggerWindow()
        {
            InitializeComponent();
            SetUp();
        }

        private async void SetUp()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;

            Employee = await GetEmployee("5e20785d2bd93500011dbf6f");
            //string username = (string)System.Windows.Application.Current.Resources["username"];

            string name = "Loged in as: " + Employee.FirstName + " " + Employee.LastName;
            UsernameLabel.Content = name;

            AllProjects = await GetProjects();
            List<string> NamesList = new List<string>();
            foreach (var pro in AllProjects)
            {
                NamesList.Add(pro.Name);
            }
            DropdownList.ItemsSource = NamesList;
            DropdownList.SelectedIndex = 0;
        }

        /// <summary>
        /// Sign out the current user
        /// </summary>
        private async void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            var accounts = await App.PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await App.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());

                    MainWindow mainWindow = new MainWindow();
                    //mainWindow.Show();
                    message = "User has signed-out";
                    this.Close();
                }
                catch (MsalException ex)
                {
                    ErrorLabel.Visibility = Visibility.Visible;
                    ErrorLabel.Content = $"Error signing-out user: {ex.Message}";
                }
            }
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartClicks % 2 != 0)
            {
                timer.Start();
                DropdownList.IsEnabled = false;
                CommitButton.IsEnabled = false;
                FromToButton.IsEnabled = false;
            }
            else
            {
                timer.Stop();
                string description;
                if (DescriptionTextBox.Text == "")
                {
                    DisplayError("Description field is required!");
                    return;
                }
                else
                    description = DescriptionTextBox.Text;

                CommitButton.IsEnabled = true;
                FromToButton.IsEnabled = true;
                DropdownList.IsEnabled = true;
                ErrorLabel.Visibility = Visibility.Collapsed;
                ClockLabel.Content = "00:00:00";

                var projectId = DropdownList.SelectedIndex;

                Commit commit = new Commit(Employee, description, elapsed.TotalHours);
                Commits.Add(commit);

                ProjectEntity entity = AllProjects[projectId];
                LoggedProjects.Add(entity);

                DataGridList.Add(new TimeInfo(AllProjects[projectId].Name, elapsed, description));

                DataGrid.ItemsSource = null;
                DataGrid.ItemsSource = DataGridList;
                DataGrid.Visibility = Visibility.Visible;
                DescriptionTextBox.Text = "";
            }

            StartButton.Content = timer.IsEnabled ? "Stop" : "Start";
            StartTime = DateTime.Now;
            StartClicks++;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            elapsed = DateTime.Now - StartTime;

            string text = "";

            text += elapsed.Hours.ToString("00") + ":" +
                    elapsed.Minutes.ToString("00") + ":" +
                    elapsed.Seconds.ToString("00");

            ClockLabel.Content = text;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.ResultText.Text = message;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            LoggMultiple();
            DataGrid.ItemsSource = null;
            DataGridList = new List<TimeInfo>();
            LoggedProjects = new List<ProjectEntity>();
            Commits = new List<Commit>();
        }
        private void FromToButton_Click(object sender, RoutedEventArgs e)
        {
            FromToWindow LogSingle = new FromToWindow();
            LogSingle.ShowDialog();
        }
        private async void LoggMultiple()
        {
            TimeTrackerService service = new TimeTrackerService()
            {
                CommitRepository = new CommitRepository(),
                ProjectRepository = new ProjectRepository(),
                EmployeeRepository = new EmployeesRepository()
            };

            await service.LogHours(LoggedProjects, Commits);
        }
        private async Task<List<ProjectEntity>> GetProjects()
        {
            ProjectRepository projectRepository = new ProjectRepository();
            var projects = await projectRepository.GetAllProjects();
            return projects;
        }
        private async Task<EmployeeEntity> GetEmployee(string id)
        {
            EmployeesRepository repo = new EmployeesRepository();
            var emp = await repo.GetEmployeeById(id);
            return emp;
        }
        private void DisplayError(string message)
        {
            ErrorLabel.Visibility = Visibility.Visible;
            ErrorLabel.Content = message;
        }
    }
}