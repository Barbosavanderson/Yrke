<p align="center">
  <img src="Yrke/wwwroot/img/Yrke.png" alt="Logo Yrke" width="450"/>
</p>


## Problema

Yrke surge para sanar uma dor que toda empresa que trabalhe com trocas de plantão sofre, "quem vai trabalhar hoje", "com quem foi trocado", "onde se encontra a documentação dessa troca" e "alguém deixou o gestor ciente". Essa são as questões que sempre aparecem quando aparece um erro, pois não documentamos, deixamos a supervisão ciente ou até mesmo nós lembramos da troca. Visto isso  precisamos ajudar as equipes a diminuir esse erro ao máximo.

#  Matriz Certezas, Suposições e Dúvidas
| Categoria     | Item                                                                 |
|---------------|----------------------------------------------------------------------|
| **Certezas**  | - A empresa deseja sistema que crie artefatos para trocas |
|               | - O sistema deve incluir notificação de aceite para os dois funcionarios. |
|               | - O sistema deve encaminhar um aviso sobre a troca (já confirmada), para a gestão. |
|               | - O público-alvo inclui todos que trabalham em plantão, contudo em teste será usada Enferemeiras para os testes iniciais. |
|               |                                                    |
| **Suposições**| - O cliente possui estrutura mínima para gerenciar testes online. |
|               | - A equipe interna está disposta a aprender a operar o sistema. |
|               | - A integração com WhatsApp e email seram suficientes para sinalizar a gestão. |
|               | - O sistema poderá ser hospedado em servidor cloud acessível. |
|                |                                                             | 
| **Dúvidas**   | - Qual será o volume médio de acessos e transações diárias? |
|               | - Haverá necessidade de criar relatórios para gestão. |
|               | - O  painel administrativo com relatórios detalhados? |
|               | - Como será feita a gestão de monetização. |
# Casos de Teste – Login

## 1. Login com credenciais válidas

| ID    | Descrição    | Entrada                  | Resultado Esperado          |
|-------|--------------|--------------------------|-----------------------------|
| CT-01 | Login válido | Usuário e senha corretos | Login realizado com sucesso |

## 2. Login com credenciais inválidas

| ID    | Descrição           | Entrada                       | Resultado Esperado                 |
|-------|---------------------|-------------------------------|------------------------------------|
| CT-02 | Senha incorreta     | Usuário válido + senha errada | Mensagem de erro                   |
| CT-03 | Usuário inexistente | Usuário inválido              | Mensagem de erro                   |
| CT-04 | Usuário vazio       | Campo usuário vazio           | Validação de campo obrigatório     |
| CT-05 | Senha vazia         | Campo senha vazio             | Validação de campo obrigatório     |

## 3. Validações de campo

| ID    | Descrição                                  |
|-------|--------------------------------------------|
| CT-06 | Limite mínimo de caracteres                |
| CT-07 | Limite máximo de caracteres                |
| CT-08 | Bloquear espaços extras                    |
| CT-09 | Case sensitivity (senha sensível a maiúsculas) |
| CT-10 | Caracteres especiais                       |

## 4. Mensagens e UX

| ID    | Descrição                             |
|-------|---------------------------------------|
| CT-11 | Mensagem clara para erro de login      |
| CT-12 | Não informar qual campo está incorreto |
| CT-13 | Indicador de carregamento              |
| CT-14 | Botão desabilitado durante envio       |

## 5. Segurança

| ID    | Descrição                                 |
|-------|-------------------------------------------|
| CT-15 | Senha mascarada                           |
| CT-16 | Não logar senha em console/network        |
| CT-17 | Tentativas de login (bloqueio ou delay)   |
| CT-18 | Proteção contra SQL Injection             |
| CT-19 | Proteção contra XSS                       |
| CT-20 | HTTPS obrigatório                         |

## 6. Sessão e autenticação

| ID    | Descrição                         |
|-------|-----------------------------------|
| CT-21 | Criar sessão após login           |
| CT-22 | Logout invalida sessão            |
| CT-23 | Expiração de sessão               |
| CT-24 | Acesso direto sem login bloqueado |

## 7. Compatibilidade

| ID    | Descrição                    |
|-------|------------------------------|
| CT-25 | Login no Chrome              |
| CT-26 | Login no Edge                |
| CT-27 | Login no Firefox             |
| CT-28 | Responsivo (mobile/tablet)   |

## 8. Performance

| ID    | Descrição                       |
|-------|---------------------------------|
| CT-29 | Tempo de resposta aceitável     |
| CT-30 | Múltiplos logins simultâneos    |

## Critérios de Aceite

- 100% dos testes críticos aprovados
- Nenhuma falha de segurança
- Mensagens claras e consistentes
- Login funcional em todos os navegadores suportados
