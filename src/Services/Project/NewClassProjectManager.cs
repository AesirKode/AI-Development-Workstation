using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace AIWorkstation.Services.Projects
{
    public class ProjectManager
    {
        private readonly string _projectsPath;
        private readonly List<ProjectTemplate> _templates;
        private readonly List<Project> _projects;

        public ProjectManager()
        {
            _projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AI-Projects");
            _templates = InitializeTemplates();
            _projects = new List<Project>();
            LoadProjects();
        }

        private List<ProjectTemplate> InitializeTemplates()
        {
            return new List<ProjectTemplate>
            {
                new ProjectTemplate
                {
                    Name = "Python AI/ML Project",
                    Type = ProjectType.AI_ML,
                    Description = "Complete AI/ML project with Jupyter notebooks, data processing, and model training",
                    Technologies = new List<string> { "Python", "Pandas", "NumPy", "Scikit-learn", "Jupyter" },
                    PowerShellCommand = "newai",
                    Icon = "Brain",
                    IsAdvanced = false
                },
                new ProjectTemplate
                {
                    Name = "Python Web Scraper",
                    Type = ProjectType.Python,
                    Description = "Web scraping project with BeautifulSoup, Selenium, and data export",
                    Technologies = new List<string> { "Python", "BeautifulSoup", "Selenium", "Pandas" },
                    PowerShellCommand = "newscraper",
                    Icon = "Web",
                    IsAdvanced = false
                },
                new ProjectTemplate
                {
                    Name = "FastAPI Backend",
                    Type = ProjectType.API,
                    Description = "Modern REST API with FastAPI, SQLAlchemy, and automatic documentation",
                    Technologies = new List<string> { "Python", "FastAPI", "SQLAlchemy", "Pydantic" },
                    PowerShellCommand = "newapi",
                    Icon = "Api",
                    IsAdvanced = true
                },
                new ProjectTemplate
                {
                    Name = "React Web App",
                    Type = ProjectType.WebApp,
                    Description = "Modern React application with TypeScript, Tailwind CSS, and Vite",
                    Technologies = new List<string> { "React", "TypeScript", "Tailwind CSS", "Vite" },
                    PowerShellCommand = "newreact",
                    Icon = "React",
                    IsAdvanced = true
                },
                new ProjectTemplate
                {
                    Name = "Python Automation Script",
                    Type = ProjectType.Python,
                    Description = "Task automation with file handling, email, and scheduling",
                    Technologies = new List<string> { "Python", "Schedule", "SMTP", "OS" },
                    PowerShellCommand = "newautomation",
                    Icon = "Robot",
                    IsAdvanced = false
                },
                new ProjectTemplate
                {
                    Name = "Desktop WPF App",
                    Type = ProjectType.Desktop,
                    Description = "WPF application with Material Design and MVVM pattern",
                    Technologies = new List<string> { "C#", "WPF", "Material Design", "MVVM" },
                    PowerShellCommand = "newwpf",
                    Icon = "DesktopWindows",
                    IsAdvanced = true
                }
            };
        }

        public List<ProjectTemplate> GetTemplates(bool includeAdvanced = true)
        {
            return includeAdvanced ? _templates : _templates.FindAll(t => !t.IsAdvanced);
        }

        public async Task<Project> CreateProjectAsync(ProjectTemplate template, string projectName, string description = "")
        {
            try
            {
                // Create project directory
                var projectPath = Path.Combine(_projectsPath, projectName);
                Directory.CreateDirectory(projectPath);

                // Run PowerShell command to create project structure
                await RunPowerShellCommand($"{template.PowerShellCommand} {projectName}");

                // Create project object
                var project = new Project
                {
                    Name = projectName,
                    Path = projectPath,
                    Type = template.Type,
                    Created = DateTime.Now,
                    LastOpened = DateTime.Now,
                    Description = description,
                    Technologies = new List<string>(template.Technologies),
                    IsActive = true
                };

                _projects.Add(project);
                SaveProjects();

                return project;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create project: {ex.Message}");
            }
        }

        public async Task OpenProjectAsync(Project project)
        {
            try
            {
                // Update last opened time
                project.LastOpened = DateTime.Now;
                SaveProjects();

                // Open in VS Code
                var startInfo = new ProcessStartInfo
                {
                    FileName = "code",
                    Arguments = $"\"{project.Path}\"",
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to open project: {ex.Message}");
            }
        }

        public List<Project> GetRecentProjects(int count = 10)
        {
            var sortedProjects = _projects.FindAll(p => p.IsActive);
            sortedProjects.Sort((a, b) => b.LastOpened.CompareTo(a.LastOpened));
            return sortedProjects.Take(count).ToList();
        }

        public void DeleteProject(Project project)
        {
            project.IsActive = false;
            SaveProjects();
        }

        private async Task RunPowerShellCommand(string command)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{command}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(startInfo))
            {
                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"PowerShell command failed: {error}");
                }
            }
        }

        private void LoadProjects()
        {
            try
            {
                var projectsFile = Path.Combine(_projectsPath, "projects.json");
                if (File.Exists(projectsFile))
                {
                    var json = File.ReadAllText(projectsFile);
                    var projects = System.Text.Json.JsonSerializer.Deserialize<List<Project>>(json);
                    _projects.AddRange(projects ?? new List<Project>());
                }
            }
            catch (Exception)
            {
                // If loading fails, start with empty list
            }
        }

        private void SaveProjects()
        {
            try
            {
                Directory.CreateDirectory(_projectsPath);
                var projectsFile = Path.Combine(_projectsPath, "projects.json");
                var json = System.Text.Json.JsonSerializer.Serialize(_projects, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(projectsFile, json);
            }
            catch (Exception)
            {
                // Silently fail if saving doesn't work
            }
        }
    }
}