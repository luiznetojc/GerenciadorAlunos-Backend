using GerenciadorDeAlunos;
using Microsoft.EntityFrameworkCore;



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