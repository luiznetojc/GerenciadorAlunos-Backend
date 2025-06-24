# 🚨 TROUBLESHOOTING - Problemas de Conectividade Supabase

## 🔍 Erro Específico

```
System.InvalidOperationException: An exception has been raised that is likely due to a transient failure.
Npgsql.NpgsqlException: Failed to connect to [IPv6]:5432
System.Net.Sockets.SocketException: Network is unreachable
```

## 🛠️ Soluções Implementadas

### 1. Connection String Otimizada

```csharp
Host=db.gkkemcwnvcpvnxialucj.supabase.co;
Database=postgres;
Username=postgres;
Password=SUA_SENHA;
Port=5432;
SSL Mode=Require;
Trust Server Certificate=true;
Pooling=true;
MinPoolSize=0;
MaxPoolSize=100;
Connection Idle Lifetime=300;
Connection Pruning Interval=10;
Server Compatibility Mode=NoTypeLoading;
Timeout=60;
Command Timeout=60
```

### 2. Configurações Específicas para Supabase

**Pooling**: Gerencia conexões de forma eficiente  
**Connection Idle Lifetime**: Evita timeouts em conexões inativas  
**Server Compatibility Mode**: Resolve problemas de compatibilidade  
**Timeout**: Aguarda mais tempo para conexões lentas

## 🔧 Verificações Manuais

### 1. Testar Conectividade

```bash
# Teste de ping para o Supabase
ping db.gkkemcwnvcpvnxialucj.supabase.co

# Teste de porta
telnet db.gkkemcwnvcpvnxialucj.supabase.co 5432
```

### 2. Verificar Status do Supabase

- Acesse: https://status.supabase.com/
- Verifique se há problemas na região do seu projeto

### 3. Verificar Configurações do Projeto

- Supabase Dashboard → Settings → Database
- Confirme se as configurações estão corretas
- Verifique se o projeto não está pausado

## 🌐 Problemas de Rede Comuns

### IPv6 vs IPv4

O erro mostra tentativa de conexão IPv6. Soluções:

- Connection string força IPv4 com configurações específicas
- Supabase às vezes tem problemas com IPv6

### Firewall/Proxy

- Verifique se não há bloqueio na porta 5432
- Alguns provedores bloqueiam conexões PostgreSQL

### Timeout de Rede

- Configuramos timeouts maiores (60s)
- Connection pooling reduz problemas de conectividade

## 🔄 Se o Problema Persistir

### 1. Recrear Connection String

Use apenas o essencial:

```
Host=db.gkkemcwnvcpvnxialucj.supabase.co;Database=postgres;Username=postgres;Password=SUA_SENHA;SSL Mode=Require;Trust Server Certificate=true
```

### 2. Verificar Logs do Supabase

- Dashboard → Logs
- Procure por erros de conexão

### 3. Testar com Cliente PostgreSQL

```bash
psql postgresql://postgres:SUA_SENHA@db.gkkemcwnvcpvnxialucj.supabase.co:5432/postgres
```

### 4. Contato Supabase

Se nada funcionar, pode ser problema na infraestrutura do Supabase.

## ✅ Configuração Atual

A configuração foi otimizada especificamente para resolver problemas de conectividade com Supabase, incluindo:

- Pool de conexões configurado
- Timeouts adequados
- Compatibilidade de servidor
- SSL obrigatório
- Configurações de pruning de conexão
