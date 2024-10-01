using System.Net.Mail;
using Microsoft.EntityFrameworkCore; // Keep this line at the top

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Enable MVC controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add ApplicationDbContext with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the SmtpClient with your configuration
builder.Services.AddSingleton<SmtpClient>(provider =>
{
    var smtpClient = new SmtpClient("smtp.mail.me.com")
    {
        Port = 587, // Adjust port as needed
        Credentials = new System.Net.NetworkCredential("leticia.asilva@icloud.com", "eeaq-moyd-tvwj-tdco"),
        EnableSsl = true,
    };
    return smtpClient;
});

// Register the EmailService
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Create the database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated(); // Create the database
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();

// Redirect root URL to Swagger UI
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
}

app.UseHttpsRedirection();

app.MapControllers(); // Enable attribute routing for your controllers

app.Run();
