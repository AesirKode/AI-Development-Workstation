using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AIWorkstation.Services.Agents
{
    public interface IAgent
    {
        string Name { get; }
        string Description { get; }
        Task<string> ExecuteAsync(string task, Dictionary<string, object> context);
        bool CanHandle(string task);
    }
}
