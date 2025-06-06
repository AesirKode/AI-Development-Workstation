using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AIWorkstation.Services.AI
{
    public class UnifiedAIService : IAIApiService
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaService _ollama;

        public UnifiedAIService()
        {
            _httpClient = new HttpClient();
            _ollama = new OllamaService(_httpClient);
        }

        public async Task<string> ChatAsync(string message, string model = null)
        {
            try
            {
                return await _ollama.ChatAsync(message, model ?? "llama3.2");
            }
            catch (Exception ex)
            {
                return $"Error connecting to AI: {ex.Message}";
            }
        }

        public async Task<string> CodeCompletionAsync(string code, string language)
        {
            var model = language.ToLower() switch
            {
                "python" => "codellama:7b",
                "csharp" => "deepseek-coder:6.7b", 
                "javascript" => "codellama:7b",
                _ => "codellama:7b"
            };

            return await _ollama.CodeCompletionAsync(code, model);
        }

        public async Task<string> CodeReviewAsync(string code)
        {
            var prompt = $@"
Review this code for:
- Bugs and errors
- Performance issues  
- Security vulnerabilities
- Best practices
- Suggestions for improvement

Code:
{code}

Provide a detailed review with specific recommendations.";

            return await ChatAsync(prompt, "deepseek-coder:6.7b");
        }

        public async Task<string> DebugAsync(string error, string code)
        {
            var prompt = $@"
Debug this error:
Error: {error}

Code: {code}

Explain what's wrong and provide a fixed version.";

            return await ChatAsync(prompt, "codellama:7b");
        }

        public async Task<bool> IsAvailableAsync()
        {
            return await _ollama.IsAvailableAsync();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
