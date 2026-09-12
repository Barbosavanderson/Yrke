# Fluxo e Implementação de Troca de Plantão

**Última atualização:** 2026-06-15  
**Status:** Backend e front parcialmente prontos — Fase 2 pendente

---

## Objetivo

Fluxo consistente para solicitação → notificação → aceite/rejeição → **efetivação no calendário** → feedback ao solicitante.

---

## O que já foi feito

### Backend (`TrocaPlantaoController.cs`)
- [x] `GET api/TrocaPlantao/usuarios` — lista colegas (exceto o logado)
- [x] `POST api/TrocaPlantao/solicitar` — registra troca com `Status = "Pendente"`
- [x] `GET api/TrocaPlantao/pendentes` — trocas pendentes do usuário
- [x] `GET api/TrocaPlantao/todas` — histórico geral
- [x] `POST api/TrocaPlantao/{id}/aceitar` — altera status para `"Aceita"`
- [x] `POST api/TrocaPlantao/{id}/rejeitar` — altera status para `"Negada"`
- [x] Notificação ao destinatário na solicitação (persistência + SignalR + e-mail)
- [x] Validação básica: destinatário existe, campos obrigatórios preenchidos

### Modelos e persistência
- [x] `Notification` com `UserId`, `Title`, `Message`, `Link`, `IsRead`, `CreatedAt`
- [x] `ApplicationDbContext` com `DbSet<TrocaPlantao>` e `DbSet<Notification>`
- [x] Migration `20260604120000_AddNotifications` aplicada

### Notificações e tempo real
- [x] `NotificationsController` — listar e marcar como lida
- [x] `NotificationHub` — push via SignalR
- [x] Navbar com sino, fetch `/api/Notifications` e badge por `isRead`

### Frontend (`Home/Trabalhos.cshtml`)
- [x] Modal de solicitação com seleção de colega e datas
- [x] Painel **Trocas Pendentes** consumindo `/api/TrocaPlantao/pendentes`
- [x] Botões Aceitar/Rejeitar no front-end
- [x] Lista **Próximas trocas** consumindo `/api/TrocaPlantao/todas`

### Infra (Fase 1)
- [x] `MapControllers()` em `Program.cs`
- [x] Cookie auth funcional em HTTP local (Development)
- [x] Login e API de notificações validados

---

## O que ainda NÃO foi feito (Fase 2)

### Termo de ciência no aceite (novo)
- [ ] Exibir `termo_santa_rita.pdf` em modal ao clicar **Aceitar**
- [ ] Preencher PDF com **nome** do destinatário e **data** atual
- [ ] Exigir confirmação “Estou ciente” antes de efetivar o aceite
- [ ] Rejeitar continua sem termo

### Crítico para o fluxo completo
- [ ] **Efetivação no calendário:** `AceitarTroca` não altera registros de `Plantao`
- [ ] **Notificação de retorno:** solicitante não é avisado quando troca é aceita/rejeitada
- [ ] **Validações de negócio:**
  - troca para si mesmo
  - plantão inexistente ou não pertencente ao usuário
  - conflito de datas do destinatário

### Melhorias de UX e dados
- [ ] Mensagem de notificação de solicitação ainda usa só data de `PlantaoB` (`yyyy-MM-dd`), sem hora
- [ ] API `pendentes`/`todas` retorna datas sem hora (`yyyy-MM-dd`)
- [ ] Validar SignalR em tempo real com dois browsers/usuários
- [ ] Campo observação/descrição no painel de pendentes (opcional)

### Qualidade
- [ ] Testes automatizados: solicitar, aceitar, rejeitar
- [ ] Logging de falhas de e-mail/SignalR/notificação (hoje silenciadas em try/catch)

---

## Plano Fase 2 (ordem sugerida)

### Etapa A — Termo de ciência (`termo_santa_rita.pdf`)

| # | Tarefa | Arquivos principais |
|---|--------|---------------------|
| A1 | Copiar/garantir acesso ao template `Documents/termo_santa_rita.pdf` | `Yrke/Documents/`, `Yrke.csproj` (CopyToOutput) |
| A2 | Criar `TermoTrocaService` — preenche PDF com **nome** (usuário logado) e **data** (hoje) | `Services/TermoTrocaService.cs` |
| A3 | Endpoint `GET /api/TrocaPlantao/{id}/termo` — só destinatário da troca pendente | `TrocaPlantaoController.cs` |
| A4 | Modal de aceite: clicar Aceitar → abrir termo → checkbox “Estou ciente” → confirmar | `Views/Home/Trabalhos.cshtml` |
| A5 | Só após ciência confirmada, chamar `POST .../aceitar` | JS em `Trabalhos.cshtml` |

**Escopo simples (conforme solicitado):**
- Preencher apenas nome da pessoa que aceita e data
- Não exige assinatura digital, upload ou armazenamento do PDF assinado
- Rejeitar troca segue direto, sem termo

**Tecnologia sugerida:** biblioteca .NET para PDF (ex.: PdfSharpCore) com sobreposição de texto no template.

---

### Etapa B — Fluxo completo de troca

| # | Tarefa | Arquivos principais |
|---|--------|---------------------|
| B1 | Notificação + e-mail ao solicitante em aceitar/rejeitar | `TrocaPlantaoController.cs` |
| B2 | Trocar plantões no banco ao aceitar | `TrocaPlantaoController.cs`, `Models/Plantao.cs` |
| B3 | Validações de negócio na solicitação e aceite | `TrocaPlantaoController.cs` |
| B4 | Datas/horas completas em notificações e API | `TrocaPlantaoController.cs`, `Trabalhos.cshtml` |
| B5 | Teste manual ciclo completo (2 usuários) | — |

### Etapa C — Qualidade (opcional nesta fase)

| # | Tarefa |
|---|--------|
| C1 | Testes automatizados solicitar/aceitar/rejeitar |
| C2 | Logs de falha e-mail/SignalR/notificação |

---

## Fluxo esperado (alvo)

```
Solicitante                    Destinatário                    Sistema
    |                               |                              |
    |-- POST /solicitar ----------->|                              |
    |                               |<-- Notificação (DB+SignalR) -|
    |                               |<-- E-mail -------------------|
    |                               |                              |
    |                               |-- Clica Aceitar ------------>|
    |                               |<-- Modal termo PDF (nome+data)|
    |                               |-- Confirma ciência ---------->|
    |                               |-- POST /aceitar ------------->|
    |<-- Notificação resultado -----|                              |
    |                               |                              |
    | (se aceita) Plantões trocados no calendário <----------------|

Rejeitar: POST /rejeitar direto (sem termo)
```

---

## Arquivos de referência

- `Yrke/Controllers/TrocaPlantaoController.cs`
- `Yrke/Controllers/NotificationsController.cs`
- `Yrke/Hubs/NotificationHub.cs`
- `Yrke/Models/Notification.cs`, `Models/Plantao.cs`, `Models/TrocaDePlantao.cs`
- `Yrke/Views/Home/Trabalhos.cshtml`
- `Yrke/Views/Shared/_Navbar.cshtml`
- `Yrke/Migrations/20260604120000_AddNotifications.cs`
