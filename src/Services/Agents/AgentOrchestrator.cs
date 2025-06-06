using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIWorkstation.Services.AI;

namespace AIWorkstation.Services.Agents
{
    public class AgentOrchestrator
    {
        private readonly List<IAgent> _agents;
        private readonly IAIApiService _aiService;

        public AgentOrchestrator(IAIApiService aiService)
        {
            _aiService = aiService;
            _agents = new List<IAgent>
            {
                new CodeAgent(aiService),
                new SystemAgent()
            };
        }

        public async Task<string> ProcessTaskAsync(string task, Dictionary<string, object> context = null)
        {
            context ??= new Dictionary<string, object>();
            
            // Find the best agent for the task
            var agent = _agents.FirstOrDefault(a => a.CanHandle(task));
            
            if (agent != null)
            {
                return await agent.ExecuteAsync(task, context);
            }
            else
            {
                // Fallback to general AI
                return await _aiService.ChatAsync(task);
            }
        }

        public List<IAgent> GetAvailableAgents()
        {
            return _agents.ToList();
        }
    }
}
