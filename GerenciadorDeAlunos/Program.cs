using GerenciadorDeAlunos;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "5226";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		if (builder.Environment.IsDevelopment())
		{
			policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
				  .AllowAnyMethod()
				  .AllowAnyHeader();
		}
		else
		{
			policy.AllowAnyOrigin()
				  .AllowAnyMethod()
				  .AllowAnyHeader();
		}
	});
});

// Configuração da string de conexão - múltiplas opções para Render
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
	?? Environment.GetEnvironmentVariable("DefaultConnection")
	?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
	?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("Connection string not found. Please configure DATABASE_URL or DefaultConnection in Render environment variables.");
}

// Log da variável original encontrada
var foundVariable = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL")) ? "DATABASE_URL" :
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DefaultConnection")) ? "DefaultConnection" :
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CONNECTION_STRING")) ? "CONNECTION_STRING" :
                   "appsettings.json";

Console.WriteLine($"[DEBUG] Connection source: {foundVariable}");
Console.WriteLine($"[DEBUG] Original format: {(connectionString.StartsWith("postgresql://") ? "PostgreSQL URL" : "Connection String")}");

// Converter DATABASE_URL para formato .NET se necessário
var originalConnectionString = connectionString;
connectionString = ConvertDatabaseUrl(connectionString);

Console.WriteLine($"[DEBUG] Converted: {originalConnectionString != connectionString}");
Console.WriteLine($"[DEBUG] Final connection ready: {!string.IsNullOrEmpty(connectionString)}");

// Método para converter DATABASE_URL em connection string .NET
static string ConvertDatabaseUrl(string databaseUrl)
{
	if (string.IsNullOrEmpty(databaseUrl) || !databaseUrl.StartsWith("postgresql://"))
		return databaseUrl;
	
	try
	{
		Console.WriteLine("[DEBUG] Converting PostgreSQL URL to .NET connection string...");
		var uri = new Uri(databaseUrl);
		var host = uri.Host;
		var port = uri.Port > 0 ? uri.Port : 5432;
		var database = uri.AbsolutePath.TrimStart('/');
		
		// Tratar casos onde não há database especificado
		if (string.IsNullOrEmpty(database))
			database = "postgres";
		
		// Extrair credenciais
		string username = "", password = "";
		if (!string.IsNullOrEmpty(uri.UserInfo))
		{
			var userInfo = uri.UserInfo.Split(':');
			username = userInfo[0];
			password = userInfo.Length > 1 ? userInfo[1] : "";
		}
		
		// Query parameters para SSL
		var query = uri.Query;
		var sslMode = query.Contains("sslmode=require") ? "Require" : "Prefer";
		
		var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};Trust Server Certificate=true";
		
		Console.WriteLine($"[DEBUG] Converted to: Host={host};Port={port};Database={database};Username={username};Password=***;SSL Mode={sslMode}");
		
		return connectionString;
	}
	catch (Exception ex)
	{
		Console.WriteLine($"[ERROR] Failed to convert DATABASE_URL: {ex.Message}");
		return databaseUrl; // Se falhar, retorna o original
	}
}

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(connectionString));

// Teste de conexão na inicialização
builder.Services.AddScoped(provider =>
{
	var context = provider.GetRequiredService<AppDbContext>();
	try
	{
		Console.WriteLine("[DEBUG] Testing database connection...");
		var canConnect = context.Database.CanConnect();
		Console.WriteLine($"[DEBUG] Database connection test: {(canConnect ? "SUCCESS" : "FAILED")}");
		
		if (!canConnect)
		{
			Console.WriteLine("[ERROR] Cannot connect to database. Check connection string and database availability.");
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"[ERROR] Database connection error: {ex.Message}");
		if (ex.InnerException != null)
		{
			Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
		}
	}
	return context;
});

builder.Services.AddControllers();
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IStudentRepository, GerenciadorDeAlunos.Repositories.StudentRepository>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IStudentService, GerenciadorDeAlunos.Services.StudentService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IDisciplineRepository, GerenciadorDeAlunos.Repositories.DisciplineRepository>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IDisciplineService, GerenciadorDeAlunos.Services.DisciplineService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IEnrollmentRepository, GerenciadorDeAlunos.Repositories.EnrollmentRepository>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IEnrollmentService, GerenciadorDeAlunos.Services.EnrollmentService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IMonthlyPaymentRepository, GerenciadorDeAlunos.Repositories.MonthlyPaymentRepository>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IMonthlyPaymentService, GerenciadorDeAlunos.Services.MonthlyPaymentService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IMonthlyPaymentDetailRepository, GerenciadorDeAlunos.Repositories.MonthlyPaymentDetailRepository>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IMonthlyPaymentDetailService, GerenciadorDeAlunos.Services.MonthlyPaymentDetailService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IMonthlyBillingService, GerenciadorDeAlunos.Services.MonthlyBillingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
else
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gerenciador de Alunos API V1");
		c.RoutePrefix = "swagger";
	});
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
					  | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

app.UseCors();

if (!app.Environment.IsProduction())
{
	app.UseHttpsRedirection();
}

app.MapHealthChecks("/health");

// Health check customizado com informações do banco
app.MapGet("/health/detailed", async (AppDbContext context) =>
{
	try
	{
		var canConnect = await context.Database.CanConnectAsync();
		return Results.Ok(new
		{
			status = "healthy",
			timestamp = DateTime.UtcNow,
			database = canConnect ? "connected" : "disconnected",
			environment = app.Environment.EnvironmentName
		});
	}
	catch (Exception ex)
	{
		return Results.Problem(new
		{
			status = "unhealthy",
			timestamp = DateTime.UtcNow,
			database = "error",
			error = ex.Message,
			environment = app.Environment.EnvironmentName
		}.ToString());
	}
});

app.MapControllers();

// Log de inicialização
Console.WriteLine("=".PadRight(50, '='));
Console.WriteLine($"[INFO] Gerenciador de Alunos API iniciada");
Console.WriteLine($"[INFO] Ambiente: {app.Environment.EnvironmentName}");
Console.WriteLine($"[INFO] Porta: {Environment.GetEnvironmentVariable("PORT") ?? "5226"}");
Console.WriteLine($"[INFO] Health Check: /health");
Console.WriteLine($"[INFO] Detailed Health: /health/detailed");
Console.WriteLine($"[INFO] Swagger: /swagger");
Console.WriteLine("=".PadRight(50, '='));

app.Run();