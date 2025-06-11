#!/bin/bash

# 🔒 Script de Validação de Segurança - Verificar dados sensíveis
# Sistema de Gerenciamento de Alunos

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}🔒 Verificando Segurança antes do Commit...${NC}"
echo ""

# Lista de padrões perigosos
SENSITIVE_PATTERNS=(
    "Password=.*[^\"']"
    "password.*="
    "secret.*="
    "key.*="
    "@.*\.supabase\.co"
    "Host=.*\..*\..*"
    "Username=postgres.*Password="
    "JWT.*="
    "API.*KEY"
    "SECRET.*KEY"
)

# Lista de arquivos a verificar
FILES_TO_CHECK=(
    "appsettings.json"
    "appsettings.Production.json"
    "docker-compose.yml"
    "render.yaml"
    "*.env"
    "*.config"
)

FOUND_ISSUES=0

echo -e "${YELLOW}📁 Verificando arquivos sensíveis...${NC}"

for pattern in "${SENSITIVE_PATTERNS[@]}"; do
    echo -e "${YELLOW}🔍 Procurando padrão: $pattern${NC}"
    
    # Buscar em todos os arquivos relevantes
    if grep -r --include="*.json" --include="*.yml" --include="*.yaml" --include="*.env" --include="*.config" -n "$pattern" . 2>/dev/null; then
        echo -e "${RED}❌ ENCONTRADO PADRÃO SENSÍVEL: $pattern${NC}"
        FOUND_ISSUES=1
    fi
done

echo ""

# Verificar arquivos específicos
echo -e "${YELLOW}📋 Verificando arquivos específicos...${NC}"

# appsettings.json
if [ -f "GerenciadorDeAlunos/appsettings.json" ]; then
    if grep -q "Host=.*\." "GerenciadorDeAlunos/appsettings.json"; then
        echo -e "${RED}❌ appsettings.json contém connection string real${NC}"
        FOUND_ISSUES=1
    else
        echo -e "${GREEN}✅ appsettings.json está limpo${NC}"
    fi
fi

# docker-compose.yml
if [ -f "docker-compose.yml" ]; then
    if grep -q "Password=" "docker-compose.yml"; then
        echo -e "${RED}❌ docker-compose.yml contém password hardcoded${NC}"
        FOUND_ISSUES=1
    else
        echo -e "${GREEN}✅ docker-compose.yml está usando variáveis${NC}"
    fi
fi

# render.yaml
if [ -f "render.yaml" ]; then
    if grep -q "Host=" "render.yaml"; then
        echo -e "${RED}❌ render.yaml contém dados sensíveis${NC}"
        FOUND_ISSUES=1
    else
        echo -e "${GREEN}✅ render.yaml está seguro${NC}"
    fi
fi

echo ""

# Verificar se .env está no .gitignore
if [ -f ".gitignore" ]; then
    if grep -q "\.env" ".gitignore"; then
        echo -e "${GREEN}✅ .env está no .gitignore${NC}"
    else
        echo -e "${RED}❌ .env NÃO está no .gitignore${NC}"
        FOUND_ISSUES=1
    fi
fi

echo ""

# Resultado final
if [ $FOUND_ISSUES -eq 0 ]; then
    echo -e "${GREEN}🎉 =================================="
    echo "   VERIFICAÇÃO DE SEGURANÇA PASSOU!"
    echo "   Nenhum dado sensível encontrado"
    echo "==================================="
    echo -e "${NC}"
    echo ""
    echo -e "${GREEN}✅ Seguro para commit e deploy!${NC}"
    exit 0
else
    echo -e "${RED}🚨 =================================="
    echo "   PROBLEMAS DE SEGURANÇA ENCONTRADOS!"
    echo "   NÃO FAÇA COMMIT AINDA!"
    echo "===================================="
    echo -e "${NC}"
    echo ""
    echo -e "${YELLOW}🔧 Ações necessárias:${NC}"
    echo "1. Remova dados sensíveis dos arquivos"
    echo "2. Use variáveis de ambiente"
    echo "3. Execute novamente este script"
    echo "4. Só então faça o commit"
    echo ""
    exit 1
fi
