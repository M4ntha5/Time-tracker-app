using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using HrApp.Services;
using HrApp;
using HrApp.Domain;
using HrApp.Entities;
using HrApp.Repositories;
using TimmeTrackerApp;
using TimmeTrackerApp.Extra;

namespace TimeTrackerApp
{
    /// <summary>
    /// Interaction logic for TrackerWindow.xaml
    /// </summary>
    public partial class TrackerWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        List<Commit> commits = new List<Commit>();
        EmployeeEntity employee;
        List<ProjectEntity> projects = new List<ProjectEntity>();
        List<TimeInfo> list = new List<TimeInfo>();

        private DateTime StartTime;
        TimeSpan elapsed;
        int StartClicks = 1;
        string message = "";

        public TrackerWindow()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            InitializeComponent();

            string username = (string)System.Windows.Application.Current.Resources["username"];
            string name = "Loged in as: " + username;
            UsernameLabel.Content = name;

            employee = new EmployeeEntity() { FirstName = name, Id = "1" };


            var projects = getProjects();
            List<string> list = new List<string>();
            foreach(var pro in projects)
            {
                list.Add(pro.Name);
            }

            DropdownList.ItemsSource = list;
            DropdownList.SelectedIndex = 0;

        }

        private List<ProjectEntity> getProjects()
        {
           /* ProjectRepository repo = new ProjectRepository();
            try
            {
                var pros = await repo.GetProjectByName("new1");

            }
            catch(Exception ex)
            {
                var a = ex.Message;
            }*/


            List<ProjectEntity> entitys = new List<ProjectEntity>();

            var pro1 = new ProjectEntity { Name = "pro1" };
            var pro2 = new ProjectEntity { Name = "pro2" };
            var pro3 = new ProjectEntity { Name = "pro3" };
            var pro4 = new ProjectEntity { Name = "pro4" };
            entitys.Add(pro1); entitys.Add(pro2);
            entitys.Add(pro3); entitys.Add(pro4);
            return entitys;
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
            if(StartClicks % 2 != 0)
            {
                timer.Start();
                DropdownList.IsEnabled = false;
            }                     
            else
            {        
                timer.Stop();
                ClockLabel.Content = "00:00:00";
                DropdownList.IsEnabled = true;
                var projectName = DropdownList.SelectedItem.ToString();
                string description = string.Empty;
                if (DescriptionTextBox.Text == "")
                    description = "Worked on " + projectName;
                else
                    description = DescriptionTextBox.Text;

                Commit commit = new Commit(employee, description, elapsed.TotalHours);
                commits.Add(commit);

                
                ProjectEntity entity = new ProjectEntity() { Name = projectName };
                projects.Add(entity);
                              
                list.Add(new TimeInfo(projectName, elapsed, description));
                var source = new BindingSource(list, null);

                DataGrid.ItemsSource = source;
                DataGrid.Visibility = Visibility.Visible;
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
        private async void Logg()
        {
            TimeTrackerService service = new TimeTrackerService();
            await service.LogHours(projects, commits);
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            //Logg();
            DataGrid.ItemsSource = null;

        }

        private void FromToButton_Click(object sender, RoutedEventArgs e)
        {
            FromToWindow popup = new FromToWindow();
            popup.ShowDialog();
        }
    }
}
