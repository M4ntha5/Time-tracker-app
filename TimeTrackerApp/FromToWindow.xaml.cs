using HrApp;
using HrApp.Domain;
using HrApp.Entities;
using HrApp.Repositories;
using HrApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TimmeTrackerApp
{
    /// <summary>
    /// Interaction logic for FromToWindow.xaml
    /// </summary>
    public partial class FromToWindow : Window
    {
        List<ProjectEntity> entitys;
        EmployeeEntity employee;

        public FromToWindow()
        {
            InitializeComponent();

            GetUser();
            var projects = GetAllProjects();

            List<string> list = new List<string>();
            foreach (var pro in projects)
            {
                list.Add(pro.Name);
            }

            FromToDropdown.ItemsSource = list;
            FromToDropdown.SelectedIndex = 0;
        }

        private void GetUser()
        {
            string username = (string)System.Windows.Application.Current.Resources["username"];

            employee = new EmployeeEntity() { FirstName = username, Id = "1" };
        }

        private List<ProjectEntity> GetAllProjects()
        {
            entitys = new List<ProjectEntity>();

            var pro1 = new ProjectEntity { Name = "pro1" };
            var pro2 = new ProjectEntity { Name = "pro2" };
            var pro3 = new ProjectEntity { Name = "pro3" };
            var pro4 = new ProjectEntity { Name = "pro4" };
            entitys.Add(pro1); entitys.Add(pro2);
            entitys.Add(pro3); entitys.Add(pro4);
            return entitys;
        }

        private  void CommitOneButton_Click(object sender, RoutedEventArgs e)
        {
            var timeWorked = TimeTextBox.Text;
            int t = 0;
            if (int.TryParse(timeWorked, out t))
            {
                var time = TimeSpan.FromHours(t);

                TimeTrackerService repo = new TimeTrackerService();
                var selectedProject = FromToDropdown.SelectedIndex;
                string description = string.Empty;

                if (DescriptionTextBox.Text == "")
                    description = "Worked on " + entitys[selectedProject].Name;
                else
                    description = DescriptionTextBox.Text;

                //  await repo.LogHours(employee, entitys[selectedProject], time, description);
                this.Close();
            }

            
        }
    }
}
