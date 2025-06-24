using GerenciadorDeAlunos;
using Microsoft.EntityFrameworkCore;

// Método para converter DATABASE_URL PostgreSQL para .NET Connection String
static string ConvertDatabaseUrl(string databaseUrl)
{
	if (string.IsNullOrEmpty(databaseUrl) || !databaseUrl.StartsWith("postgresql://"))
		return databaseUrl;

	try
	{
		var uri = new Uri(databaseUrl);
		var host = uri.Host;
		var port = uri.Port > 0 ? uri.Port : 5432;
		var database = uri.AbsolutePath.TrimStart('/');

		if (string.IsNullOrEmpty(database))
			database = "postgres";

		string username = "", password = "";
		if (!string.IsNullOrEmpty(uri.UserInfo))
		{
			var userInfo = uri.UserInfo.Split(':');
			username = userInfo[0];
			password = userInfo.Length > 1 ? userInfo[1] : "";
		}

		// Para Supabase, sempre usar SSL Mode=Require
		return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
	}
	catch
	{
		return databaseUrl;
	}
}

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

// Configuração da string de conexão
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
	?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("DATABASE_URL not found. Configure it in Render environment variables.");
}

// Log simples para verificar se a variável foi carregada
Console.WriteLine($"[INFO] Database URL configured: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL"))}");

// Converter para formato .NET se necessário
connectionString = ConvertDatabaseUrl(connectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(connectionString));

builder.Services.AddControllers();

// Repositories
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IStudentRepository, GerenciadorDeAlunos.Repositories.StudentRepository>();
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IDisciplineRepository, GerenciadorDeAlunos.Repositories.DisciplineRepository>();
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IEnrollmentRepository, GerenciadorDeAlunos.Repositories.EnrollmentRepository>();
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IMonthlyPaymentRepository, GerenciadorDeAlunos.Repositories.MonthlyPaymentRepository>();
builder.Services.AddScoped<GerenciadorDeAlunos.Repositories.IMonthlyPaymentDetailRepository, GerenciadorDeAlunos.Repositories.MonthlyPaymentDetailRepository>();

// Services
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IStudentService, GerenciadorDeAlunos.Services.StudentService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IDisciplineService, GerenciadorDeAlunos.Services.DisciplineService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IEnrollmentService, GerenciadorDeAlunos.Services.EnrollmentService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IMonthlyPaymentService, GerenciadorDeAlunos.Services.MonthlyPaymentService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IMonthlyPaymentDetailService, GerenciadorDeAlunos.Services.MonthlyPaymentDetailService>();
builder.Services.AddScoped<GerenciadorDeAlunos.Services.IMonthlyBillingService, GerenciadorDeAlunos.Services.MonthlyBillingService>();

var app = builder.Build();

// Swagger
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

// Headers e CORS
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

// Endpoints
app.MapHealthChecks("/health");
app.MapControllers();

// Log de inicialização
Console.WriteLine($"[INFO] Gerenciador de Alunos API iniciada - Ambiente: {app.Environment.EnvironmentName}");

app.Run();