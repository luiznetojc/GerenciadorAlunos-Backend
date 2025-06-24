# 🐛 DEBUG - Problemas de Conexão no Render

## 📊 Logs Implementados

O sistema agora possui logs detalhados que vão aparecer no Render:

```
[DEBUG] Connection source: DATABASE_URL
[DEBUG] Original format: PostgreSQL URL
[DEBUG] Converting PostgreSQL URL to .NET connection string...
[DEBUG] Converted to: Host=db.xxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=***;SSL Mode=Require
[DEBUG] Converted: true
[DEBUG] Final connection ready: true
[DEBUG] Testing database connection...
[DEBUG] Database connection test: SUCCESS/FAILED
```

## 🔍 Como Verificar no Render

### 1. Ver Logs de Deploy
- Vá para seu serviço no Render
- Clique em **Logs**
- Procure pelos logs `[DEBUG]` acima

### 2. Testar Health Check
Acesse estas URLs para verificar o status:
- `https://sua-app.onrender.com/health` - Health check básico
- `https://sua-app.onrender.com/health/detailed` - Status detalhado do banco

### 3. Verificar Variáveis de Ambiente
No painel do Render, confirme que você tem:
```
DATABASE_URL = postgresql://postgres:sua_senha@db.xxx.supabase.co:5432/postgres?sslmode=require
```

## 🚨 Possíveis Problemas

### Problema 1: String de Conexão Incorreta
**Log esperado**: `[ERROR] Failed to convert DATABASE_URL`
**Solução**: Verificar formato da URL do PostgreSQL

### Problema 2: Banco Inacessível
**Log esperado**: `[ERROR] Database connection error`
**Soluções**:
- Verificar se o Supabase está funcionando
- Confirmar credenciais
- Verificar regras de firewall

### Problema 3: SSL/TLS
**Log esperado**: `SSL connection required`
**Solução**: Garantir que `sslmode=require` está na URL

### Problema 4: Timeout
**Log esperado**: `timeout expired`
**Solução**: Problema de rede entre Render e Supabase

## 🔧 Formato Correto da URL

Para Supabase, use exatamente este formato:
```
postgresql://postgres:SUA_SENHA@db.PROJETO_ID.supabase.co:5432/postgres?sslmode=require
```

Substitua:
- `SUA_SENHA` pela senha do seu projeto
- `PROJETO_ID` pelo ID do seu projeto Supabase

## ✅ Como Confirmar que Funcionou

1. **Logs de sucesso**:
   ```
   [DEBUG] Database connection test: SUCCESS
   [INFO] Gerenciador de Alunos API iniciada
   ```

2. **Health check retorna**:
   ```json
   {
     "status": "healthy",
     "database": "connected"
   }
   ```

3. **API responde** em `/swagger`
