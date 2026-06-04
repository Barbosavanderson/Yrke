Resumo do estado do projeto — última atualização: 2026-06-04

- Objetivo: Habilitar fluxo completo de solicitação de troca de plantão com notificações (persistidas + SignalR + e-mail).

O que foi alterado/aplicado:
- Migration adicionada: `Yrke/Migrations/AddNotifications.cs` (cria tabela `Notifications`).
- `ApplicationDbContext` já contém `DbSet<Notification> Notifications`.
- `TrocaPlantaoController.SolicitarTroca`: gravação e envio de notificação encapsulados em `try/catch` (para não bloquear criação da troca se notificação falhar).
- Frontend: em `Yrke/Views/Shared/_Navbar.cshtml` removido o atributo `integrity` da importação do SignalR para evitar bloqueio do script.
- Scripts adicionados:
  - `Yrke/scripts/CreateNotificationsTable.ps1` — cria tabela `Notifications` se não existir.
  - `Yrke/scripts/test_swap_flow.py` — script Python para testar login + criar solicitação de troca.

Estado atual observado (resumo):
- Build do projeto compila após ajustes.
- Servidor iniciado em `http://localhost:5002` durante testes locais.
- Seed: usuário `admin@yrke.com` com senha `Admin@123` criado via `SeedData` (ver `Yrke/Data/SeedData.cs`).
- Comportamento crítico anterior: inserção em `Notifications` lançava erro (tabela ausente). Criamos migration/script para resolver isso.
- Testes automáticos tentados pelo assistente (PowerShell/Python) para validar fluxo; servidor respondeu, mas recomendo teste manual final pelo usuário.

Como testar manualmente (rápido):
1) Iniciar servidor:

   dotnet run --project Yrke/Yrke.csproj --urls http://localhost:5002

2) Entrar em `http://localhost:5002/Account/Login` com `admin@yrke.com` / `Admin@123`.
3) Ir em `Home/Trabalhos`, abrir modal "Solicitar troca" e submeter.
4) Verificar no banco:

   SELECT * FROM Trocas ORDER BY Id DESC;
   SELECT * FROM Notifications ORDER BY CreatedAt DESC;

5) Verificar campainha no navbar (SignalR) e logs do `EmailService` para envio de e-mail.

Próximos passos recomendados:
- Se preferir, executar o script SQL (ou o PS) para garantir a tabela `Notifications` no DB.
- Teste manual de troca/aceitar/rejeitar e checar notificações em cliente e banco.
- (Opcional) Remover avisos de nulabilidade em models (`CS8618`) para limpeza futura.

Notas úteis:
- Arquivos relevantes:
  - `Yrke/Controllers/TrocaPlantaoController.cs`
  - `Yrke/Views/Shared/_Navbar.cshtml`
  - `Yrke/Migrations/AddNotifications.cs`
  - `Yrke/scripts/CreateNotificationsTable.ps1`
  - `Yrke/scripts/test_swap_flow.py`
  - `Yrke/Data/SeedData.cs`

Se quiser, na próxima sessão eu sigo a partir desse arquivo e continuo os testes automatizados ou aplico a migration definitiva no banco.