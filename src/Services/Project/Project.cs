using System;
using System.Collections.Generic;

namespace AIWorkstation.Services.Projects
{
    public class Project
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public ProjectType Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastOpened { get; set; }
        public string Description { get; set; }
        public List<string> Technologies { get; set; }
        public string GitRepository { get; set; }
        public bool IsActive { get; set; }
    }
}