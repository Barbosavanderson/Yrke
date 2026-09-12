# Índice da pasta `txt/`

Documentação interna do projeto Yrke. Atualizado em **2026-06-15**.

---

## Qual arquivo usar?

| Arquivo | Manter? | Para que serve |
|---------|---------|----------------|
| **ResumoEstado.md** | Sim — principal | Status geral, Fase 1 concluída, plano Fase 2, como testar |
| **fluxo-e-implementacao-de-troca.md** | Sim | Especificação técnica do fluxo de troca (feito vs pendente) |
| **NovasAcoes.txt** | Sim | Checklist rápido de tarefas (grupos 1–5) |
| **ANALISE_PROJETO_YRKE.txt** | Sim (referência) | Auditoria de 02/05/2026 + tabela de reconciliação no topo |
| **ajuste-troca-plantao-2026-06-06.md** | Opcional | Arquivado — histórico de sessão; pode ser removido se quiser |

---

## Recomendação de limpeza

- **Remover:** `Yrke/txt/ResumoEstado.md` — duplicata desatualizada (substituída por `txt/ResumoEstado.md`)
- **Manter os 4–5 arquivos acima** — cada um tem papel distinto; evita perder contexto histórico da auditoria
- **Não recriar** scripts mencionados em docs antigos (`CreateNotificationsTable.ps1`, `test_swap_flow.py`) — migration EF oficial substitui o script SQL

---

## Ordem de leitura sugerida

1. `ResumoEstado.md` — visão geral
2. `fluxo-e-implementacao-de-troca.md` — detalhe da feature em andamento
3. `NovasAcoes.txt` — checklist executável
4. `ANALISE_PROJETO_YRKE.txt` — só se precisar do histórico completo da auditoria
