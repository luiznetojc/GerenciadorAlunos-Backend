# 🚀 CONFIGURAÇÃO RENDER - VARIÁVEIS DE AMBIENTE

## 📋 PASSO A PASSO NO RENDER

### 1. Acesse o Render Dashboard
- Vá para: https://dashboard.render.com
- Selecione seu Web Service

### 2. Configure a Variável de Ambiente
- Clique na aba **Environment**
- Role até **Environment Variables** (não use Environment Groups)
- Clique em **Add Environment Variable**

### 3. Adicione a Variável do Banco
```
Key: DATABASE_URL
Value: postgresql://postgres:@Leleco2025@db.gkkemcwnvcpvnxialucj.supabase.co:5432/postgres
```

### 4. Deploy
- Clique em **Deploy Latest Commit**
- Monitore os logs

## ✅ VERIFICAÇÃO

### Logs Esperados
```
[DEBUG] Connection source: DATABASE_URL
[DEBUG] Original format: PostgreSQL URL
[DEBUG] Converting PostgreSQL URL to .NET connection string...
[DEBUG] Converted to: Host=db.gkkemcwnvcpvnxialucj.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=***;SSL Mode=Require
[DEBUG] Database connection test: SUCCESS
[INFO] Gerenciador de Alunos API iniciada
```

### Endpoints de Teste
- **Health Check**: `https://sua-app.onrender.com/health`
- **Status Detalhado**: `https://sua-app.onrender.com/health/detailed`
- **API Docs**: `https://sua-app.onrender.com/swagger`

### Resposta de Sucesso (/health/detailed)
```json
{
  "status": "healthy",
  "timestamp": "2025-06-24T...",
  "database": "connected",
  "environment": "Production"
}
```

## 🚨 TROUBLESHOOTING

### Se aparecer "FAILED" nos logs:

**Erro de SSL:**
```
[ERROR] SSL connection required
```
**Solução**: Confirmar `?sslmode=require` na URL

**Erro de Credenciais:**
```
[ERROR] password authentication failed
```
**Solução**: Verificar senha `@Leleco2025` no Supabase

**Erro de Host:**
```
[ERROR] could not translate host name
```
**Solução**: Verificar ID do projeto `gkkemcwnvcpvnxialucj`

## 📁 ARQUIVOS DE REFERÊNCIA

- `.env.production` - Variáveis para copiar para o Render
- `DEBUG_RENDER_CONNECTION.md` - Guia de debugging
- `RENDER_ENVIRONMENT_CONFIG.md` - Documentação completa

## 🔐 SEGURANÇA

✅ Nunca commitar arquivos .env  
✅ Usar variáveis de ambiente no Render  
✅ SSL habilitado no Supabase  
✅ Logs não expõem senhas  
