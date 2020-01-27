using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1
{
    public partial class TrackerForm : Form
    {
        private DateTime StartTime;
        TimeSpan elapsed;
        List<Project> projects = new List<Project>();
        public TrackerForm()
        {
            InitializeComponent();
        }

        private void start_button_Click_1(object sender, EventArgs e)
        {
            if(start_button.Text == "Stop")
            {
                var projectname = comboBox1.SelectedItem.ToString();
                var timeWorked = elapsed;
                Project project = new Project(projectname, timeWorked);
                ShowBeforeCommit(project);
            }
            timer1.Enabled = !timer1.Enabled;
            start_button.Text = timer1.Enabled ? "Stop" : "Start";
            StartTime = DateTime.Now;        
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            elapsed = DateTime.Now - StartTime;
            string text = "";


            text += elapsed.Hours.ToString("00") + ":" +
                    elapsed.Minutes.ToString("00") + ":" +
                    elapsed.Seconds.ToString("00");

            label1.Text = text;
        }

        private void TrackerForm_Load(object sender, EventArgs e)
        {
            comboBox1.Enabled = start_button.Text == "Stop" ? false : true;

            List<string> list = new List<string>();
            list.Add("pro1");
            list.Add("pro2");
            list.Add("pro3");
            list.Add("pro4");
            list.Add("pro5");
            

            comboBox1.DataSource = list;
        }

        private void ShowBeforeCommit(Project project)
        {
            projects.Add(project);
            var source = new BindingSource(projects, null);
            dataGridView1.DataSource = source;
        }

        private void InsertLogs()
        {

        }

        private void commit_button_Click(object sender, EventArgs e)
        {

        }
    }
}
