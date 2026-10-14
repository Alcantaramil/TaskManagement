# TaskManagement — API de Gerenciamento de Tarefas

API REST desenvolvida em **C# / ASP.NET Core (.NET 8)** para cadastrar, consultar, atualizar e excluir tarefas, com filtros por status e data de vencimento.

## Funcionalidades

- Criar uma tarefa com título, descrição, status e data de vencimento.
- Listar tarefas e consultar uma tarefa pelo ID.
- Filtrar tarefas por **status**, **data de vencimento** ou ambos.
- Atualizar e excluir tarefas.
- Validar os dados de entrada e retornar códigos HTTP adequados.
- Explorar e testar os endpoints com Swagger em ambiente de desenvolvimento.

### Status disponíveis

| Valor | Status |
|---|---|
| `0` | Pendente |
| `1` | Em andamento |
| `2` | Concluído |

## Tecnologias e arquitetura

- .NET 8, C# e ASP.NET Core Web API
- Entity Framework Core com provider Npgsql
- PostgreSQL 16 no Docker Compose
- Padrão Repository e camada Service, com injeção de dependência
- DTOs para entrada e saída de dados
- Swagger / OpenAPI
- xUnit, Moq e Coverlet para testes unitários e cobertura
- Docker e Docker Compose

A aplicação segue o fluxo **Controller → Service → Repository → Entity Framework Core → PostgreSQL**.

## Estrutura do projeto

```text
TaskManagement/
├── Controllers/
│   └── TarefaController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── DTOs/
│   ├── TarefaRequestDto.cs
│   └── TarefaResponseDto.cs
├── Entities/
│   └── Tarefa.cs
├── Enum/
│   └── EnumStatus.cs
├── Helpers/
│   └── EnumExtensions.cs
├── Repositories/
│   ├── ITarefaRepository.cs
│   └── TarefaRepository.cs
├── Services/
│   ├── ITarefaServices.cs
│   └── TarefaServices.cs
├── docker/
│   └── init.sql
├── Program.cs
├── TaskManagement.csproj
├── Dockerfile
├── docker-compose.yml
├── .dockerignore
└── .gitignore

├── TaskManagement.Tests/
│   ├── TaskManagement.Tests.csproj
│   └── ...
├── .env.example
├── TaskManagement.sln
├── README.md
```


## Pré-requisitos

Para a execução recomendada, instale **Docker Desktop** com suporte a contêineres Linux (no Windows, WSL 2). Não é necessário instalar o PostgreSQL localmente para executar via Docker Compose.

Para executar sem Docker, é necessário o **SDK .NET 8** e uma instância PostgreSQL configurada.

## Executar com Docker Compose

1. Crie o arquivo `.env` a partir do exemplo fornecido:

   ```powershell
   Copy-Item .env.example .env
   ```

   Depois, abra o arquivo `.env` e substitua a senha de exemplo por uma senha forte:

   ```dotenv
   POSTGRES_PASSWORD=substitua_por_uma_senha_forte
   ```

   **Não versione o arquivo `.env` nem compartilhe senhas reais.**

2. Construa e inicie os serviços:

   ```bash
   docker compose up --build -d
   ```

3. Confira os contêineres:

   ```bash
   docker compose ps
   ```

4. Abra o Swagger em **http://localhost:8080/swagger**.

5. Para acompanhar logs ou encerrar os serviços:

   ```bash
   docker compose logs -f api
   docker compose down
   ```

O serviço `postgres` usa a porta interna `5432` e, na configuração atual, publica a porta `5433` no computador. A API utiliza a porta `8080`.

O script `docker/init.sql` cria a tabela `tarefas` **na primeira inicialização de um volume de dados novo**. O volume `postgres_data` preserva os registros após reinicializações e após `docker compose down` sem `-v`. **Atenção:** `docker compose down -v` remove o volume e apaga os dados armazenados nele.

O Compose define `ASPNETCORE_ENVIRONMENT=Development` para disponibilizar o Swagger durante a avaliação local. Essa configuração e o gerenciamento de segredos devem ser revistos antes de publicar a aplicação em produção.

## Executar localmente sem Docker

1. Configure uma instância do PostgreSQL e crie a tabela conforme `docker/init.sql`.
2. Configure `ConnectionStrings:DefaultConnection` por meio de variável de ambiente, User Secrets ou configuração local não versionada.
3. Na pasta do projeto, execute:

   ```bash
   dotnet restore
   dotnet run
   ```

4. Acesse o endereço Swagger indicado na saída da aplicação, em ambiente `Development`.

## Endpoints

| Método | Rota | Descrição | Resposta de sucesso |
|---|---|---|---|
| `GET` | `/api/Tarefa` | Lista tarefas; aceita filtros opcionais | `200 OK` |
| `GET` | `/api/Tarefa/{id}` | Consulta uma tarefa | `200 OK` |
| `POST` | `/api/Tarefa` | Cria uma tarefa | `201 Created` |
| `PUT` | `/api/Tarefa/{id}` | Atualiza uma tarefa | `204 No Content` |
| `DELETE` | `/api/Tarefa/{id}` | Exclui uma tarefa | `204 No Content` |

**Filtros opcionais da listagem:** `status` (0, 1 ou 2) e `dataVencimento` (`AAAA-MM-DD`). Exemplo:

```http
GET /api/Tarefa?status=0&dataVencimento=2026-11-30
```

**Exemplo de cadastro:**

```http
POST /api/Tarefa
Content-Type: application/json
```

```json
{
  "titulo": "Estudar .NET",
  "descricao": "Revisar testes unitários",
  "status": 0,
  "dataVencimento": "2026-11-30"
}
```

A API valida os campos obrigatórios e os valores aceitos para `status`. Requisições inválidas retornam `400 Bad Request`; consultas, atualizações ou exclusões de tarefas inexistentes retornam `404 Not Found`.

## Testes unitários e cobertura

Foram executados **9 testes unitários com xUnit e Moq**, todos aprovados. Os testes cobrem os métodos públicos da camada `TarefaService`, incluindo cenários de sucesso e de tarefa inexistente.

A coleta com Coverlet e o relatório com ReportGenerator indicaram, na execução realizada, **100% de cobertura de linhas e 100% de cobertura de ramificações da classe `TarefaService`**. Esses percentuais **não representam a cobertura da API inteira**.

Depois de incluir o projeto `TaskManagement.Tests` no repositório e na solução, execute na pasta da solução:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Para gerar um relatório HTML, instale o ReportGenerator e aponte para o arquivo `coverage.cobertura.xml` produzido na pasta `TestResults` do projeto de testes:

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator "-reports:**/TestResults/**/coverage.cobertura.xml" "-targetdir:CoverageReport" "-reporttypes:Html"
```

Abra `CoverageReport/index.html` no navegador.

## Observações de segurança e entrega

- Não publique `.env`, credenciais reais ou configurações locais contendo senhas.
- Não versione pastas geradas (`bin`, `obj`, `.vs`, `TestResults`, `CoverageReport`).
- Confirme que o projeto `TaskManagement.Tests` está incluído no repositório antes de entregar o bônus de testes.
- A configuração Docker apresentada é destinada a desenvolvimento e avaliação local.
