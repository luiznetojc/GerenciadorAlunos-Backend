using GerenciadorDeAlunos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar porta do Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "5226";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Health Checks
builder.Services.AddHealthChecks();

// Configure CORS para produção
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
			// Para produção, configure domínios específicos do seu frontend
			policy.AllowAnyOrigin()
				  .AllowAnyMethod()
				  .AllowAnyHeader();
		}
	});
});
// Configurar conexão com banco (usar variável de ambiente para produção)
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
	?? builder.Configuration.GetConnectionString("DefaultConnection");

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
else
{
	// Em produção, também habilitar Swagger para testes
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gerenciador de Alunos API V1");
		c.RoutePrefix = "swagger";
	});
}

// Configurar headers para proxies (necessário para Render)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
					  | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

app.UseCors();

// Em produção do Render, não forçar HTTPS pois o proxy já trata isso
if (!app.Environment.IsProduction())
{
	app.UseHttpsRedirection();
}

// Health Check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();