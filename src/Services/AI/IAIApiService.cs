using System;
using System.Threading.Tasks;

namespace AIWorkstation.Services.AI
{
    public interface IAIApiService
    {
        Task<string> ChatAsync(string message, string model = null);
        Task<string> CodeCompletionAsync(string code, string language);
        Task<string> CodeReviewAsync(string code);
        Task<string> DebugAsync(string error, string code);
        Task<bool> IsAvailableAsync();
    }
}
