# Dockerfile para o Backend ASP.NET Core - Sistema de Gerenciamento de Alunos
# Otimizado para deployment no Render

# Etapa 1: Build - usando SDK completo
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar arquivos de projeto e restaurar dependências
COPY GerenciadorDeAlunos.sln ./
COPY GerenciadorDeAlunos/*.csproj ./GerenciadorDeAlunos/

# Restaurar dependências com cache otimizado
RUN dotnet restore GerenciadorDeAlunos.sln --verbosity minimal

# Copiar todo o código fonte
COPY . .

# Construir e publicar a aplicação em uma única etapa
WORKDIR /app/GerenciadorDeAlunos
RUN dotnet publish -c Release -o /app/publish --no-restore --verbosity minimal

# Etapa 2: Runtime - usando runtime otimizado
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Instalar dependências necessárias
RUN apt-get update && apt-get install -y \
	curl \
	ca-certificates \
	&& apt-get clean \
	&& rm -rf /var/lib/apt/lists/*

# Copiar arquivos publicados
COPY --from=build /app/publish .

# Configurar variáveis de ambiente para Render
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:$PORT
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Expor a porta (Render usa variável PORT)
EXPOSE $PORT

# Health check otimizado para Render
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
	CMD curl -f http://localhost:$PORT/health || exit 1

# Comando de entrada
ENTRYPOINT ["dotnet", "GerenciadorDeAlunos.dll"]
