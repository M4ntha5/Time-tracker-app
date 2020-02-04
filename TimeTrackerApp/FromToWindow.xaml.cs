using HrApp;
using HrApp.Entities;
using HrApp.Repositories;
using HrApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;


namespace TimeTrackerApp
{
    /// <summary>
    /// Interaction logic for FromToWindow.xaml
    /// </summary>
    public partial class FromToWindow : Window
    {
        List<ProjectEntity> AllProjects;
        EmployeeEntity Employee;

        public FromToWindow()
        {
            InitializeComponent();
            SetUp();
        }
        private async void SetUp()
        {
            //temporary only until working aad
            Employee = await GetEmployee("5e20785d2bd93500011dbf6f");
            AllProjects = await GetProjects();

            List<string> namesList = new List<string>();
            foreach (var pro in AllProjects)
            {
                namesList.Add(pro.Name);
            }

            FromToDropdown.ItemsSource = namesList;
            FromToDropdown.SelectedIndex = 0;
        }
        private void CommitOneButton_Click(object sender, RoutedEventArgs e)
        {
            var timeWorked = TimeTextBox.Text;

            if (double.TryParse(timeWorked, out double t) )
            {           
                if(t <= 0)
                {
                    DisplayError("Time must be greater than 0! (>0)");
                    return;
                }

                var time = TimeSpan.FromHours(t);           
                var projectId = FromToDropdown.SelectedIndex;
                string description;

                if (DescriptionTextBox.Text == "")
                {
                    DisplayError("Description field is required!");
                    return;
                }
                else
                    description = DescriptionTextBox.Text;

                LoggOne(projectId, time, description);
                this.Close();
            }
            else
            {
                DisplayError("Time is required and must be numeric!");
                return;
            }
        }

        private void DisplayError(string message)
        {
            ErrorLabel.Visibility = Visibility.Visible;
            ErrorLabel.Content = message;
        }
        private async void LoggOne(int projectId, TimeSpan timeWorked, string description)
        {
            TimeTrackerService repo = new TimeTrackerService()
            {
                CommitRepository = new CommitRepository(),
                ProjectRepository = new ProjectRepository(),
                EmployeeRepository = new EmployeesRepository()
            };

            await repo.LogHours(Employee, AllProjects[projectId], timeWorked, description);
        }
        private async Task<EmployeeEntity> GetEmployee(string id)
        {
            EmployeesRepository repo = new EmployeesRepository();
            var emp = await repo.GetEmployeeById(id);

            return emp;
        }
        private async Task<List<ProjectEntity>> GetProjects()
        {
            ProjectRepository projectRepository = new ProjectRepository();
            var projects = await projectRepository.GetAllProjects();

            return projects;
        }
    }
}
