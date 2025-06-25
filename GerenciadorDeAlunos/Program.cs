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
	?? builder.Configuration["DefaultConnection"] ?? string.Empty;

Console.WriteLine($"[INFO] Database connection configured from: {(Environment.GetEnvironmentVariable("DATABASE_URL") != null ? "DATABASE_URL environment variable" : "appsettings")}");

if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("DATABASE_URL not found. Configure it in Render environment variables.");
}



builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseNpgsql(connectionString, npgsqlOptions =>
	{
		// Configurações específicas para Render + Supabase
		npgsqlOptions.EnableRetryOnFailure(
			maxRetryCount: 5,
			maxRetryDelay: TimeSpan.FromSeconds(30),
			errorCodesToAdd: null);
		
		// Timeout aumentado para conexões lentas no Render
		npgsqlOptions.CommandTimeout(300);
	});

	// Log SQL queries em desenvolvimento
	if (builder.Environment.IsDevelopment())
	{
		options.EnableSensitiveDataLogging();
		options.LogTo(Console.WriteLine);
	}
	
	// Em produção, log apenas erros críticos
	if (builder.Environment.IsProduction())
	{
		options.LogTo(message => Console.WriteLine($"[EF] {message}"), Microsoft.Extensions.Logging.LogLevel.Error);
	}
});

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

// Teste de conexão com o banco de dados
try
{
	using var scope = app.Services.CreateScope();
	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

	Console.WriteLine("[INFO] Testando conexão com o banco de dados...");

	// Teste mais detalhado de conectividade
	var stopwatch = System.Diagnostics.Stopwatch.StartNew();
	await context.Database.CanConnectAsync();
	stopwatch.Stop();

	Console.WriteLine($"[INFO] ✅ Conexão com banco estabelecida com sucesso! ({stopwatch.ElapsedMilliseconds}ms)");

	// Verificar se as migrações estão aplicadas
	var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
	if (pendingMigrations.Any())
	{
		Console.WriteLine($"[WARNING] Existem {pendingMigrations.Count()} migrações pendentes");
		foreach (var migration in pendingMigrations)
		{
			Console.WriteLine($"[WARNING] Migração pendente: {migration}");
		}
	}
	else
	{
		Console.WriteLine("[INFO] ✅ Todas as migrações estão aplicadas");
	}

	// Teste simples de query
	try
	{
		var testQuery = await context.Database.ExecuteSqlRawAsync("SELECT 1");
		Console.WriteLine("[INFO] ✅ Teste de query executado com sucesso");
	}
	catch (Exception queryEx)
	{
		Console.WriteLine($"[WARNING] Erro no teste de query: {queryEx.Message}");
	}
}
catch (Exception ex)
{
	Console.WriteLine($"[ERROR] ❌ Falha na conexão com banco: {ex.Message}");
	Console.WriteLine($"[ERROR] Tipo da exceção: {ex.GetType().Name}");

	// Informações específicas para debugging
	if (ex.InnerException != null)
	{
		Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
		Console.WriteLine($"[ERROR] Inner exception type: {ex.InnerException.GetType().Name}");
	}

	// Detalhes específicos para diferentes tipos de erro
	if (ex.Message.Contains("timeout"))
	{
		Console.WriteLine("[ERROR] 🕐 Problema de timeout - verifique a latência da rede");
	}
	else if (ex.Message.Contains("host") || ex.Message.Contains("Host"))
	{
		Console.WriteLine("[ERROR] 🌐 Problema de DNS/Host - verifique a URL do Supabase");
	}
	else if (ex.Message.Contains("authentication") || ex.Message.Contains("password"))
	{
		Console.WriteLine("[ERROR] 🔐 Problema de autenticação - verifique usuário/senha");
	}
	else if (ex.Message.Contains("SSL") || ex.Message.Contains("certificate"))
	{
		Console.WriteLine("[ERROR] 🔒 Problema de SSL - verifique configurações de certificado");
	}

	Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");

	// Em produção, não falhar imediatamente para permitir debugging
	if (app.Environment.IsProduction())
	{
		Console.WriteLine("[ERROR] ⚠️  Aplicação iniciará mesmo com erro de banco para permitir debugging");
		Console.WriteLine("[ERROR] ⚠️  Endpoints que usam banco podem falhar!");
	}
}

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