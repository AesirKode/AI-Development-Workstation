using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIWorkstation.Services.Agents
{
    public class SystemAgent : IAgent
    {
        public string Name => "SystemAgent";
        public string Description => "Handles system operations, monitoring, and control";

        public bool CanHandle(string task)
        {
            var systemKeywords = new[] { "system", "monitor", "status", "performance", "gpu", "memory", "cpu" };
            return systemKeywords.Any(keyword => task.ToLower().Contains(keyword));
        }

        public async Task<string> ExecuteAsync(string task, Dictionary<string, object> context)
        {
            if (task.ToLower().Contains("status"))
            {
                return await GetSystemStatusAsync();
            }
            else if (task.ToLower().Contains("gpu"))
            {
                return await GetGPUStatusAsync();
            }
            
            return "System task completed";
        }

        private async Task<string> GetSystemStatusAsync()
        {
            await Task.Delay(100); // Simulate async work
            return "System: Operational\nCPU: 15%\nRAM: 8GB/32GB\nGPU: RTX 4070 TI Ready\nAI Models: 6 Ready";
        }

        private async Task<string> GetGPUStatusAsync()
        {
            await Task.Delay(100); // Simulate async work
            return "RTX 4070 TI\n12GB VRAM\nGPU Acceleration: Enabled\n6 AI Models Ready\nOllama: Running";
        }
    }
}
