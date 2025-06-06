using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIWorkstation.Services.AI
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:11434";

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ChatAsync(string message, string model)
        {
            var request = new
            {
                model = model,
                prompt = message,
                stream = false
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{BaseUrl}/api/generate", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OllamaResponse>(responseContent);
            
            return result?.Response ?? "No response received.";
        }

        public async Task<string> CodeCompletionAsync(string code, string model)
        {
            var prompt = $"Complete this code:\n\n{code}";
            return await ChatAsync(prompt, model);
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class OllamaResponse
    {
        public string Response { get; set; }
    }
}
