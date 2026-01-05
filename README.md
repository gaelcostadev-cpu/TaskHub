# TaskHub .NET — Sistema Colaborativo de Gestão de Tarefas

Sistema de gestão de tarefas colaborativo desenvolvido com foco em arquitetura distribuída em .NET, comunicação assíncrona via RabbitMQ, autenticação JWT e notificações em tempo real através de SignalR. O objetivo do projeto é demonstrar a implementação de um ambiente profissional com múltiplos serviços, integrações consistentes e boas práticas de engenharia de software.

---

## Visão Geral
O TaskHub é uma plataforma para gerenciamento colaborativo de tarefas. Ele permite criação, edição, atribuição e acompanhamento de tarefas em equipes, incluindo comentários, histórico de alterações e notificações em tempo real.

É voltado para:
- Desenvolvedores e equipes que desejam estudar e utilizar um exemplo real de arquitetura distribuída em .NET.
- Ambientes corporativos que buscam organização e rastreabilidade de tarefas.
- Uso educacional ou demonstração técnica de domínio no ecossistema C#, APIs Web e mensageria.

Pode ser utilizado tanto em ambiente de desenvolvimento quanto ajustado para produção.

---

## Arquitetura
A aplicação segue uma arquitetura baseada em microserviços com comunicação assíncrona via RabbitMQ.  
Os serviços são independentes, cada um responsável pelo seu contexto, e um API Gateway centraliza o acesso HTTP externo.

- API Gateway responsável por roteamento, autenticação e documentação.
- Auth Service para cadastro, login, refresh token e segurança.
- Tasks Service para CRUD de tarefas, atribuições, comentários e histórico.
- Notifications Service para consumo de eventos e entrega em tempo real via SignalR.
- Comunicação assíncrona baseada em eventos publicados e consumidos via RabbitMQ.
- Persistência utilizando PostgreSQL.

---

## Tecnologias Utilizadas
- Linguagem: C# (.NET 8)
- Framework: ASP.NET Core Web API
- Banco de Dados: PostgreSQL com Entity Framework Core
- Mensageria: RabbitMQ
- Tempo Real: SignalR
- Versionamento e Documentação: Swagger / OpenAPI
- Infraestrutura: Docker e Docker Compose
- Observabilidade / Logging: Serilog (ou equivalente)

---

## Instalação
Clonar o repositório e acessar a pasta principal do projeto.

```bash
git clone https://github.com/gaelcostadev-cpu/TaskHub.git
cd taskhub
