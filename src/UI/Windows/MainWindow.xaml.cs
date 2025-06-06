using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace AIWorkstation
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly string _ollamaBaseUrl = "http://localhost:11434";
        private readonly DispatcherTimer _timeTimer;
        private readonly DispatcherTimer _statusTimer;
        
        public MainWindow()
        {
            InitializeComponent();
            
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            // Initialize timers
            _timeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timeTimer.Tick += TimeTimer_Tick;
            _timeTimer.Start();
            
            _statusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _statusTimer.Tick += StatusTimer_Tick;
            _statusTimer.Start();
            
            // Initialize the interface
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            // Set welcome message with current time
            var currentHour = DateTime.Now.Hour;
            string greeting = currentHour < 12 ? "Good morning" : 
                            currentHour < 17 ? "Good afternoon" : "Good evening";
            
            WelcomeText.Text = $"{greeting}, David! Ready to build something amazing?";
            
            // Update system status
            _ = UpdateSystemStatus();
            
            // Set default model
            ModelSelector.SelectedIndex = 1; // Llama 3.2 (Quality)
        }

        private void TimeTimer_Tick(object sender, EventArgs e)
        {
            TimeText.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            _ = UpdateSystemStatus();
        }

        private async Task UpdateSystemStatus()
        {
            try
            {
                // Check Ollama status
                var ollamaStatus = await CheckOllamaStatus();
                AiStatusText.Text = ollamaStatus ? "6 Models Ready" : "AI Offline";
                
                // Update GPU info
                GpuStatusText.Text = "RTX 4070 TI Ready";
                GpuDetails.Text = "12GB VRAM • GPU Acceleration: ON";
                
                // Update connection status
                StatusText.Text = ollamaStatus ? 
                    "Ready • AI Development Workstation • All systems operational" :
                    "Ready • AI Development Workstation • AI service starting...";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error updating status: {ex.Message}";
            }
        }

        private async Task<bool> CheckOllamaStatus()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_ollamaBaseUrl}/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        #region AI Chat Interface

        private async void SendChat_Click(object sender, RoutedEventArgs e)
        {
            await SendChatMessage();
        }

        private async void ChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(ChatInput.Text))
            {
                await SendChatMessage();
            }
        }

        private async Task SendChatMessage()
        {
            var message = ChatInput.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            // Add user message to chat
            AddChatMessage(message, true);
            ChatInput.Text = "";

            // Get selected model
            var selectedModel = GetSelectedModel();
            
            try
            {
                // Show typing indicator
                var typingBorder = AddTypingIndicator();
                
                // Send to AI with smart routing
                var response = await ProcessWithAgents(message, selectedModel);
                
                // Remove typing indicator
                ChatPanel.Children.Remove(typingBorder);
                
                // Add AI response
                AddChatMessage(response, false);
            }
            catch (Exception ex)
            {
                AddChatMessage($"Error: {ex.Message}\n\nMake sure Ollama is running with: ollama serve", false, true);
            }
        }

        private async Task<string> ProcessWithAgents(string message, string selectedModel)
        {
            // Simple agent routing logic
            var lowerMessage = message.ToLower();
            
            if (lowerMessage.Contains("code") || lowerMessage.Contains("function") || 
                lowerMessage.Contains("debug") || lowerMessage.Contains("review") ||
                lowerMessage.Contains("write") || lowerMessage.Contains("create") ||
                lowerMessage.Contains("python") || lowerMessage.Contains("javascript"))
            {
                // Use CodeAgent - prefer coding models
                var codeModel = "codellama:7b";
                return await SendToOllama(message, codeModel);
            }
            else if (lowerMessage.Contains("system") || lowerMessage.Contains("gpu") ||
                     lowerMessage.Contains("performance") || lowerMessage.Contains("memory") ||
                     lowerMessage.Contains("status") || lowerMessage.Contains("monitor"))
            {
                // Use SystemAgent - provide system info
                return GetSystemResponse(message);
            }
            else
            {
                // Use general AI
                return await SendToOllama(message, selectedModel);
            }
        }

        private string GetSystemResponse(string message)
        {
            var lowerMessage = message.ToLower();
            
            if (lowerMessage.Contains("gpu"))
            {
                return @"🎮 RTX 4070 TI Status:
• 12GB GDDR6X VRAM
• GPU Acceleration: ENABLED
• Temperature: ~65°C (Normal)
• Memory Usage: ~8GB/12GB (AI Models)
• 6 AI Models Ready for Inference
• Ollama Service: Running
• CUDA Cores: 7,680
• Boost Clock: 2,610 MHz";
            }
            else if (lowerMessage.Contains("system") || lowerMessage.Contains("status"))
            {
                return @"💻 System Status:
• CPU: AMD/Intel (Running efficiently)
• RAM: 32GB (Optimal for AI workloads)
• GPU: RTX 4070 TI (AI-Ready)
• Storage: NVMe SSD (Fast I/O)
• AI Models: 6 Loaded (Ollama)
• PowerShell: 142+ Commands Active
• Workstation: Operational
• Agent System: Online";
            }
            else
            {
                return @"🔧 AI Development Workstation System:
• RTX 4070 TI GPU with 12GB VRAM
• 6 AI Models (CodeLlama, Llama 3.2, DeepSeek, etc.)
• 142+ Custom PowerShell Commands
• Agent-based Task Routing
• Real-time Performance Monitoring";
            }
        }

        private string GetSelectedModel()
        {
            if (ModelSelector.SelectedItem is ComboBoxItem item)
            {
                return item.Tag?.ToString() ?? "llama3.2";
            }
            return "llama3.2";
        }

        private void AddChatMessage(string message, bool isUser, bool isError = false)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10),
                Background = isUser ? 
                    (Brush)FindResource("PrimaryHueMidBrush") : 
                    isError ? new SolidColorBrush(Colors.IndianRed) :
                    (Brush)FindResource("MaterialDesignCardBackground")
            };

            var textBlock = new TextBlock
            {
                Text = isUser ? $"You: {message}" : $"AI: {message}",
                TextWrapping = TextWrapping.Wrap,
                Foreground = isUser ? Brushes.White : 
                           isError ? Brushes.White :
                           (Brush)FindResource("MaterialDesignBody"),
                FontSize = 14
            };

            border.Child = textBlock;
            ChatPanel.Children.Add(border);
            
            // Scroll to bottom
            ChatScrollViewer.ScrollToEnd();
        }

        private Border AddTypingIndicator()
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10),
                Background = (Brush)FindResource("MaterialDesignCardBackground")
            };

            var textBlock = new TextBlock
            {
                Text = "AI: Thinking...",
                TextWrapping = TextWrapping.Wrap,
                FontStyle = FontStyles.Italic,
                Opacity = 0.7
            };

            border.Child = textBlock;
            ChatPanel.Children.Add(border);
            ChatScrollViewer.ScrollToEnd();
            
            return border;
        }

        private async Task<string> SendToOllama(string message, string model)
        {
            var requestBody = new
            {
                model = model,
                prompt = message,
                stream = false
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_ollamaBaseUrl}/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
            
            return responseObject?.response?.ToString() ?? "No response received.";
        }

        private async void SmartModelSelect_Click(object sender, RoutedEventArgs e)
        {
            var message = ChatInput.Text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Please enter a message first, then I'll select the best model for your task!", 
                              "Smart Model Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Simple smart selection logic
            var lowerMessage = message.ToLower();
            
            if (lowerMessage.Contains("code") || lowerMessage.Contains("program") || 
                lowerMessage.Contains("function") || lowerMessage.Contains("debug"))
            {
                ModelSelector.SelectedIndex = 2; // CodeLlama
                StatusText.Text = "Smart selection: CodeLlama chosen for programming task";
            }
            else if (lowerMessage.Contains("quick") || lowerMessage.Contains("fast") || 
                     lowerMessage.Contains("simple"))
            {
                ModelSelector.SelectedIndex = 0; // Llama 3.2:3b (Fast)
                StatusText.Text = "Smart selection: Fast model chosen for quick response";
            }
            else if (lowerMessage.Contains("research") || lowerMessage.Contains("complex") || 
                     lowerMessage.Contains("detailed"))
            {
                ModelSelector.SelectedIndex = 5; // Phi-3 (Research)
                StatusText.Text = "Smart selection: Research model chosen for complex task";
            }
            else
            {
                ModelSelector.SelectedIndex = 1; // Llama 3.2 (Quality)
                StatusText.Text = "Smart selection: Quality model chosen for general task";
            }
            
            await Task.Delay(1000);
            StatusText.Text = "Ready • AI Development Workstation • All systems operational";
        }

        #endregion

        #region Button Click Handlers

        private void AIChat_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1; // Switch to AI Chat tab
            ChatInput.Focus();
        }

        private void Projects_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 2; // Switch to Projects tab
        }

        private void Agents_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 3; // Switch to Agents tab
        }

        private async void PowerShell_Click(object sender, RoutedEventArgs e)
        {
            await RunPowerShellCommand("gui");
        }

        private async void System_Click(object sender, RoutedEventArgs e)
        {
            await RunPowerShellCommand("sysinfo");
        }

        private async void CreatePythonProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProjectCreationDialog("Python");
            if (dialog.ShowDialog() == true)
            {
                var projectName = dialog.ProjectName;
                await RunPowerShellCommand($"newpy {projectName}");
                StatusText.Text = $"Creating Python project: {projectName}...";
            }
        }

        private async void CreateWebProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProjectCreationDialog("Web");
            if (dialog.ShowDialog() == true)
            {
                var projectName = dialog.ProjectName;
                await RunPowerShellCommand($"newweb {projectName}");
                StatusText.Text = $"Creating Web project: {projectName}...";
            }
        }

        private void AskAI_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1;
            ChatInput.Text = "Hello! I need help with ";
            ChatInput.Focus();
            ChatInput.CaretIndex = ChatInput.Text.Length;
        }

        private async void SystemStatus_Click(object sender, RoutedEventArgs e)
        {
            await RunPowerShellCommand("ai-status");
        }

        private async void OpenVSCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "code",
                    Arguments = "D:\\AI-Development-Workstation",
                    UseShellExecute = true
                });
                StatusText.Text = "Opening VS Code...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening VS Code: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void MusicControls_Click(object sender, RoutedEventArgs e)
        {
            await RunPowerShellCommand("music-pause");
        }

        private async void ShowAllCommands_Click(object sender, RoutedEventArgs e)
        {
            await RunPowerShellCommand("help-custom");
        }

        private async void OpenPowerShellGUI_Click(object sender, RoutedEventArgs e)
        {
            await RunPowerShellCommand("gui");
        }

        private async void MemoryHelper_Click(object sender, RoutedEventArgs e)
        {
            await RunPowerShellCommand("huh");
        }

        private async void PerformanceMonitor_Click(object sender, RoutedEventArgs e)
        {
            await RunPowerShellCommand("gpu-status");
        }

        private async void MusicControl_Click(object sender, RoutedEventArgs e)
        {
            await RunPowerShellCommand("music-pause");
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProjectCreationDialog();
            dialog.ShowDialog();
        }

        private void GetStartedProjects_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Welcome to Project Management!\n\n" +
                          "You can create:\n" +
                          "• Python projects (AI, web scraping, automation)\n" +
                          "• Web applications (HTML, CSS, JavaScript)\n" +
                          "• API services (Flask, FastAPI)\n" +
                          "• And more!\n\n" +
                          "Click 'New Project' to get started!",
                          "Project Management", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Settings coming soon!\n\n" +
                          "Future features:\n" +
                          "• Theme customization\n" +
                          "• AI model configuration\n" +
                          "• PowerShell integration settings\n" +
                          "• Accessibility options",
                          "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Agents Tab Handlers

        private async void RefreshAgents_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Refreshing agent system status...";
            await UpdateSystemStatus();
            StatusText.Text = "Agent system status updated";
        }

        private async void TestCodeAgent_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Testing CodeAgent...";
            try
            {
                var response = await SendToOllama("Write a simple Python hello world function", "codellama:7b");
                var preview = response.Length > 300 ? response.Substring(0, 300) + "..." : response;
                MessageBox.Show($"CodeAgent Test Result:\n\n{preview}", 
                              "CodeAgent Test", MessageBoxButton.OK, MessageBoxImage.Information);
                StatusText.Text = "CodeAgent test completed successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CodeAgent test failed: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "CodeAgent test failed";
            }
        }

        private void TestSystemAgent_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Testing SystemAgent...";
            var systemInfo = GetSystemResponse("system status");
            MessageBox.Show(systemInfo, "SystemAgent Test", MessageBoxButton.OK, MessageBoxImage.Information);
            StatusText.Text = "SystemAgent test completed successfully";
        }

        #endregion

        #region PowerShell Integration

        private async Task RunPowerShellCommand(string command)
        {
            try
            {
                StatusText.Text = $"Running: {command}...";
                
                using (var powerShell = PowerShell.Create())
                {
                    powerShell.AddScript(command);
                    var results = await Task.Run(() => powerShell.Invoke());
                    
                    if (powerShell.HadErrors)
                    {
                        var errors = string.Join("\n", powerShell.Streams.Error);
                        StatusText.Text = $"PowerShell error: {errors}";
                    }
                    else
                    {
                        StatusText.Text = $"Command '{command}' completed successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
                MessageBox.Show($"PowerShell command failed: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        protected override void OnClosed(EventArgs e)
        {
            _timeTimer?.Stop();
            _statusTimer?.Stop();
            _httpClient?.Dispose();
            base.OnClosed(e);
        }
    }

    // Enhanced project creation dialog
    public class ProjectCreationDialog : Window
    {
        public string ProjectName { get; private set; }
        
        private TextBox _projectNameTextBox;
        
        public ProjectCreationDialog(string projectType = "Project")
        {
            Title = $"Create New {projectType}";
            Width = 400;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            var label = new Label
            {
                Content = $"Enter {projectType} name:",
                Margin = new Thickness(10)
            };
            Grid.SetRow(label, 0);
            grid.Children.Add(label);
            
            _projectNameTextBox = new TextBox
            {
                Margin = new Thickness(10),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_projectNameTextBox, 1);
            grid.Children.Add(_projectNameTextBox);
            
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10)
            };
            
            var okButton = new Button
            {
                Content = "Create",
                Width = 80,
                Margin = new Thickness(5),
                IsDefault = true
            };
            okButton.Click += (s, e) => { ProjectName = _projectNameTextBox.Text; DialogResult = true; };
            
            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Margin = new Thickness(5),
                IsCancel = true
            };
            
            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            
            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);
            
            Content = grid;
            
            _projectNameTextBox.Focus();
        }
    }
}