using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace InventoryAgent.Services
{
    public class AgentFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public AgentFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<ChatClientAgent> CreateAgentAsync(string userId, string role)
        {
            var chatClient = _serviceProvider.GetRequiredService<IChatClient>();

            // Create an HttpClient with specific headers for THIS user
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("x-ms-api-userid", userId.ToString()); // Always use ToString()
            httpClient.DefaultRequestHeaders.Add("x-ms-api-role", role.ToString());

            // 2. Define the options (Endpoint, etc.)
            var transportOptions = new HttpClientTransportOptions
            {
                Endpoint = new Uri("http://localhost:5000/mcp")
            };

            // 3. Pass both to the HttpClientTransport constructor
            var transport = new HttpClientTransport(transportOptions, httpClient);

            // Now proceed with your McpClient
            var mcpClient = await McpClient.CreateAsync(transport);
            var mcpTools = await mcpClient.ListToolsAsync();
            var aiTools = mcpTools.Cast<AITool>().ToList();

            return new ChatClientAgent(chatClient, new ChatClientAgentOptions
            {
                ChatOptions = new ChatOptions
                {
                    Instructions = $"You are an IT Inventory Assistant. Current context: User {userId} ({role}). Use tools to query data.",
                    Tools = aiTools
                }
            });
        }
    }
}
