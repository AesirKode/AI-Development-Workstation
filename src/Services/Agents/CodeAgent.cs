using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIWorkstation.Services.AI;

namespace AIWorkstation.Services.Agents
{
    public class CodeAgent : IAgent
    {
        public string Name => "CodeAgent";
        public string Description => "Handles all coding tasks - generation, review, debugging";
        
        private readonly IAIApiService _aiService;

        public CodeAgent(IAIApiService aiService)
        {
            _aiService = aiService;
        }

        public bool CanHandle(string task)
        {
            var codeKeywords = new[] { "code", "function", "class", "debug", "review", "optimize", "write", "create" };
            return codeKeywords.Any(keyword => task.ToLower().Contains(keyword));
        }

        public async Task<string> ExecuteAsync(string task, Dictionary<string, object> context)
        {
            if (task.ToLower().Contains("debug"))
            {
                var code = context.GetValueOrDefault("code")?.ToString() ?? "";
                var error = context.GetValueOrDefault("error")?.ToString() ?? "";
                return await _aiService.DebugAsync(error, code);
            }
            else if (task.ToLower().Contains("review"))
            {
                var code = context.GetValueOrDefault("code")?.ToString() ?? "";
                return await _aiService.CodeReviewAsync(code);
            }
            else
            {
                return await _aiService.ChatAsync(task, "codellama:7b");
            }
        }
    }
}
