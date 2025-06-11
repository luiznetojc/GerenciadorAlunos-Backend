#!/bin/bash

# Scripts para gerenciar o Docker do Backend - Sistema de Gerenciamento de Alunos

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Função para exibir ajuda
show_help() {
    echo -e "${GREEN}Scripts Docker - Sistema de Gerenciamento de Alunos${NC}"
    echo ""
    echo "Uso: ./docker-scripts.sh [COMANDO]"
    echo ""
    echo "Comandos disponíveis:"
    echo "  build      - Constrói a imagem Docker"
    echo "  run        - Executa o container"
    echo "  stop       - Para o container"
    echo "  restart    - Reinicia o container"
    echo "  logs       - Exibe os logs do container"
    echo "  clean      - Remove container e imagem"
    echo "  compose-up - Inicia com docker-compose"
    echo "  compose-down - Para docker-compose"
    echo "  health     - Verifica health check"
    echo "  help       - Exibe esta ajuda"
}

# Função para construir a imagem
build() {
    echo -e "${YELLOW}Construindo imagem Docker...${NC}"
    docker build -t gerenciador-alunos-backend .
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Imagem construída com sucesso!${NC}"
    else
        echo -e "${RED}❌ Erro ao construir imagem${NC}"
        exit 1
    fi
}

# Função para executar o container
run() {
    echo -e "${YELLOW}Iniciando container...${NC}"
    docker run -d \
        --name gerenciador-alunos-backend \
        -p 5226:5226 \
        -e ASPNETCORE_ENVIRONMENT=Production \
        gerenciador-alunos-backend

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Container iniciado com sucesso!${NC}"
        echo -e "${GREEN}🌐 API disponível em: http://localhost:5226${NC}"
        echo -e "${GREEN}🏥 Health check: http://localhost:5226/health${NC}"
    else
        echo -e "${RED}❌ Erro ao iniciar container${NC}"
        exit 1
    fi
}

# Função para parar o container
stop() {
    echo -e "${YELLOW}Parando container...${NC}"
    docker stop gerenciador-alunos-backend
    docker rm gerenciador-alunos-backend
    echo -e "${GREEN}✅ Container parado e removido${NC}"
}

# Função para reiniciar
restart() {
    stop
    run
}

# Função para exibir logs
logs() {
    echo -e "${YELLOW}Exibindo logs do container...${NC}"
    docker logs -f gerenciador-alunos-backend
}

# Função para limpeza completa
clean() {
    echo -e "${YELLOW}Removendo container e imagem...${NC}"
    docker stop gerenciador-alunos-backend 2>/dev/null
    docker rm gerenciador-alunos-backend 2>/dev/null
    docker rmi gerenciador-alunos-backend 2>/dev/null
    echo -e "${GREEN}✅ Limpeza concluída${NC}"
}

# Função para docker-compose up
compose_up() {
    echo -e "${YELLOW}Iniciando com docker-compose...${NC}"
    docker-compose up -d
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Aplicação iniciada com docker-compose!${NC}"
        echo -e "${GREEN}🌐 API disponível em: http://localhost:5226${NC}"
        echo -e "${GREEN}🏥 Health check: http://localhost:5226/health${NC}"
    else
        echo -e "${RED}❌ Erro ao iniciar com docker-compose${NC}"
        exit 1
    fi
}

# Função para docker-compose down
compose_down() {
    echo -e "${YELLOW}Parando docker-compose...${NC}"
    docker-compose down
    echo -e "${GREEN}✅ Docker-compose parado${NC}"
}

# Função para verificar health check
health() {
    echo -e "${YELLOW}Verificando health check...${NC}"
    response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5226/health)
    if [ "$response" = "200" ]; then
        echo -e "${GREEN}✅ Aplicação está saudável (HTTP 200)${NC}"
    else
        echo -e "${RED}❌ Aplicação não está respondendo corretamente (HTTP $response)${NC}"
    fi
}

# Main
case "$1" in
    build)
        build
        ;;
    run)
        run
        ;;
    stop)
        stop
        ;;
    restart)
        restart
        ;;
    logs)
        logs
        ;;
    clean)
        clean
        ;;
    compose-up)
        compose_up
        ;;
    compose-down)
        compose_down
        ;;
    health)
        health
        ;;
    help|--help|-h)
        show_help
        ;;
    *)
        echo -e "${RED}Comando inválido: $1${NC}"
        echo ""
        show_help
        exit 1
        ;;
esac
