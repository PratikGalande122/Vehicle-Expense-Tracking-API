using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleExpense.Application.Interfaces;
using VehicleExpense.Application.Services;
using VehicleExpense.Infrastructure.Data;
using VehicleExpense.Infrastructure.Email;
using VehicleExpense.Infrastructure.Repositories;
using VehicleExpense.Infrastructure.Security;

namespace VehicleExpense.Infrastructure.DependencyInjection;

/// <summary>
/// Registers all infrastructure services into the DI container.
/// </summary>
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=vehicleexpense.db";
        var dbFactory = new DbConnectionFactory(connectionString);
        dbFactory.InitializeDatabase();
        dbFactory.SeedDatabase();
        services.AddSingleton(dbFactory);

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IFuelRepository, FuelRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();

        // Security
        services.AddScoped<IJwtService, JwtService>();

        // Email
        services.AddScoped<IEmailService, SmtpEmailService>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IFuelService, FuelService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
