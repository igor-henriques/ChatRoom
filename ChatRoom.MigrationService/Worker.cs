using ChatRoom.Domain.Entities;
using ChatRoom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace ChatRoom.MigrationService;

public sealed class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<Worker> logger,
    IConfiguration configuration) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("Ensuring database is created");
            await EnsureDatabaseAsync(dbContext, cancellationToken);

            logger.LogInformation("Running any pending migrations");
            await RunMigrationAsync(dbContext, cancellationToken);

            logger.LogInformation("Seeding data into the database");
            //await SeedDataAsync(serviceProvider);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error ocurred while preparing the database");
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private static async Task RunMigrationAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }

    private async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ExtendedIdentityUser>>();

        var existingUser = await userManager.FindByEmailAsync("messageconsumer@ironside.dev");
        if (existingUser == null)
        {
            var newUser = new ExtendedIdentityUser
            {
                UserName = "messageconsumer@ironside.dev",
                Email = "messageconsumer@ironside.dev",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newUser, configuration["BaseConsumerUserPassword"]!);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create seed user: {errors}");
            }
        }
    }
}