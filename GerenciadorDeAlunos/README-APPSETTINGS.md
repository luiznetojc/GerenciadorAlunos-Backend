# 🔧 CONFIGURAÇÃO DOS APPSETTINGS

## 📁 Arquivos Ignorados pelo Git

Os seguintes arquivos contêm dados sensíveis e estão no `.gitignore`:

- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Production.json`
- `appsettings.Staging.json`
- `appsettings.*.json`

## 🚀 Como Configurar

### 1. Para Desenvolvimento Local

Copie o arquivo template e configure com sua senha:

```bash
cp appsettings.template.json appsettings.json
```

Em seguida, edite `appsettings.json` e substitua `[YOUR-PASSWORD]` pela senha real do Supabase.

### 2. Para Produção (Render)

Configure a variável de ambiente no Render Dashboard:

```
Key: DATABASE_URL
Value: postgresql://postgres:SuaSenhaReal@db.gkkemcwnvcpvnxialucj.supabase.co:5432/postgres
```

## 📋 Template de Configuração

O arquivo `appsettings.template.json` contém:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.gkkemcwnvcpvnxialucj.supabase.co;Database=postgres;Username=postgres;Password=[YOUR-PASSWORD];SSL Mode=Require;Trust Server Certificate=true"
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

## ⚠️ IMPORTANTE

- **NUNCA** commitar arquivos `appsettings.json` com senhas reais
- **SEMPRE** usar `[YOUR-PASSWORD]` como placeholder nos templates
- **SUBSTITUIR** pela senha real apenas nos arquivos locais
- **CONFIGURAR** variáveis de ambiente para produção

## 🔐 Segurança

✅ Arquivos sensíveis no `.gitignore`  
✅ Templates públicos sem credenciais  
✅ Variáveis de ambiente para produção  
✅ SSL obrigatório para conexões

Para mais informações, consulte `SECURITY_CONFIG.md`
