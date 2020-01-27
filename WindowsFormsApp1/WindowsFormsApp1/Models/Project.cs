using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Project
    {
        public string Name { get; set; }
        public TimeSpan TimeWorked { get; set; }

        public Project(string name, TimeSpan timeWorked)
        {
            Name = name;
            TimeWorked = timeWorked;
        }
    }
}
