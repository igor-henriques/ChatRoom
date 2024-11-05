using ChatRoom.ApiService.Application.Behaviors;
using ChatRoom.ApiService.Application.Services;
using ChatRoom.ApiService.Middlewares;
using ChatRoom.ApiService.Models;
using ChatRoom.Infrastructure.Data;
using ChatRoom.Infrastructure.Repositories;
using Swashbuckle.AspNetCore.Filters;

namespace ChatRoom.ApiService.StartupConfiguration;

public static class ServicesConfigurator
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureLogging();
        builder.AddRabbitMQClient("chatbroker");
        builder.AddServiceDefaults();
        builder.AddNpgsqlDbContext<ApplicationDbContext>("chatroomdb");

        builder.Services.Configure<AppDefinitions>(builder.Configuration.GetSection(nameof(AppDefinitions)));

        builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(Program).Assembly)
          .AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>)));        

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        builder.Services.AddAuthorization();

        builder.Services.AddIdentityApiEndpoints<ExtendedIdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddProblemDetails();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<Domain.Services.IPublisher, RealTimeFanoutPublisher>();

        return builder;
    }
}
