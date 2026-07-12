using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Augur.Agent;
using Augur.Agent.Knowledge;
using Augur.Agent.Llm;

namespace Augur.Agent;

/// <summary>Registers the agent layer. With <c>Agent:LlmEndpoint</c> set it wires an LLM-backed
/// agent; empty leaves a deterministic-only (offline-safe) agent.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAgent(this IServiceCollection services, IConfiguration configuration)
    {
        var catalog = ControlCatalog.Default();
        services.AddSingleton(catalog);

        var endpoint = configuration["Agent:LlmEndpoint"];
        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            var model = configuration["Agent:Model"] ?? "local-model";
            services.AddHttpClient<OpenAiCompatibleClient>(client =>
            {
                client.BaseAddress = new Uri(endpoint);
                client.Timeout = TimeSpan.FromSeconds(60);
            });
            // Factory so the client resolves its base address from DI.
            services.AddScoped<ComplianceAgent>(sp =>
            {
                var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(OpenAiCompatibleClient));
                return new ComplianceAgent(new OpenAiCompatibleClient(http, model), catalog);
            });
        }
        else
        {
            services.AddScoped<ComplianceAgent>(_ => new ComplianceAgent(null, catalog));
        }

        services.AddScoped<ComplianceAgentService>();
        return services;
    }
}
