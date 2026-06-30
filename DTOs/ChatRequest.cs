namespace InventoryAgent.DTOs
{
    public record ChatRequest(string UserId, string Role, string Prompt);
}
