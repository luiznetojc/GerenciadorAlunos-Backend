#!/bin/bash

# 🚀 Script de Deploy para Render
# Sistema de Gerenciamento de Alunos

# Cores
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

clear
echo -e "${BLUE}"
echo "🚀 =================================="
echo "   DEPLOY PARA RENDER - BACKEND"
echo "   Sistema de Gerenciamento de Alunos"
echo "==================================="
echo -e "${NC}"

# Verificar se estamos no diretório correto
if [ ! -f "Dockerfile" ] || [ ! -f "render.yaml" ]; then
    echo -e "${YELLOW}❌ Execute este script no diretório GerenciadorAlunos-Backend${NC}"
    exit 1
fi

# Verificar status do Git
echo -e "${YELLOW}📋 Verificando status do Git...${NC}"
if ! git status &>/dev/null; then
    echo -e "${YELLOW}⚠️  Repositório Git não inicializado${NC}"
    echo -e "${YELLOW}💡 Inicialize com: git init${NC}"
    exit 1
fi

# Mostrar arquivos modificados
echo -e "${YELLOW}📁 Arquivos que serão commitados:${NC}"
git status --porcelain
echo ""

# Validar configuração
echo -e "${YELLOW}🔍 Executando validação...${NC}"
./validate-render.sh | tail -5
echo ""

# Confirmar deploy
echo -e "${YELLOW}❓ Deseja fazer o commit e push para deploy? (y/N)${NC}"
read -r response

if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
    echo -e "${YELLOW}📦 Adicionando arquivos...${NC}"
    git add .
    
    echo -e "${YELLOW}💾 Fazendo commit...${NC}"
    git commit -m "feat: Backend configurado para deploy no Render

✅ Dockerfile otimizado para produção
✅ Program.cs configurado para Render (PORT, DATABASE_URL)
✅ Health check endpoint implementado
✅ CORS configurado para produção
✅ ForwardedHeaders para proxy
✅ render.yaml com configuração automática
✅ Scripts de validação e deploy
✅ Documentação completa

Ready for production deployment! 🚀"

    echo -e "${YELLOW}🚀 Fazendo push...${NC}"
    git push origin $(git branch --show-current)
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}"
        echo "✅ =================================="
        echo "   PUSH REALIZADO COM SUCESSO!"
        echo "===================================="
        echo -e "${NC}"
        echo ""
        echo -e "${GREEN}🎯 Próximos passos:${NC}"
        echo ""
        echo "1. Acesse https://render.com"
        echo "2. Clique em 'New +' → 'Blueprint'"
        echo "3. Conecte seu repositório Git"
        echo "4. O arquivo render.yaml será detectado automaticamente"
        echo "5. Aguarde o build (5-10 minutos)"
        echo ""
        echo -e "${GREEN}📖 Para mais detalhes:${NC}"
        echo "• DEPLOY_RENDER.md - Guia completo"
        echo "• RENDER_READY.md - Status e resumo"
        echo ""
        echo -e "${GREEN}🌐 URLs após deploy:${NC}"
        echo "• API: https://seu-app.onrender.com"
        echo "• Swagger: https://seu-app.onrender.com/swagger"
        echo "• Health: https://seu-app.onrender.com/health"
        echo ""
    else
        echo -e "${YELLOW}❌ Erro no push. Verifique sua configuração Git.${NC}"
        exit 1
    fi
else
    echo -e "${YELLOW}⏸️ Deploy cancelado pelo usuário.${NC}"
    echo -e "${YELLOW}💡 Execute novamente quando estiver pronto.${NC}"
fi

echo -e "${GREEN}🎉 Script finalizado!${NC}"
