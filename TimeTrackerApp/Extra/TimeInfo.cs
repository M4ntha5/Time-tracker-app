using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimmeTrackerApp.Extra
{
    public class TimeInfo
    {
        public string ProjectName { get; set; }
        public TimeSpan Time { get; set; }
        public string Description { get; set; }
        public TimeInfo(string projectName, TimeSpan time, string description)
        {
            ProjectName = projectName;
            Time = time;
            Description = description;
        }
    }
}
