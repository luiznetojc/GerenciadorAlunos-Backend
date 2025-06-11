# 🔐 Configuração Local - Dados Sensíveis

## 📋 Arquivos Ignorados pelo Git

Os seguintes arquivos contêm dados sensíveis e estão no `.gitignore`:

- `appsettings.json` - Configurações com connection string real
- `appsettings.Production.json` - Configurações de produção
- `docker-compose.yml` - Docker compose com credenciais

## 🚀 Setup Local

### 1. Configurar appsettings.json

Copie o template e configure com seus dados:

```bash
cp GerenciadorDeAlunos/appsettings.template.json GerenciadorDeAlunos/appsettings.json
```

Edite o arquivo `appsettings.json` com suas credenciais reais:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.gkkemcwnvcpvnxialucj.supabase.co;Database=postgres;Username=postgres;Password=@Leleco2025;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 2. Configurar Docker Compose

Copie o template:

```bash
cp docker-compose.template.yml docker-compose.yml
```

Edite conforme necessário ou use variáveis de ambiente.

## 🌍 Variáveis de Ambiente

Alternativamente, você pode usar variáveis de ambiente:

```bash
export DATABASE_URL="Host=db.gkkemcwnvcpvnxialucj.supabase.co;Database=postgres;Username=postgres;Password=@Leleco2025;SSL Mode=Require;Trust Server Certificate=true"
```

## 🚀 Deploy no Render

Para deploy no Render:

1. **Os templates são commitados** (sem dados sensíveis)
2. **Configure variáveis de ambiente no Render:**
   - `DATABASE_URL` - Sua connection string do PostgreSQL
   - `ASPNETCORE_ENVIRONMENT=Production`

## 🔒 Segurança

- ✅ Dados sensíveis ficam apenas na sua máquina
- ✅ Templates são seguros para commit
- ✅ Render usa variáveis de ambiente
- ✅ Sem credenciais no repositório

## 📝 Arquivos Template Disponíveis

- `appsettings.template.json` - Template das configurações
- `docker-compose.template.yml` - Template do docker compose
- `.env.example` - Exemplo de variáveis de ambiente
