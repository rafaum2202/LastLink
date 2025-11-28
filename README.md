# 📌 LastLink – API de Solicitação de Antecipações  
API REST criada por Rafael Aguiar Rodrigues em .NET 8 utilizando arquitetura limpa (Domain → Application → Infrastructure → API), permitindo que Creators solicitem antecipações com cálculo automático de valor líquido, controle de status e versionamento de endpoints.

## 🚀 Executando o Projeto
### 1. Abrir a pasta onde extraiu o arquivo .zip
cd LastLink

### 2. Restaurar dependências
dotnet restore

### 3. Buildar a solução
dotnet build


### 3. Rodar a API
cd src/LastLink.API
dotnet run

A API iniciará em:
https://localhost:7239/swagger
http://localhost:5213/swagger

## 📘 Swagger + Versionamento
Acesse via:
/swagger/v1/swagger.json
/swagger

## 📄 Modelo de Dados
{
  "id": "guid",
  "creatorId": "string",
  "valorSolicitado": 150.00,
  "valorLiquido": 142.50,
  "status": "Pendente",
  "dataSolicitacao": "2025-11-27T01:46:18"
}

## 🧮 Regras de Negócio
- Valor mínimo: R$ 100
- Apenas 1 solicitação pendente por Creator
- Taxa fixa: 5%
- Simulação sem persistência
- Status permitido para update: Aprovada/Recusada

## 🔥 Endpoints
POST /api/v1/anticipations  
GET /api/v1/anticipations/creator/{creatorId}  
POST /api/v1/anticipations/simulate  
PUT /api/v1/anticipations/{id}/status/{newStatus}

## 🧪 Testes Automatizados
Para rodar:
dotnet test

Cobrem:
- Criação
- Validações
- Atualização de status
- Simulações
- Cenários de erro

## 🗂️ Tecnologias
- .NET 8  
- EF Core InMemory  
- xUnit  
- Moq  
- FluentResults
- FluentValidators
- Swagger
- Logs na camada de Middlewares com id de correlação

