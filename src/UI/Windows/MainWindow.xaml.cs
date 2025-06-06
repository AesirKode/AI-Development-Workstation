// Add this to your MainWindow.xaml.cs to complete the agent system integration

using AIWorkstation.Services.Agents;
using AIWorkstation.Services.AI;

namespace AIWorkstation
{
    public partial class MainWindow : Window
    {
        private readonly AgentOrchestrator _agentOrchestrator;
        private readonly UnifiedAIService _aiService;

        // Add this to your constructor after existing initialization
        private void InitializeAgents()
        {
            _aiService = new UnifiedAIService();
            _agentOrchestrator = new AgentOrchestrator(_aiService);
        }

        // Enhanced SendChatMessage method with agent routing
        private async Task SendChatMessage()
        {
            var message = ChatInput.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            // Add user message to chat
            AddChatMessage(message, true);
            ChatInput.Text = "";

            try
            {
                // Show typing indicator
                var typingBorder = AddTypingIndicator();

                // Use agent orchestrator for smart routing
                var response = await _agentOrchestrator.ProcessTaskAsync(message);

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

        // Add Agents tab to your XAML TabControl (insert after Projects tab)
        /*
        <TabItem Header="🤖 Agents" Style="{StaticResource CategoryTabStyle}">
            <Grid Margin="20">
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                </Grid.RowDefinitions>
                
                <StackPanel Grid.Row="0">
                    <TextBlock Text="🤖 AI Agent Management" Style="{StaticResource HeaderTextStyle}"/>
                    <TextBlock Text="Specialized AI agents for different types of tasks" 
                             FontSize="14" Opacity="0.8" Margin="0,0,0,20"/>
                </StackPanel>
                
                <ScrollViewer Grid.Row="1" VerticalScrollBarVisibility="Auto">
                    <StackPanel x:Name="AgentsPanel">
                        <!-- CodeAgent Card -->
                        <Border Background="{DynamicResource MaterialDesignCardBackground}" 
                              CornerRadius="10" Padding="20" Margin="0,0,0,15"
                              materialDesign:ShadowAssist.ShadowDepth="Depth2">
                            <Grid>
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="*"/>
                                    <ColumnDefinition Width="Auto"/>
                                </Grid.ColumnDefinitions>
                                
                                <StackPanel Grid.Column="0">
                                    <StackPanel Orientation="Horizontal" Margin="0,0,0,10">
                                        <materialDesign:PackIcon Kind="Code" Height="24" Width="24" 
                                                               Foreground="{DynamicResource PrimaryHueMidBrush}"/>
                                        <TextBlock Text="CodeAgent" FontWeight="Bold" FontSize="18" 
                                                 Margin="10,0,0,0"/>
                                        <Border Background="#4CAF50" CornerRadius="10" Padding="5,2" 
                                              Margin="10,0,0,0">
                                            <TextBlock Text="ACTIVE" FontSize="10" Foreground="White"/>
                                        </Border>
                                    </StackPanel>
                                    
                                    <TextBlock Text="Specialized agent for all coding tasks including generation, review, debugging, and optimization."
                                             TextWrapping="Wrap" Margin="0,0,0,10" Opacity="0.8"/>
                                    
                                    <StackPanel Orientation="Horizontal">
                                        <TextBlock Text="Models:" FontWeight="Bold" Margin="0,0,5,0"/>
                                        <TextBlock Text="CodeLlama, DeepSeek Coder" Opacity="0.8"/>
                                    </StackPanel>
                                </StackPanel>
                                
                                <Button Grid.Column="1" Content="Test Agent" 
                                      Style="{StaticResource MaterialDesignRaisedButton}"
                                      Background="{DynamicResource PrimaryHueMidBrush}"
                                      Click="TestCodeAgent_Click"/>
                            </Grid>
                        </Border>
                        
                        <!-- SystemAgent Card -->
                        <Border Background="{DynamicResource MaterialDesignCardBackground}" 
                              CornerRadius="10" Padding="20" Margin="0,0,0,15"
                              materialDesign:ShadowAssist.ShadowDepth="Depth2">
                            <Grid>
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="*"/>
                                    <ColumnDefinition Width="Auto"/>
                                </Grid.ColumnDefinitions>
                                
                                <StackPanel Grid.Column="0">
                                    <StackPanel Orientation="Horizontal" Margin="0,0,0,10">
                                        <materialDesign:PackIcon Kind="Monitor" Height="24" Width="24" 
                                                               Foreground="{DynamicResource SecondaryHueMidBrush}"/>
                                        <TextBlock Text="SystemAgent" FontWeight="Bold" FontSize="18" 
                                                 Margin="10,0,0,0"/>
                                        <Border Background="#4CAF50" CornerRadius="10" Padding="5,2" 
                                              Margin="10,0,0,0">
                                            <TextBlock Text="ACTIVE" FontSize="10" Foreground="White"/>
                                        </Border>
                                    </StackPanel>
                                    
                                    <TextBlock Text="Handles system operations, monitoring, performance tracking, and hardware status."
                                             TextWrapping="Wrap" Margin="0,0,0,10" Opacity="0.8"/>
                                    
                                    <StackPanel Orientation="Horizontal">
                                        <TextBlock Text="Capabilities:" FontWeight="Bold" Margin="0,0,5,0"/>
                                        <TextBlock Text="GPU Status, System Info, Performance" Opacity="0.8"/>
                                    </StackPanel>
                                </StackPanel>
                                
                                <Button Grid.Column="1" Content="Test Agent" 
                                      Style="{StaticResource MaterialDesignRaisedButton}"
                                      Background="{DynamicResource SecondaryHueMidBrush}"
                                      Click="TestSystemAgent_Click"/>
                            </Grid>
                        </Border>
                    </StackPanel>
                </ScrollViewer>
            </Grid>
        </TabItem>
        */

        // Add these event handlers for agent testing
        private async void TestCodeAgent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var testMessage = "Write a simple Python function to calculate fibonacci numbers";
                var response = await _agentOrchestrator.ProcessTaskAsync(testMessage);

                MessageBox.Show($"CodeAgent Response:\n\n{response}",
                              "Agent Test Result", MessageBoxButton.OK, MessageBoxImage.Information);

                StatusText.Text = "CodeAgent test completed successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Agent test failed: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void TestSystemAgent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var testMessage = "Show me the current system status";
                var response = await _agentOrchestrator.ProcessTaskAsync(testMessage);

                MessageBox.Show($"SystemAgent Response:\n\n{response}",
                              "Agent Test Result", MessageBoxButton.OK, MessageBoxImage.Information);

                StatusText.Text = "SystemAgent test completed successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Agent test failed: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Add this to your button panel in the left sidebar (after System button)
        /*
        <Button Style="{StaticResource CategoryButtonStyle}" 
                Background="#FFFF6B6B"
                Click="Agents_Click">
            <StackPanel>
                <materialDesign:PackIcon Kind="Robot" Height="24" Width="24"/>
                <TextBlock Text="Agents" FontWeight="Bold" Margin="0,5,0,0"/>
                <TextBlock Text="AI specialists" FontSize="10" Opacity="0.8"/>
            </StackPanel>
        </Button>
        */

        private void Agents_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 3; // Switch to Agents tab (adjust index as needed)
        }
    }
}