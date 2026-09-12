# Resumo do estado do projeto Yrke

**Última atualização:** 2026-06-15  
**Fase atual:** Fase 2 concluída (termo de ciência + fluxo completo de troca)

---

## Objetivo do projeto

Habilitar o fluxo completo de **troca de plantão** com notificações persistidas, SignalR e e-mail, incluindo aceite/rejeição e efetivação no calendário.

---

## Fase 1 — Concluída (2026-06-15)

### Build e infraestrutura
- [x] `dotnet build Yrke.sln` compila sem erros
- [x] Servidor validado em `http://localhost:5002`
- [x] `SeedData.Inicializar` executado **antes** de `app.Run()` em `Program.cs`
- [x] `app.MapControllers()` adicionado (rotas `/api/*` funcionando)
- [x] Cookie de auth com `SecurePolicy.SameAsRequest` em Development (login funciona em HTTP local)

### Banco de dados
- [x] Migration `20260604120000_AddNotifications` corrigida e aplicada
- [x] Tabela `Notifications` criada no banco
- [x] Histórico de migrations: `InitialCreate`, `AddPlantoes`, `AddUrlFotoToUser`, `AddNotifications`

> **Nota:** o arquivo antigo `AddNotifications.cs` (sem timestamp/Designer) foi substituído pela migration oficial acima.

### Autenticação e API
- [x] Login com claims unificadas (`NameIdentifier`, `Role`, `Email`, `Name`)
- [x] Login testado: `admin@yrke.com` / `Admin@123` e `joao@yrke.com` / `Senha123!`
- [x] `GET /api/Notifications` retorna JSON 200 quando autenticado
- [x] `GET /api/Notifications` redireciona para login quando não autenticado

### Usuários de teste (SeedData)
| Email | Senha | Role |
|-------|-------|------|
| admin@yrke.com | Admin@123 | Administrador |
| joao@yrke.com | Senha123! | Funcionario |
| maria@yrke.com | Senha123! | Funcionario |
| vandersonbarbosatrabalho@gmail.com | 123456 | Funcionario |

> **Atenção:** se o usuário já existia no banco antes do seed, a senha antiga permanece. Em ambiente local, pode ser necessário redefinir a senha manualmente ou recriar o usuário.

---

## Já implementado (antes e durante Fase 1)

### Troca de plantão — backend
- [x] `TrocaPlantaoController` com endpoints: `usuarios`, `solicitar`, `pendentes`, `todas`, `aceitar`, `rejeitar`
- [x] Notificação ao destinatário na solicitação (DB + SignalR + e-mail, com try/catch)
- [x] `NotificationsController` (`GET /api/Notifications`, `POST markread/{id}`)
- [x] `NotificationHub` (SignalR)

### Troca de plantão — frontend
- [x] Modal de solicitação em `Home/Trabalhos.cshtml`
- [x] Painel de trocas pendentes com botões Aceitar/Rejeitar
- [x] Lista geral de trocas consumindo API
- [x] Navbar com sino, badge e conexão SignalR

### Correções gerais (itens do backlog)
- [x] Rota admin na navbar → `Home/DashboardAdmin`
- [x] `InserirTeste` protegido: `[Authorize(Roles = "Administrador")]`, `[HttpPost]`, antiforgery
- [x] `PlantaoController.Listar` com `[Authorize]` e filtro por usuário logado
- [x] Credenciais SMTP removidas de `appsettings.json` (campos vazios; usar User Secrets)
- [x] `EmailService.SendEmailAsync` com `using` e envio assíncrono
- [x] `Cadastrar` atribui `Funcao = model.Funcao`
- [x] `EditarPerfil` GET/POST implementado com upload de foto em `wwwroot/uploads/perfil`
- [x] `PerfilViewModel` unificado (removido `PerfiViewModel.cs` duplicado)

---

## Fase 2 — Concluída (2026-06-15)

### Termo de ciência
- [x] `TermoTrocaService` + `GET /api/TrocaPlantao/{id}/termo` (PDF com nome e data)
- [x] Modal de aceite com iframe + checkbox “Estou ciente” em `Trabalhos.cshtml`

### Fluxo de troca
- [x] Notificação/e-mail ao solicitante ao aceitar ou rejeitar
- [x] Troca efetivada nos registros de `Plantao`
- [x] Impede troca consigo mesmo; datas completas nas mensagens
- [x] Aceitar/Rejeitar só para destinatário (`podeResponder`)

### Melhorias futuras
- [ ] Validação rigorosa de plantão existente e conflito de datas
- [ ] Testes automatizados do fluxo
- [ ] Ajuste fino da posição do nome/data no PDF (se necessário)
- [ ] Validar SignalR com dois browsers

### Backlog geral
- [ ] Recuperação de senha mais segura
- [ ] FKs User ↔ Plantao ↔ TrocaPlantao
- [ ] Enums para Status, Turno, Role
- [ ] Warnings CS8618; seed de senhas em Development

---

## Como testar manualmente

```powershell
dotnet run --project Yrke/Yrke.csproj --urls http://localhost:5002
```

1. Login em `/Account/Login` com credenciais da tabela acima
2. Abrir `/Home/Trabalhos` → solicitar troca com outro usuário
3. Conferir no banco:

```sql
SELECT * FROM Trocas ORDER BY Id DESC;
SELECT * FROM Notifications ORDER BY CreatedAt DESC;
```

4. Login como destinatário → clicar **Aceitar** → ler termo PDF → marcar ciência → confirmar
5. Login como solicitante → verificar notificação de aceite/rejeição

---

## Arquivos principais

| Área | Arquivos |
|------|----------|
| Troca | `Controllers/TrocaPlantaoController.cs`, `Services/TermoTrocaService.cs`, `Views/Home/Trabalhos.cshtml`, `Documents/termo_santa_rita.pdf` |
| Notificações | `Controllers/NotificationsController.cs`, `Hubs/NotificationHub.cs`, `Views/Shared/_Navbar.cshtml` |
| Infra | `Program.cs`, `Data/SeedData.cs`, `Migrations/20260604120000_AddNotifications.cs` |
| Conta/Perfil | `Controllers/AccountController.cs`, `Views/Account/Perfil.cshtml` |

---

## Documentação relacionada na pasta `txt/`

| Arquivo | Uso |
|---------|-----|
| `ResumoEstado.md` | **Este arquivo** — status geral e plano |
| `fluxo-e-implementacao-de-troca.md` | Especificação técnica do fluxo de troca |
| `NovasAcoes.txt` | Checklist de correções gerais (muitas já feitas) |
| `ANALISE_PROJETO_YRKE.txt` | Auditoria histórica (02/05/2026) com reconciliação no topo |
| `ajuste-troca-plantao-2026-06-06.md` | **Arquivado** — notas de sessão antiga; consultar só para histórico |
