* * *

Desafio Técnico - Plataforma de Pagamentos PagueVeloz
=====================================================

Este projeto é uma implementação do desafio técnico para a vaga de Desenvolvedor(a) .NET na PagueVeloz, focado na construção do núcleo transacional de uma plataforma de pagamentos.

Sumário
-------

*   [Visão Geral do Projeto](#visão-geral-do-projeto)
    
*   [Como Configurar e Rodar o Projeto](#como-configurar-e-rodar-o-projeto)
    
    *   [Pré-requisitos](#pré-requisitos)
        
    *   [Clonar o Repositório](#clonar-o-repositório)
        
    *   [Configuração do Banco de Dados (SQLite)](#configuração-do-banco-de-dados-sqlite)
        
    *   [Rodar a Aplicação](#rodar-a-aplicação)

    * [Rodar a Aplicação com Docker Compose](#rodar-a-aplicação-com-docker-compose)
        
    *   [Acessar a Documentação da API (Swagger)](#acessar-a-documentação-da-api-swagger)
        
*   [Visão Geral da Arquitetura](#visão-geral-da-arquitetura)
    
    *   [Estrutura de Camadas](#estrutura-de-camadas)
        
    *   [Fluxo de Dados (Requisição -> Resposta)](#fluxo-de-dados-requisição---resposta)
        
*   [Principais Decisões de Design e Arquitetura](#principais-decisões-de-design-e-arquitetura)
    
    *   [Arquitetura (Clean Architecture / Hexagonal)](#arquitetura-clean-architecture--hexagonal)
        
    *   [Princípios SOLID](#princípios-solid)
        
    *   [Padrões de Design](#padrões-de-design)
        
    *   [Escolha do Banco de Dados](#escolha-do-banco-de-dados)
        
    *   [Resiliência (Polly)](#resiliência-polly)
        
    *   [Concorrência e Consistência](#concorrência-e-consistência)
        
    *   [Rollback e Idempotência](#rollback-e-idempotência)
        
*   [Como Rodar os Testes](#como-rodar-os-testes)
    
    *   [Testes Unitários](#testes-unitários)
        
    *   [Testes de Integração](#testes-de-integração)
        
    *   [Testes de Concorrência](#testes-de-concorrência)
        

Visão Geral do Projeto
----------------------

Este projeto implementa um sistema financeiro transacional com suporte a clientes, contas bancárias (PF e PJ) e diversas operações financeiras, incluindo vendas (crédito/débito), estornos, transferências e depósitos. O sistema foi projetado com foco em escalabilidade, desempenho e resiliência, utilizando uma arquitetura limpa para facilitar a manutenção e a expansão futura.

As principais funcionalidades implementadas incluem:

*   Criação de Clientes e Contas (uma conta por cliente).
    
*   Gestão de Saldo (disponível, bloqueado, futuro).
    
*   Transações de Venda (Débito, Crédito à Vista, Crédito Parcelado).
    
*   Transações de Estorno (com validação de saldo disponível).
    
*   Transações de Transferência (apenas entre contas ativas).
    
*   Transações de Depósito (para simular entrada de fundos).
    
*   Consulta de Saldo e Histórico de Transações por Conta.
    

Como Configurar e Rodar o Projeto
---------------------------------

### Pré-requisitos

*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
    
*   Um editor de código como [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) ou [VS Code](https://code.visualstudio.com/)

* [Docker Desktop](https://www.docker.com/products/docker-desktop/) (necessário para a execução via Docker Compose)   

### Clonar o Repositório

Bash

    git clone https://github.com/uines/PagueVeloz.Challenge.git
    cd PagueVeloz.Challenge

### Configuração do Banco de Dados (SQLite)

O projeto utiliza **SQLite** para persistência, o que simplifica a configuração, pois o banco de dados é um arquivo (`.db`) gerado localmente.

1.  **Restaurar Pacotes NuGet:**
    
    Bash
    
        dotnet restore
    
2.  **Aplicar Migrações do Entity Framework Core:** As migrações criarão o arquivo de banco de dados (`PagueVelozChallenge.db`) e as tabelas necessárias no diretório do projeto `PagueVeloz.Challenge.Api`.
    
    Bash
    
        dotnet ef database update --project PagueVeloz.Challenge.Infrastructure --startup-project PagueVeloz.Challenge.Api
    

### Rodar a Aplicação

Navegue até o diretório do projeto da API e execute:

Bash

    cd PagueVeloz.Challenge.Api
    dotnet run

A aplicação será iniciada, geralmente na porta `5000` (HTTP) e `5001` (HTTPS). Você verá as URLs no console.

### Rodar a Aplicação com Docker Compose
Esta é a forma recomendada para executar a aplicação, pois encapsula todas as dependências e o banco de dados em contêineres, proporcionando um ambiente isolado e consistente.

1. **Certifique-se de estar na raiz da solução** (onde está o arquivo docker-compose.yml):
    Bash
        
        cd PagueVeloz.Challenge

2. **Construa e inicie os contêineres:**  
    Bash 

        docker compose up --build -d

    * `--build`: Garante que as imagens Docker sejam construídas (ou reconstruídas se houver alterações no código ou Dockerfile).
    * `-d`: Inicia os contêineres em modo `detached` (em segundo plano).

3. **Verificar o status dos contêineres (opcional):**
    Bash
        docker compose ps

4. **Para parar e remover os contêineres:**

    * O volume de dados do SQLite (`db_data`) será persistido, então seus dados não se perderão entre as execuções de `down` e `up`.
    * Para remover também o volume de dados (e resetar o DB), use `docker compose down -v`.

### Acessar a Documentação da API (Swagger)

Com a aplicação rodando, abra seu navegador e acesse: `https://localhost:<porta_https>/swagger` (ex: `https://localhost:5001/swagger`)

Você poderá interagir com os endpoints da API (Criar Cliente, Realizar Venda, Depósito, Transferência, Estorno, Consultar Saldo/Histórico) diretamente pelo Swagger UI.

Visão Geral da Arquitetura
--------------------------

O projeto segue a

**Clean Architecture (ou Arquitetura Hexagonal)**, um padrão que promove a separação de preocupações e o isolamento da lógica de negócio de detalhes de infraestrutura e frameworks.

### Estrutura de Camadas

A solução é organizada em múltiplos projetos para representar as camadas da arquitetura:

*   **`PagueVeloz.Challenge.Domain`**:
    
    *   **Coração da Aplicação:** Contém as entidades de negócio (`Cliente`, `Conta`, `Transacao`), seus comportamentos, enumeradores (`TipoCliente`, `TipoConta`, `TipoTransacao`, `StatusConta`, `StatusTransacao`), exceções de domínio e as **interfaces** de repositório (`IClienteRepository`, `IContaRepository`, `ITransacaoRepository`, `IUnitOfWork`).
        
    *   **Regra:** Não possui dependências de nenhum outro projeto da solução, garantindo que o domínio seja puro e independente.
        
*   **`PagueVeloz.Challenge.Application`**:
    
    *   **Casos de Uso da Aplicação:** Define os casos de uso do sistema através de Comandos (para modificações de estado) e Queries (para consultas de dados). Contém DTOs (Data Transfer Objects), Comandos, Queries e Handlers correspondentes.
        
    *   **Regra:** Depende apenas de `PagueVeloz.Challenge.Domain`. Orquestra a lógica de negócio utilizando as entidades de domínio e as interfaces de repositório.
        
*   **`PagueVeloz.Challenge.Infrastructure`**:
    
    *   **Implementações de Infraestrutura:** Contém as implementações concretas das interfaces de repositório (usando Entity Framework Core para SQLite), o `DbContext`, a configuração do banco de dados e as políticas de resiliência (Polly).
        
    *   **Regra:** Depende de `PagueVeloz.Challenge.Domain` e `PagueVeloz.Challenge.Application`.
        
*   **`PagueVeloz.Challenge.Api`**:
    
    *   **Ponto de Entrada:** É a camada de apresentação que expõe a funcionalidade do sistema através de uma API RESTful usando ASP.NET Core Web API. Contém os controladores e a configuração da injeção de dependência.
        
    *   **Regra:** Depende de `PagueVeloz.Challenge.Application`. A comunicação com as camadas inferiores é feita através da injeção de dependência dos Handlers.
        
*   **`PagueVeloz.Challenge.Tests`**:
    
    *   **Testes Automatizados:** Contém testes unitários (focados no `Domain` e `Application` com mocks) e testes de integração (validando a interação entre camadas e a persistência no banco de dados).
        

### Fluxo de Dados (Requisição -> Resposta)

1.  **Requisição HTTP (API):** Uma requisição chega ao `PagueVeloz.Challenge.Api` (Controller).
    
2.  **Validação (API/Application):** O Controller recebe um DTO e, através do FluentValidation, valida os dados de entrada.
    
3.  **Comando/Query (Application):** O DTO é mapeado para um Comando (para ações de escrita) ou Query (para ações de leitura).
    
4.  **Handler (Application):** O Controller invoca o Handler correspondente. O Handler contém a lógica de negócio para o caso de uso, utilizando entidades de domínio e interfaces de repositório.
    
5.  **Domínio (Domain):** As entidades de domínio aplicam suas regras de negócio e manipulam seu estado.
    
6.  **Repositório (Infrastructure):** O Handler invoca as interfaces de repositório para persistir ou recuperar dados. As implementações de repositório (`PagueVeloz.Challenge.Infrastructure`) usam EF Core para interagir com o banco de dados, aplicando políticas de resiliência (Polly) nas operações de I/O.
    
7.  **Unidade de Trabalho (Infrastructure):** O `IUnitOfWork` gerencia a transação de banco de dados, garantindo que todas as operações sejam salvas atomicamente (ou tudo, ou nada).
    
8.  **Resposta (Application -> API):** Após o processamento, o Handler retorna um DTO de resposta para o Controller, que o envia de volta como uma resposta HTTP.
    

Principais Decisões de Design e Arquitetura
-------------------------------------------

### Arquitetura (Clean Architecture / Hexagonal)

*   **Escolha:** Optou-se pela Clean Architecture para garantir alta testabilidade, separação de preocupações, manutenibilidade e escalabilidade. Isso prepara o terreno para uma possível evolução para microsserviços.
    
*   **Benefícios:** Isolamento do domínio, independência de frameworks, facilidade para adicionar novas funcionalidades e substituir tecnologias.
    

### Princípios SOLID

Os princípios SOLID foram aplicados ao longo do design:

*   **SRP (Single Responsibility Principle):** Cada classe e módulo tem uma responsabilidade bem definida (ex: `Conta` gerencia saldo, `RealizarVendaCommandHandler` processa vendas).
    
*   **OCP (Open/Closed Principle):** Novas transações (ex: um novo tipo de pagamento) podem ser adicionadas criando novos comandos e handlers, sem modificar os componentes existentes do core transacional.
    
*   **DIP (Dependency Inversion Principle):** As camadas superiores (`Application`, `Api`) dependem de abstrações (interfaces de repositório definidas no `Domain`), não de implementações concretas (`Infrastructure`). Isso é habilitado pela Injeção de Dependência.
    

### Padrões de Design

*   **Command-Query Responsibility Segregation (CQRS):** Separa explicitamente as operações de leitura (Queries) das operações de escrita (Commands), cada uma com seus próprios DTOs e Handlers. Isso otimiza o desempenho e a complexidade de cada fluxo.
    
*   **Repositório e Unit of Work:** Abstraem a camada de persistência, permitindo que a lógica de negócio trabalhe com coleções de objetos e garanta a atomicidade das transações.
    
*   **Injeção de Dependência:** Utilizada em toda a aplicação para gerenciar as dependências entre as classes, promovendo baixo acoplamento.
    

### Escolha do Banco de Dados

*   **SQLite:** Escolhido por sua simplicidade e facilidade de configuração para um desafio técnico. Não exige um servidor de banco de dados separado, tornando a configuração e execução do projeto muito simples.
    
*   **Entity Framework Core:** Como ORM, facilita a interação com o banco de dados através de objetos C# e o gerenciamento de esquemas via migrações.
    

### Resiliência (Polly)

*   **Contexto:** Em um sistema de alto volume e sensível a falhas, a resiliência é fundamental.
    
*   **Implementação:** A biblioteca Polly foi integrada na camada `Infrastructure` para aplicar políticas de resiliência nas operações que interagem com o banco de dados:
    
    *   **Retry:** Configurado para re-tentar operações de banco de dados que falham temporariamente (ex: `DbUpdateException` devido a bloqueios ou deadlocks).
        
    *   **Fallback:** Um mecanismo de fallback é definido para lidar com falhas não recuperáveis, permitindo que o sistema responda de forma controlada (ex: logando o erro e talvez retornando um status de falha apropriado).
        

### Concorrência e Consistência

*   **Regra de Negócio:** "Operações concorrentes na mesma conta deve ser bloqueadas. Garantir consistência nos eventuais lock."
    
*   **Estratégia Adotada (SQLite/EF Core):**
    
    *   **Atomicidade do `UnitOfWork`:** As operações críticas de saldo (débito/crédito) e o registro da transação são encapsulados dentro de uma única transação de banco de dados (`_unitOfWork.Commit()`). O Entity Framework Core, ao usar `SaveChanges()`, garante que todas as alterações rastreadas sejam aplicadas atomicamente. Para SQLite, isso implica em bloqueio a nível de arquivo durante a escrita, prevenindo a corrupção de dados por escritas simultâneas.
        
    *   **Validações Internas:** A lógica de domínio nas entidades (ex: `Conta.Debitar()` verificando `SaldoDisponivel`) é a primeira linha de defesa contra estados inválidos.
        
    *   **Retry com Polly:** Ajuda a lidar com falhas de concorrência que podem resultar em exceções temporárias (como `DbUpdateException`), re-tentando a operação e permitindo que a transação seja bem-sucedida após o conflito inicial.
        
*   **Testes de Concorrência:** O teste de concorrência (`ConcorrenciaTests.cs` no `IntegrationTests`) simula múltiplas requisições simultâneas na mesma conta para validar se o sistema mantém a consistência do saldo e lida com os conflitos corretamente (via exceções e retries).
    

### Rollback e Idempotência

*   **Rollback:** Garantido pela atomicidade das transações de banco de dados gerenciadas pelo `UnitOfWork`. Se qualquer parte de uma operação falhar durante o
    
    `Commit()`, todas as alterações são revertidas, deixando o banco de dados em um estado consistente.
    
*   **Idempotência:**
    
    *   **IDs Únicos (GUIDs):** O uso de `Guid` para identificadores de Cliente, Conta e Transação garante que a criação de novos recursos seja idempotente em um nível básico (tentar criar o mesmo recurso com o mesmo ID resultaria em um erro de violação de chave primária, ou a lógica de negócio impede a duplicação, como o documento do cliente).
        
    *   **Estornos:** O `RealizarEstornoCommandHandler` verifica o status da `TransacaoOriginal` antes de processar um estorno, evitando múltiplos estornos para a mesma transação já cancelada/estornada. Isso torna a operação de estorno idempotente no nível da lógica de negócio.
        
    *   **Melhoria Futura:** Para idempotência robusta de requisições API em sistemas distribuídos, seria ideal implementar uma `Idempotency-Key` no cabeçalho da requisição, rastreada por uma tabela de idempotência no banco de dados.
        

Como Rodar os Testes
--------------------

Os testes são cruciais para garantir a qualidade e a funcionalidade do sistema. Eles estão divididos em Unitários e de Integração.

1.  **Navegue até a raiz da solução:**
    
    Bash
    
        cd PagueVeloz.Challenge
    
2.  **Execute todos os testes:**
    
    Bash
    
        dotnet test
    
    Isso executará todos os testes definidos no projeto `PagueVeloz.Challenge.Tests`.
    

### Testes Unitários

*   **Localização:** `PagueVeloz.Challenge.Tests/UnitTests/`
    
*   **Foco:** Testam unidades isoladas de código (entidades de domínio, handlers de aplicação) em isolamento, usando mocks (Moq) para simular dependências (repositórios, etc.).
    
*   **Exemplos:** Testes para métodos da `Conta` (creditar, debitar), e para `CriarClienteCommandHandler`.
    

### Testes de Integração

*   **Localização:** `PagueVeloz.Challenge.Tests/IntegrationTests/`
    
*   **Foco:** Validam a interação entre as camadas (Application, Infrastructure) e a persistência de dados. Utilizam um banco de dados **SQLite em memória** para garantir que sejam rápidos, isolados e que as operações de banco de dados funcionem como esperado.
    
*   **Exemplos:** Testes para o fluxo completo de criação de cliente, depósitos, vendas, transferências e estornos, verificando o estado final do banco de dados.
    

### Testes de Concorrência

*   **Localização:** `PagueVeloz.Challenge.Tests/IntegrationTests/ConcorrenciaTests.cs` (se implementado conforme discutido).
    
*   **Foco:** Simulam cenários de acesso simultâneo à mesma conta ou recurso para verificar se o controle de concorrência (transações de banco de dados, retries do Polly) funciona corretamente para manter a consistência e evitar `race conditions`.
    
*   **Execução:** Esses testes envolvem a execução de múltiplas tarefas `async` em paralelo para forçar conflitos e observar o comportamento do sistema.
    

* * *
