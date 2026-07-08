using FluentValidation;
using MediatR;
using MiniGrc.Application.Behaviors;
using MiniGrc.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace MiniGrc.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddValidatorsFromAssembly(assembly);
        MappingConfig.Register();

        return services;
    }
}
