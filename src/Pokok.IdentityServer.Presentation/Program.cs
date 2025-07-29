using Pokok.BuildingBlocks.Domain.Events;
using Pokok.BuildingBlocks.Messaging.Abstractions;
using Pokok.BuildingBlocks.Messaging.RabbitMQ;
using Pokok.IdentityServer.Application.DomainEventHandlers;
using Pokok.IdentityServer.Domain.Aggregates.Users.Events;
using Pokok.IdentityServer.Infrastructure.BackgroundWorkers;
using Pokok.IdentityServer.Infrastructure.DuendeIdentityServer;
using Pokok.IdentityServer.Infrastructure.Extensions;
using Pokok.Messaging.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Resolve ambiguity by explicitly specifying the namespace for AddIdentity
ServiceCollectionExtensions.AddIdentity(builder.Services, builder.Configuration);
ServiceCollectionExtensions.AddIdentityServer(builder.Services, builder.Configuration);
ServiceCollectionExtensions.AddOutbox(builder.Services, builder.Configuration);

builder.Services.AddRazorPages();

builder.Services.AddScoped<IDomainEventHandler<UserRegistrationConfirmationRequestedDomainEvent>, UserRegistrationConfirmationRequestedDomainEventHandler>();
builder.Services.Configure<EmailTemplatesOptions>(builder.Configuration.GetSection(EmailTemplatesOptions.SectionName));
builder.Services.Configure<UserRegisteredConfirmationOptions>(builder.Configuration.GetSection(UserRegisteredConfirmationOptions.SectionName));
builder.Services.AddScoped<ITemplateRenderer, SimpleTemplateRenderer>();
builder.Services.AddHostedService<OutboxProcessorHostedService>();

builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMQMessagePublisher>();
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

await IdentityServerSeed.SeedAsync(app.Services, builder.Environment.EnvironmentName);
app.UseAuthentication();    // Required before UseIdentityServer
app.UseAuthorization();
app.UseIdentityServer();    // Registers IdentityServer middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var environment = builder.Environment.EnvironmentName;
await IdentityServerSeed.SeedAsync(app.Services, environment);

app.MapRazorPages(); // If you're using Map-based minimal hosting

app.Run();
