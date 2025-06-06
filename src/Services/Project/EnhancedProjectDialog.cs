using System;
using System.Collections.Generic;

namespace AIWorkstation.Services.Projects
{
    public class Project
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public ProjectType Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastOpened { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Technologies { get; set; } = new List<string>();
        public string GitRepository { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Version { get; set; } = "1.0.0";
        public List<string> Tags { get; set; } = new List<string>();
        public int OpenCount { get; set; } = 0;
        public TimeSpan TotalWorkTime { get; set; } = TimeSpan.Zero;
        public DateTime LastCommit { get; set; }
        public string Status { get; set; } = "Active"; // Active, Paused, Completed, Archived
        public int Priority { get; set; } = 3; // 1-5 scale
        public string Notes { get; set; } = string.Empty;
    }
}