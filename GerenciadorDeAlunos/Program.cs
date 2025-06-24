using GerenciadorDeAlunos;
using Microsoft.EntityFrameworkCore;
using System.Linq;



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
				  .AllowAnyHeader()
				  .AllowCredentials();
		}
		else
		{
			policy.SetIsOriginAllowed(origin =>
			{
				if (string.IsNullOrEmpty(origin)) return false;
				
				// Permitir qualquer subdomínio do Vercel
				var uri = new Uri(origin);
				return uri.Host.EndsWith(".vercel.app") || 
					   uri.Host.EndsWith("vercel.app") ||
					   uri.Host == "localhost"; // Para testes locais
			})
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials();
		}
	});
});

// Configuração da string de conexão
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
	?? builder.Configuration["DefaultConnection"];
Console.WriteLine($"[INFO] Database URL configured: {connectionString}");

if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("DATABASE_URL not found. Configure it in Render environment variables.");
}



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

// Middleware de debug para CORS (apenas em produção para monitorar)
if (app.Environment.IsProduction())
{
	app.Use(async (context, next) =>
	{
		var origin = context.Request.Headers["Origin"].FirstOrDefault();
		Console.WriteLine($"[CORS DEBUG] Request from origin: {origin ?? "null"}");
		
		await next();
		
		var corsHeaders = context.Response.Headers.Where(h => h.Key.StartsWith("Access-Control-"));
		foreach (var header in corsHeaders)
		{
			Console.WriteLine($"[CORS DEBUG] Response header: {header.Key} = {header.Value}");
		}
	});
}

// CORS deve vir antes de outros middlewares
app.UseCors();

// Headers de forwarding (importante para Render)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
					  | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

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

if (!app.Environment.IsProduction())
{
	app.UseHttpsRedirection();
}

// Endpoints
app.MapHealthChecks("/health");
app.MapControllers();

// Log de inicialização
Console.WriteLine($"[INFO] Gerenciador de Alunos API iniciada - Ambiente: {app.Environment.EnvironmentName}");
Console.WriteLine($"[INFO] CORS configurado para aceitar origens *.vercel.app");

app.Run();