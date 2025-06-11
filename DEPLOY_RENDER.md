# 🚀 Deployment no Render - Sistema de Gerenciamento de Alunos

## 📋 Pré-requisitos

1. Conta no [Render](https://render.com)
2. Repositório Git com o código (GitHub, GitLab, etc.)
3. Docker configurado (já incluído neste projeto)

## 🔧 Configuração no Render

### Opção 1: Deploy Automático via render.yaml

1. **Faça push do código para seu repositório**
2. **Conecte o repositório no Render:**
   - Acesse o dashboard do Render
   - Clique em "New +" → "Blueprint"
   - Conecte seu repositório
   - O arquivo `render.yaml` será detectado automaticamente

### Opção 2: Deploy Manual

#### 1. Criar Database PostgreSQL

1. No dashboard do Render: "New +" → "PostgreSQL"
2. Configure:

   - **Name**: `gerenciador-alunos-db`
   - **Database**: `gerenciador_alunos`
   - **User**: `postgres`
   - **Region**: Escolha a região mais próxima
   - **Plan**: Free (para testes) ou Starter/Pro

3. **Anote a URL de conexão** que será fornecida

#### 2. Criar Web Service

1. No dashboard do Render: "New +" → "Web Service"
2. Conecte seu repositório Git
3. Configure:
   - **Name**: `gerenciador-alunos-backend`
   - **Runtime**: `Docker`
   - **Build Command**: _(deixe vazio - usa Dockerfile)_
   - **Start Command**: _(deixe vazio - usa Dockerfile)_
   - **Plan**: Free, Starter ou Standard

#### 3. Configurar Variáveis de Ambiente

Na seção "Environment" do seu Web Service, adicione:

```bash
# Obrigatórias
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=[SUA_URL_DO_POSTGRESQL_AQUI]

# Opcionais (já configuradas no Dockerfile)
ASPNETCORE_URLS=http://+:$PORT
DOTNET_RUNNING_IN_CONTAINER=true
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
```

**⚠️ Importante**: A variável `DATABASE_URL` deve ser a string de conexão completa do PostgreSQL criado no passo 1.

## 🔗 Exemplo de URL de Conexão

```
postgresql://usuario:senha@host:porta/database
```

Exemplo real:

```
postgresql://gerenciador_user:abc123@dpg-abc123-a.oregon-postgres.render.com:5432/gerenciador_alunos
```

## 📁 Estrutura dos Arquivos de Deploy

```
GerenciadorAlunos-Backend/
├── Dockerfile                  # ✅ Otimizado para Render
├── .dockerignore              # ✅ Otimiza build
├── render.yaml                # ✅ Configuração automática
├── docker-compose.yml         # Para desenvolvimento local
├── DEPLOY_RENDER.md           # Este arquivo
└── GerenciadorDeAlunos/
    ├── appsettings.json       # Configuração local
    ├── appsettings.Production.json  # ✅ Configuração produção
    └── Program.cs             # ✅ Configurado para Render
```

## 🚀 Processo de Deploy

1. **Push para repositório**:

   ```bash
   git add .
   git commit -m "Deploy para Render"
   git push origin main
   ```

2. **O Render automaticamente**:

   - Detecta mudanças no repositório
   - Executa `docker build`
   - Faz deploy da nova versão
   - Monitora health checks

3. **URLs resultantes**:
   - **API**: `https://seu-app.onrender.com`
   - **Swagger**: `https://seu-app.onrender.com/swagger`
   - **Health Check**: `https://seu-app.onrender.com/health`

## 🏥 Health Check

O health check está configurado em:

- **Endpoint**: `/health`
- **Intervalo**: 30 segundos
- **Timeout**: 10 segundos
- **Start Period**: 60 segundos (tempo para inicialização)

## 🔍 Monitoramento

### Logs

```bash
# Via Render Dashboard
- Acesse seu Web Service
- Clique na aba "Logs"
- Monitore em tempo real
```

### Métricas

- CPU e memória disponíveis no dashboard
- Tempo de resposta das requisições
- Status de health checks

## 🐛 Troubleshooting

### Erro de Conexão com Banco

```bash
# Verifique:
1. URL do DATABASE_URL está correta
2. Banco PostgreSQL está ativo
3. Firewall/rede permite conexão
```

### Erro de Build

```bash
# Verifique:
1. Dockerfile está na raiz do projeto
2. .dockerignore não está excluindo arquivos necessários
3. Logs de build no dashboard do Render
```

### Erro de Port

```bash
# O Render usa variável $PORT automaticamente
# Não é necessário configurar porta fixa
```

## 🔄 Updates Automáticos

Toda vez que você fizer push para a branch principal:

1. Render detecta mudanças
2. Rebuilda a imagem Docker
3. Faz deploy automático
4. Zero downtime deploy

## 💰 Custos

### Free Tier

- **Web Service**: 750 horas/mês
- **PostgreSQL**: 1GB storage, 1 milhão rows
- **Limitações**: Sleep após 15min inatividade

### Starter Plan ($7/mês)

- **Web Service**: Always-on, sem sleep
- **PostgreSQL**: 256MB RAM, 1GB storage
- **SSL**: Incluído

## 🔐 Segurança

✅ **Implementado**:

- HTTPS automático (Render fornece)
- Variáveis de ambiente seguras
- Headers de proxy configurados
- CORS configurado para produção

## 📝 Próximos Passos

1. ✅ Deploy básico funcionando
2. ⏳ Configurar domínio customizado
3. ⏳ Configurar CI/CD avançado
4. ⏳ Monitoramento com alertas
5. ⏳ Backup automático do banco

## 🎯 URLs Finais

Após o deploy bem-sucedido:

- **Backend API**: `https://gerenciador-alunos-backend.onrender.com`
- **Swagger UI**: `https://gerenciador-alunos-backend.onrender.com/swagger`
- **Health Check**: `https://gerenciador-alunos-backend.onrender.com/health`

### Endpoints da API

```
GET  /api/Student
GET  /api/Discipline
GET  /api/Enrollment/student/{id}
GET  /api/MonthlyPayment/student/{id}
GET  /api/MonthlyPaymentDetail/payment/{id}
```

---

## 🆘 Suporte

Em caso de problemas:

1. Verifique logs no dashboard do Render
2. Teste health check endpoint
3. Verifique variáveis de ambiente
4. Consulte [documentação oficial do Render](https://render.com/docs)
