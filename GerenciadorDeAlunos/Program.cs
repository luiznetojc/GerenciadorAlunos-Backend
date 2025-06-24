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

// Converter DATABASE_URL para formato .NET se necessário
connectionString = ConvertDatabaseUrl(connectionString);

// Log para debug (sem expor a senha)
Console.WriteLine($"Using connection string: {(connectionString.Contains("Password") ? "***CONNECTION SET***" : connectionString)}");

// Método para converter DATABASE_URL em connection string .NET
static string ConvertDatabaseUrl(string databaseUrl)
{
	if (string.IsNullOrEmpty(databaseUrl) || !databaseUrl.StartsWith("postgresql://"))
		return databaseUrl;
	
	try
	{
		var uri = new Uri(databaseUrl);
		var host = uri.Host;
		var port = uri.Port;
		var database = uri.AbsolutePath.TrimStart('/');
		var username = uri.UserInfo.Split(':')[0];
		var password = uri.UserInfo.Split(':')[1];
		
		return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
	}
	catch
	{
		return databaseUrl; // Se falhar, retorna o original
	}
}

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(connectionString));
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

app.MapControllers();

app.Run();