// Enhanced Project Management System for AI Development Workstation
// Add these classes to: src/Services/Projects/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

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
        public string Name { get; set; }
        public ProjectType Type { get; set; }
        public string Description { get; set; }
        public List<string> Technologies { get; set; }
        public string PowerShellCommand { get; set; }
        public string Icon { get; set; }
        public bool IsAdvanced { get; set; }
    }

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
                    Name = "Node.js Express API",
                    Type = ProjectType.API,
                    Description = "Express.js API with MongoDB, authentication, and testing",
                    Technologies = new List<string> { "Node.js", "Express", "MongoDB", "JWT" },
                    PowerShellCommand = "newnode",
                    Icon = "Nodejs",
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
                },
                new ProjectTemplate
                {
                    Name = "Data Analysis Project",
                    Type = ProjectType.AI_ML,
                    Description = "Data analysis with Pandas, Matplotlib, and statistical modeling",
                    Technologies = new List<string> { "Python", "Pandas", "Matplotlib", "Seaborn", "Jupyter" },
                    PowerShellCommand = "newdata",
                    Icon = "ChartLine",
                    IsAdvanced = false
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

// Enhanced ProjectCreationDialog class for MainWindow.xaml.cs
public class EnhancedProjectCreationDialog : Window
{
    public Project CreatedProject { get; private set; }
    private readonly ProjectManager _projectManager;
    private ListBox _templatesListBox;
    private TextBox _projectNameTextBox;
    private TextBox _descriptionTextBox;
    private CheckBox _openAfterCreationCheckBox;

    public EnhancedProjectCreationDialog()
    {
        _projectManager = new ProjectManager();
        InitializeDialog();
    }

    private void InitializeDialog()
    {
        Title = "🚀 Create New Project";
        Width = 600;
        Height = 500;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        var mainGrid = new Grid();
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Header
        var headerPanel = new StackPanel { Margin = new Thickness(20, 20, 20, 10) };
        var titleText = new TextBlock
        {
            Text = "Choose a Project Template",
            FontSize = 18,
            FontWeight = FontWeights.Bold
        };
        headerPanel.Children.Add(titleText);
        Grid.SetRow(headerPanel, 0);
        mainGrid.Children.Add(headerPanel);

        // Templates List
        _templatesListBox = new ListBox
        {
            Margin = new Thickness(20, 0, 20, 10),
            SelectionMode = SelectionMode.Single
        };

        var templates = _projectManager.GetTemplates();
        foreach (var template in templates)
        {
            var item = new ListBoxItem
            {
                Content = CreateTemplateCard(template),
                Tag = template,
                Margin = new Thickness(0, 5, 0, 5)
            };
            _templatesListBox.Items.Add(item);
        }

        Grid.SetRow(_templatesListBox, 1);
        mainGrid.Children.Add(_templatesListBox);

        // Project Name
        var namePanel = new StackPanel { Margin = new Thickness(20, 0, 20, 10) };
        namePanel.Children.Add(new TextBlock { Text = "Project Name:", Margin = new Thickness(0, 0, 0, 5) });
        _projectNameTextBox = new TextBox { Padding = new Thickness(5) };
        namePanel.Children.Add(_projectNameTextBox);
        Grid.SetRow(namePanel, 2);
        mainGrid.Children.Add(namePanel);

        // Description
        var descPanel = new StackPanel { Margin = new Thickness(20, 0, 20, 10) };
        descPanel.Children.Add(new TextBlock { Text = "Description (optional):", Margin = new Thickness(0, 0, 0, 5) });
        _descriptionTextBox = new TextBox { Padding = new Thickness(5), Height = 60, TextWrapping = TextWrapping.Wrap };
        descPanel.Children.Add(_descriptionTextBox);
        Grid.SetRow(descPanel, 3);
        mainGrid.Children.Add(descPanel);

        // Bottom Panel
        var bottomPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(20)
        };

        _openAfterCreationCheckBox = new CheckBox
        {
            Content = "Open in VS Code after creation",
            IsChecked = true,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 20, 0)
        };

        var createButton = new Button
        {
            Content = "Create Project",
            Width = 120,
            Margin = new Thickness(5),
            IsDefault = true
        };
        createButton.Click += CreateButton_Click;

        var cancelButton = new Button
        {
            Content = "Cancel",
            Width = 80,
            Margin = new Thickness(5),
            IsCancel = true
        };

        bottomPanel.Children.Add(_openAfterCreationCheckBox);
        bottomPanel.Children.Add(createButton);
        bottomPanel.Children.Add(cancelButton);
        Grid.SetRow(bottomPanel, 4);
        mainGrid.Children.Add(bottomPanel);

        Content = mainGrid;
        _projectNameTextBox.Focus();
    }

    private StackPanel CreateTemplateCard(ProjectTemplate template)
    {
        var card = new StackPanel();

        var header = new StackPanel { Orientation = Orientation.Horizontal };
        header.Children.Add(new TextBlock
        {
            Text = template.Name,
            FontWeight = FontWeights.Bold,
            FontSize = 14
        });

        if (template.IsAdvanced)
        {
            var advancedBadge = new Border
            {
                Background = new SolidColorBrush(Colors.Orange),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(5, 2),
                Margin = new Thickness(10, 0, 0, 0)
            };
            advancedBadge.Child = new TextBlock { Text = "ADVANCED", FontSize = 10, Foreground = Brushes.White };
            header.Children.Add(advancedBadge);
        }

        card.Children.Add(header);
        card.Children.Add(new TextBlock
        {
            Text = template.Description,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 5, 0, 5),
            Opacity = 0.8
        });

        var techPanel = new StackPanel { Orientation = Orientation.Horizontal };
        techPanel.Children.Add(new TextBlock { Text = "Technologies: ", FontWeight = FontWeights.SemiBold });
        techPanel.Children.Add(new TextBlock { Text = string.Join(", ", template.Technologies), Opacity = 0.7 });
        card.Children.Add(techPanel);

        return card;
    }

    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_templatesListBox.SelectedItem is not ListBoxItem selectedItem ||
                selectedItem.Tag is not ProjectTemplate template)
            {
                MessageBox.Show("Please select a project template.", "No Template Selected",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var projectName = _projectNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(projectName))
            {
                MessageBox.Show("Please enter a project name.", "No Project Name",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var description = _descriptionTextBox.Text.Trim();

            // Show progress
            var progressWindow = new Window
            {
                Title = "Creating Project...",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = "Creating your project...", HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(20) },
                        new ProgressBar { IsIndeterminate = true, Margin = new Thickness(20) }
                    }
                }
            };
            progressWindow.Show();

            CreatedProject = await _projectManager.CreateProjectAsync(template, projectName, description);

            progressWindow.Close();

            if (_openAfterCreationCheckBox.IsChecked == true)
            {
                await _projectManager.OpenProjectAsync(CreatedProject);
            }

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to create project: {ex.Message}", "Error",
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}