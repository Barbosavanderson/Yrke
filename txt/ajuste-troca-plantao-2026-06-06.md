# Ajuste de Troca de Plantão - 2026-06-06

## Contexto
- A notificação é criada no backend, mas o sino do navbar não está exibindo o aviso corretamente.
- Não existe atualmente uma área de confirmação/aceite/rejeição visível para o destinatário da troca.
- O fluxo de troca precisa mostrar data e hora da troca com clareza para o usuário.

## O que já existe
- `Yrke/Controllers/TrocaPlantaoController.cs` com endpoints:
  - `GET api/TrocaPlantao/usuarios`
  - `POST api/TrocaPlantao/solicitar`
  - `GET api/TrocaPlantao/pendentes`
  - `GET api/TrocaPlantao/todas`
  - `POST api/TrocaPlantao/{id}/aceitar`
  - `POST api/TrocaPlantao/{id}/rejeitar`
- `Yrke/Controllers/NotificationsController.cs` que retorna notificações do usuário e marca como lida.
- `Yrke/Views/Shared/_Navbar.cshtml` com badge de notificações e conexão SignalR.
- `Yrke/Views/Home/Trabalhos.cshtml` com modal para solicitar troca e lista de trocas gerais.

## Problemas identificados
1. O navbar mostra o sino, mas não há um local de confirmação para aceitar/rejeitar a troca.
2. A lista atual de trocas (`api/TrocaPlantao/todas`) apresenta apenas nomes e status; não mostra datas/horários claros.
3. A notificação criada está genérica e apenas contém a data do `PlantaoB`, sem link direto para o fluxo de confirmação.
4. O front-end ainda não consome `api/TrocaPlantao/pendentes` para exibir as solicitações que o usuário deve responder.
5. O seed atual no código não contém o usuário `vandersonbarbosatrabalho@gmail.com`; portanto, se o teste for com seed padrão, esse login não estará disponível.

## O que foi feito
- Implementado um painel de trocas pendentes em `Yrke/Views/Home/Trabalhos.cshtml`.
- A lista de trocas pendentes agora consome `GET api/TrocaPlantao/pendentes` e mostra `Solicitante`, `Destinatário`, `Plantão A` e `Plantão B` com data/hora.
- Adicionados botões `Aceitar` e `Rejeitar` para solicitações pendentes no front-end.
- A lista geral de trocas em `Yrke/Views/Home/Trabalhos.cshtml` agora exibe data/hora e status de forma mais clara.
- As notificações geradas em `Yrke/Controllers/TrocaPlantaoController.cs` foram enriquecidas com data/hora completas e link para `Home/Trabalhos`.
- O endpoint de aceitação/rejeição agora envia notificação de retorno ao solicitante quando a troca é aceita ou rejeitada.
- Adicionado o usuário de teste `vandersonbarbosatrabalho@gmail.com` com senha `123456` em `Yrke/Data/SeedData.cs`.
- O build travado por bloqueio de arquivo (`MSB3027/MSB3021`) foi resolvido ao encerrar o processo `Yrke` que mantinha `Yrke.exe` aberto.

## O que podemos fazer
1. Garantir que a navbar carregue corretamente as notificações com `fetch('/api/Notifications')` e atualize o badge usando o campo `isRead`.
2. Validar o fluxo de SignalR e se o claim do usuário (`NameIdentifier`) está sendo usado corretamente para entregar a notificação.
3. Adicionar observação/descrição no painel de trocas pendentes para cada troca.
4. Criar testes automatizados para `SolicitarTroca`, `AceitarTroca` e `RejeitarTroca`.
5. Revisar possíveis melhorias de usabilidade no modal de solicitação (ex: seleção de horário mais refinada).

## Testes sugeridos
- Fazer login com uma conta válida e abrir `Home/Trabalhos`.
- Criar uma troca e verificar se a notificação aparece em `/api/Notifications`.
- Verificar o badge do sino no navbar após criar/receber a notificação.
- Confirmar que o usuário destinatário consegue ver a troca pendente com detalhes de data/hora.
- Aceitar/rejeitar e validar se a troca muda de status.

## Nota sobre usuário de teste
- O seed atual contém apenas: `admin@yrke.com`, `joao@yrke.com` e `maria@yrke.com`.
- O usuário `vandersonbarbosatrabalho@gmail.com` não é criado por `SeedData.cs` neste código.
- Se o login falhar, preciso que você confirme se devemos adicionar esse usuário ao seed ou se já existe no seu banco de dados local.
