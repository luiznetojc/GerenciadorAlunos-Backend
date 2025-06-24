# Configuração de Variáveis de Ambiente no Render

# Configuração de Variáveis de Ambiente no Render

## Variáveis Obrigatórias no Render Dashboard

No painel do Render, vá para seu serviço e adicione as seguintes variáveis em:
**Environment** → **Environment Variables**

### Opção 1: DATABASE_URL (Recomendado pelo Render)

```
Nome: DATABASE_URL
Valor: postgresql://usuario:senha@host:porta/database?sslmode=require
```

### Opção 2: DefaultConnection (Formato .NET)

```
Nome: DefaultConnection
Valor: Host=seu_host;Database=seu_database;Username=seu_user;Password=sua_senha;SSL Mode=Require;Trust Server Certificate=true
```

### Opção 3: CONNECTION_STRING (Alternativa)

```
Nome: CONNECTION_STRING
Valor: Host=seu_host;Database=seu_database;Username=seu_user;Password=sua_senha;SSL Mode=Require;Trust Server Certificate=true
```

## Variáveis Automáticas do Render

O Render define automaticamente:

```
PORT - Porta da aplicação (gerenciado automaticamente)
RENDER - indica que está rodando no Render
```

## Formato da String de Conexão

### Para PostgreSQL/Supabase (Formato .NET):

```
Host=db.xxxxxxxx.supabase.co;Database=postgres;Username=postgres;Password=sua_senha;SSL Mode=Require;Trust Server Certificate=true
```

### Para PostgreSQL (Formato URL - DATABASE_URL):

```
postgresql://postgres:[YOUR-PASSWORD]@db.gkkemcwnvcpvnxialucj.supabase.co:5432/postgres
```

## ✅ SOLUÇÃO: Como Configurar no Render

### PASSO 1: No Render Dashboard

1. Vá para seu Web Service
2. Clique na aba **Environment**
3. Role até **Environment Variables** (NÃO use Environment Groups)
4. Clique em **Add Environment Variable**

### PASSO 2: Adicione UMA das opções abaixo:

**OPÇÃO A - DATABASE_URL (Recomendado):**

```
Key: DATABASE_URL
Value: postgresql://postgres:@Leleco2025@db.gkkemcwnvcpvnxialucj.supabase.co:5432/postgres?sslmode=require
```

**OPÇÃO B - DefaultConnection:**

```
Key: DefaultConnection
Value: Host=db.gkkemcwnvcpvnxialucj.supabase.co;Database=postgres;Username=postgres;Password=@Leleco2025;SSL Mode=Require;Trust Server Certificate=true
```

### PASSO 3: Deploy

Após adicionar a variável, clique em **Deploy Latest Commit**

## ⚠️ IMPORTANTE

- **NÃO use Environment Groups**
- **Use apenas Environment Variables direto no serviço**
- **Escolha apenas UMA das opções acima**
- **O sistema agora suporta ambos os formatos automaticamente**

## Configuração Local para Desenvolvimento

Para desenvolvimento local, edite o arquivo `appsettings.json` com sua configuração local:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=GerenciamentoAlunos;Username=postgres;Password=sua_senha_local"
  }
}
```

## Como o Sistema Funciona

1. **Em Produção (Render)**: O sistema usa a variável de ambiente `DefaultConnection`
2. **Em Desenvolvimento**: O sistema usa o valor do `appsettings.json`
3. **Fallback**: Se não encontrar nenhuma configuração, o sistema lança uma exceção clara

## Segurança

- ✅ As variáveis de ambiente no Render são criptografadas
- ✅ Não há credenciais hardcoded no código
- ✅ Arquivos de configuração sensíveis são ignorados pelo Git
- ✅ Logs de produção são minimizados para evitar exposição de dados
