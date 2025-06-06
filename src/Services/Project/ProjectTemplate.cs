using System.Collections.Generic;

namespace AIWorkstation.Services.Projects
{
    public enum ProjectType
    {
        Python,
        WebApp,
        API,
        Desktop,
        AI_ML,
        Game,
        Mobile
    }

    public class ProjectTemplate
    {
        public string Name { get; set; } = string.Empty;
        public ProjectType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Technologies { get; set; } = new List<string>();
        public string PowerShellCommand { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsAdvanced { get; set; }
        public int EstimatedSetupTime { get; set; } = 5; // minutes
        public string Category { get; set; } = "General";
        public List<string> Prerequisites { get; set; } = new List<string>();
    }
}