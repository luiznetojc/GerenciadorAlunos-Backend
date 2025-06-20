# Sistema de Débitos Automáticos - Documentação

## Visão Geral

O sistema possui funcionalidades avançadas para gestão de débitos mensais automáticos com cálculo dinâmico de atraso e geração de relatórios de inadimplência. **Os campos de atraso são calculados automaticamente em tempo real**, eliminando a necessidade de armazenamento redundante e garantindo sempre informações atualizadas.

## Arquitetura Inteligente

### Cálculo Dinâmico de Atraso

- **IsOverdue**: Calculado automaticamente comparando `DateTime.Now` com `DueDate` e status `IsPaid`
- **DaysOverdue**: Calculado automaticamente como `(DateTime.Now - DueDate).Days` quando em atraso
- **StatusDescription**: Gerado dinamicamente ("Pago", "Em dia", "X dias em atraso")

### Vantagens desta Abordagem

- ✅ **Sempre preciso**: Não há risco de dados desatualizados
- ✅ **Performance**: Menos campos na base de dados
- ✅ **Simplicidade**: Sem necessidade de jobs de atualização
- ✅ **Manutenibilidade**: Lógica centralizada no modelo

## Novas Funcionalidades

### 1. Campos Essenciais em MonthlyPayment

- **DueDate**: Data de vencimento do pagamento (padrão: dia 10 do mês)
- **CreatedDate**: Data de criação do débito
- **IsOverdue**: Propriedade calculada automaticamente
- **DaysOverdue**: Propriedade calculada automaticamente

### 2. Endpoints da API

#### Geração de Débitos

- `POST /api/MonthlyBilling/generate/{year}/{month}` - Gera débitos para todos os alunos
- `POST /api/MonthlyBilling/generate/student/{studentId}/{year}/{month}` - Gera débito para um aluno específico
- `POST /api/MonthlyBilling/generate-current-month` - Gera débitos para o mês atual

#### Relatórios (com cálculo dinâmico)

- `GET /api/MonthlyBilling/debt-summary` - Resumo de débitos por aluno
- `GET /api/MonthlyBilling/overdue` - Lista pagamentos em atraso

#### Pagamentos

- `PUT /api/MonthlyPayment/{id}/mark-paid` - Marca um pagamento como pago

## Exemplos de Uso

### 1. Gerar Débitos do Mês Atual

```bash
curl -X POST "http://localhost:5226/api/MonthlyBilling/generate-current-month"
```

### 2. Gerar Débitos para Junho/2025

```bash
curl -X POST "http://localhost:5226/api/MonthlyBilling/generate/2025/6"
```

### 3. Verificar Pagamentos em Atraso

```bash
curl -X GET "http://localhost:5226/api/MonthlyBilling/overdue"
```

### 4. Marcar Pagamento como Pago

```bash
curl -X PUT "http://localhost:5226/api/MonthlyPayment/1/mark-paid"
```

### 5. Obter Resumo de Débitos

```bash
curl -X GET "http://localhost:5226/api/MonthlyBilling/debt-summary"
```

## Automação

### Background Service

O sistema inclui um serviço em background que:

- Executa automaticamente a cada hora
- Atualiza o status de atraso de todos os pagamentos
- Registra logs das operações

### Como Funciona a Geração Automática

1. **Criação do Débito**: Para cada aluno matriculado, o sistema cria um `MonthlyPayment`
2. **Detalhamento**: Para cada disciplina matriculada, cria um `MonthlyPaymentDetail`
3. **Cálculo**: Aplica descontos e calcula o valor total
4. **Vencimento**: Define o vencimento para o dia 10 do mês
5. **Controle de Atraso**: Verifica automaticamente se está em atraso

## Estrutura dos Dados

### MonthlyPaymentResponseDto

```json
{
  "id": 1,
  "studentId": 1,
  "studentName": "João Silva",
  "year": 2025,
  "month": 6,
  "monthName": "junho",
  "totalAmount": 150.0,
  "isPaid": false,
  "paymentDate": null,
  "dueDate": "2025-06-10T00:00:00Z",
  "createdDate": "2025-06-19T10:30:00Z",
  "isOverdue": true,
  "daysOverdue": 9,
  "details": [
    {
      "id": 1,
      "disciplineName": "Matemática",
      "originalAmount": 100.0,
      "discountAmount": 10.0,
      "finalAmount": 90.0
    }
  ]
}
```

### MonthlyPaymentSummaryDto

```json
{
  "studentId": 1,
  "studentName": "João Silva",
  "totalDebt": 300.0,
  "overdueCount": 2,
  "payments": [
    // Lista de pagamentos em aberto
  ]
}
```

## Configuração de Produção

### Variáveis de Ambiente

- `DATABASE_URL`: String de conexão com a base de dados
- `PORT`: Porta da aplicação (padrão: 5226)

### Execução da Migration

```bash
dotnet ef database update
```

## Monitorização

### Logs

O sistema gera logs para:

- Criação de débitos
- Atualização de status de atraso
- Erros de processamento

### Health Check

- Endpoint: `/health`
- Monitoriza o estado da aplicação

## Sugestões de Uso

### 1. Rotina Mensal

- No início de cada mês, execute a geração de débitos
- Acompanhe o relatório de inadimplência

### 2. Notificações (Futuro)

- Implementar sistema de email para notificar atrasos
- SMS para lembretes de vencimento

### 3. Dashboard (Futuro)

- Gráficos de inadimplência
- Relatórios de receitas
- Análise de tendências

## Segurança

- Todos os endpoints são protegidos por autenticação (se configurada)
- Validação de dados de entrada
- Tratamento de erros
- Logs de auditoria
