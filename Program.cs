using InventoryAgent.Services;
using Microsoft.Extensions.AI;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, new DefaultJsonTypeInfoResolver());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the Gemini Client
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var apiKey = builder.Configuration["Gemini:ApiKey"] ?? throw new Exception("Gemini key not found");
    return new Google.GenAI.Client(apiKey: apiKey).AsIChatClient("gemini-3.1-flash-lite");
});

// Register the AgentFactory instead of the Singleton Agent
builder.Services.AddScoped<AgentFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.RoutePrefix = "swagger"; });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
