using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIWorkstation.Services.Projects
{
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
}