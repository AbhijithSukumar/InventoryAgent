using InventoryAgent.DTOs;
using InventoryAgent.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly AgentFactory _factory;

    // Inject the Factory instead of the Agent instance
    public AgentController(AgentFactory factory)
    {
        _factory = factory;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest("Message cannot be empty.");
        }

        try
        {
            // 1. Use the factory to create a scoped agent instance 
            // that contains the correct HttpClient and headers for this specific user.
            var agent = await _factory.CreateAgentAsync(request.UserId, request.Role);

            // 2. Prepare the context
            string identityContext = $"[CURRENT_USER_CONTEXT]\nUserId: {request.UserId}\nRole: {request.Role}\n";
            var fullPrompt = $"{identityContext}\nUser Message: {request.Prompt}";

            // 3. Run the reasoning loop with the scoped agent
            var response = await agent.RunAsync(fullPrompt);

            return Ok(response.ToString());
        }
        catch (Exception ex)
        {
            // In a real app, use ILogger to log the exception
            return StatusCode(500, $"An error occurred while processing the request: {ex.Message}");
        }
    }
}
