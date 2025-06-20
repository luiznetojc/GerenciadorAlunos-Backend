# Configuração de Variáveis de Ambiente no Render

## Variáveis Obrigatórias no Render Dashboard

No painel do Render, vá para seu serviço e adicione as seguintes variáveis em:
**Environment** → **Environment Variables**

### 1. String de Conexão do Banco
```
Nome: DefaultConnection
Valor: Host=seu_host;Database=seu_database;Username=seu_user;Password=sua_senha;SSL Mode=Require;Trust Server Certificate=true
```

### 2. Porta (opcional - Render gerencia automaticamente)
```
Nome: PORT
Valor: (gerenciado pelo Render)
```

### 3. Ambiente
```
Nome: ASPNETCORE_ENVIRONMENT
Valor: Production
```

## String de Conexão do Supabase (exemplo)
Para o Supabase, a string segue este formato:
```
Host=db.XXXXXXXXXXXXXXXX.supabase.co;Database=postgres;Username=postgres;Password=SUA_SENHA;SSL Mode=Require;Trust Server Certificate=true
```

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
