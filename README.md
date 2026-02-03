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
