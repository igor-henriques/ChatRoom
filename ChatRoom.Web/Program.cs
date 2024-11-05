using ChatRoom.Infrastructure.HttpHandlers;
using ChatRoom.Infrastructure.Services;
using ChatRoom.Web;
using Microsoft.AspNetCore.DataProtection;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.ConfigureLogging();
    builder.AddNpgsqlDbContext<ApplicationDbContext>("chatroomdb");
    builder.AddRabbitMQClient("chatbroker");
    builder.AddRedisClient("chatroom-redis");

    builder.Services.AddDefaultIdentity<ExtendedIdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.AddServiceDefaults();

    builder.Services.AddScoped<AuthHandler>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.Configure<AppDefinitions>(builder.Configuration.GetSection(nameof(AppDefinitions)));
    builder.Services.AddSingleton<IPublisher, ChatMessagePublisher>();
    builder.Services.AddScoped<IAuthenticatedUserProvider, AuthenticatedUserProvider>();

    builder.Services.AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.IsEssential = true;
        options.Cookie.Name = "ChatRoom.Session";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.IdleTimeout = TimeSpan.FromDays(7);
    });

    builder.Services.AddScoped<ISessionTokenService, SessionTokenService>();
    builder.Services.AddScoped<RetryHandler>();
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))
        .SetApplicationName("ChatRoomApp")
        .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

    builder.Services.AddSignalR();
    builder.Services.AddRazorPages();
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddOutputCache();

    builder.Services.AddHttpClient<ITokenProviderApiClient, TokenProviderApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    }).AddHttpMessageHandler<RetryHandler>();

    builder.Services.AddHttpClient<IChatRoomApiClient, ChatRoomApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    })
    .AddHttpMessageHandler<AuthHandler>()
    .AddHttpMessageHandler<RetryHandler>();

    builder.Services.AddAuthorizationCore();
    builder.Services.AddDistributedMemoryCache();
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Identity/Account/Login";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseSession();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAntiforgery();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseOutputCache();

    app.MapRazorPages();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.MapDefaultEndpoints();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex.ToString());
    throw;
}
finally
{
    Log.CloseAndFlush();
}