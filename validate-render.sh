#!/bin/bash

# Script de validação para deployment no Render
# Sistema de Gerenciamento de Alunos

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 Validação para Deploy no Render${NC}"
echo -e "${BLUE}====================================${NC}"
echo ""

# Função para verificar se arquivo existe
check_file() {
    if [ -f "$1" ]; then
        echo -e "${GREEN}✅ $1 existe${NC}"
        return 0
    else
        echo -e "${RED}❌ $1 não encontrado${NC}"
        return 1
    fi
}

# Função para verificar conteúdo do arquivo
check_content() {
    if grep -q "$2" "$1" 2>/dev/null; then
        echo -e "${GREEN}✅ $1 contém: $2${NC}"
        return 0
    else
        echo -e "${RED}❌ $1 não contém: $2${NC}"
        return 1
    fi
}

# Verificar arquivos essenciais
echo -e "${YELLOW}📁 Verificando arquivos essenciais...${NC}"
check_file "Dockerfile"
check_file ".dockerignore" 
check_file "render.yaml"
check_file "DEPLOY_RENDER.md"
check_file "GerenciadorDeAlunos/GerenciadorDeAlunos.csproj"
check_file "GerenciadorDeAlunos/Program.cs"
check_file "GerenciadorDeAlunos/appsettings.json"
echo ""

# Verificar configurações do Dockerfile
echo -e "${YELLOW}🐳 Verificando Dockerfile...${NC}"
check_content "Dockerfile" "FROM mcr.microsoft.com/dotnet/sdk:8.0"
check_content "Dockerfile" "FROM mcr.microsoft.com/dotnet/aspnet:8.0"
check_content "Dockerfile" "ASPNETCORE_URLS=http://+:\$PORT"
check_content "Dockerfile" "HEALTHCHECK"
check_content "Dockerfile" "ENTRYPOINT.*GerenciadorDeAlunos.dll"
echo ""

# Verificar configurações do Program.cs
echo -e "${YELLOW}⚙️ Verificando Program.cs...${NC}"
check_content "GerenciadorDeAlunos/Program.cs" "Environment.GetEnvironmentVariable.*PORT"
check_content "GerenciadorDeAlunos/Program.cs" "DATABASE_URL"
check_content "GerenciadorDeAlunos/Program.cs" "UseForwardedHeaders"
check_content "GerenciadorDeAlunos/Program.cs" "MapHealthChecks"
echo ""

# Verificar render.yaml
echo -e "${YELLOW}☁️ Verificando render.yaml...${NC}"
check_content "render.yaml" "type: web"
check_content "render.yaml" "runtime: docker"
check_content "render.yaml" "healthCheckPath: /health"
check_content "render.yaml" "ASPNETCORE_ENVIRONMENT"
check_content "render.yaml" "DATABASE_URL"
echo ""

# Testar build local (opcional)
echo -e "${YELLOW}🔨 Testando build Docker local...${NC}"
if command -v docker &> /dev/null; then
    if docker build -t gerenciador-test . > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Docker build executado com sucesso${NC}"
        
        # Limpar imagem de teste
        docker rmi gerenciador-test > /dev/null 2>&1
    else
        echo -e "${RED}❌ Erro no Docker build${NC}"
        echo -e "${YELLOW}💡 Execute: docker build -t gerenciador-test . para ver detalhes${NC}"
    fi
else
    echo -e "${YELLOW}⏭️ Docker não disponível localmente (normal - Render fará o build)${NC}"
fi
echo ""

# Verificar dependências do projeto
echo -e "${YELLOW}📦 Verificando dependências...${NC}"
if [ -f "GerenciadorDeAlunos/GerenciadorDeAlunos.csproj" ]; then
    check_content "GerenciadorDeAlunos/GerenciadorDeAlunos.csproj" "Microsoft.AspNetCore"
    check_content "GerenciadorDeAlunos/GerenciadorDeAlunos.csproj" "Npgsql.EntityFrameworkCore.PostgreSQL"
    check_content "GerenciadorDeAlunos/GerenciadorDeAlunos.csproj" "Swashbuckle.AspNetCore"
fi
echo ""

# Checklist para deploy
echo -e "${BLUE}📋 Checklist para Deploy no Render:${NC}"
echo ""
echo -e "${YELLOW}Antes do deploy:${NC}"
echo "□ Código commitado e pushed para repositório Git"
echo "□ Variáveis de ambiente DATABASE_URL configurada"
echo "□ PostgreSQL database criado no Render"
echo "□ URL do frontend atualizada para produção (se necessário)"
echo ""
echo -e "${YELLOW}Durante o deploy:${NC}"
echo "□ Conectar repositório no Render"
echo "□ Configurar variáveis de ambiente"
echo "□ Aguardar primeiro build (pode demorar alguns minutos)"
echo "□ Testar endpoint /health"
echo ""
echo -e "${YELLOW}Após o deploy:${NC}"
echo "□ Testar API endpoints"
echo "□ Verificar Swagger UI"
echo "□ Confirmar conexão com banco de dados"
echo "□ Testar integração com frontend"
echo ""

# URLs que estarão disponíveis
echo -e "${GREEN}🌐 URLs que estarão disponíveis após deploy:${NC}"
echo "• API Base: https://seu-app.onrender.com"
echo "• Swagger: https://seu-app.onrender.com/swagger"  
echo "• Health: https://seu-app.onrender.com/health"
echo "• Students: https://seu-app.onrender.com/api/Student"
echo ""

echo -e "${GREEN}🎉 Validação concluída!${NC}"
echo -e "${YELLOW}📖 Consulte DEPLOY_RENDER.md para instruções detalhadas${NC}"
