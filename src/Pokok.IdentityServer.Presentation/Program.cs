using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Domain.Events;
using Pokok.BuildingBlocks.Messaging.Abstractions;
using Pokok.BuildingBlocks.Messaging.RabbitMQ;
using Pokok.IdentityServer.Application.DomainEventHandlers;
using Pokok.IdentityServer.Domain.Aggregates.Users.Events;
using Pokok.IdentityServer.Infrastructure.DuendeIdentityServer;
using Pokok.Messaging.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Resolve ambiguity by explicitly specifying the namespace for AddIdentity
Pokok.IdentityServer.Infrastructure.Extensions.ServiceCollectionExtensions.AddIdentity(builder.Services, builder.Configuration);
Pokok.IdentityServer.Infrastructure.Extensions.ServiceCollectionExtensions.AddIdentityServer(builder.Services, builder.Configuration);
Pokok.IdentityServer.Infrastructure.Extensions.ServiceCollectionExtensions.AddOutbox(builder.Services, builder.Configuration);

builder.Services.AddRazorPages();

builder.Services.AddScoped<IDomainEventHandler<UserRegistrationConfirmationRequested>, UserRegistrationConfirmationRequestedHandler>();
builder.Services.AddOptions<EmailTemplatesOptions>().BindConfiguration(EmailTemplatesOptions.SectionName);
builder.Services.AddScoped<ITemplateRenderer, SimpleTemplateRenderer>();

builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
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

await IdentityServerSeed.SeedAsync(app.Services);
app.UseAuthentication();    // Required before UseIdentityServer
app.UseAuthorization();
app.UseIdentityServer();    // Registers IdentityServer middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // If you're using Map-based minimal hosting

app.Run();
