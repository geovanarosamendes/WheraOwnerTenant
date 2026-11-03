# Owner Tenant — auditoria de telas e contratos

O nome ERA da especificação foi interpretado como Whera, conforme o contexto dos repositórios.

## Contratos confirmados na API Whera

| Tela | Endpoint | Situação |
|---|---|---|
| Login | `POST /api/autenticacao/login` | Pronto |
| Cadastro inicial | — | Removido deste painel; realizado no `centro_comecial` |
| Vínculo Tenant | — | Removido deste painel; o fluxo de cadastro e vínculo é iniciado no `centro_comecial` |
| Dashboard / empresa atual | `GET /api/perfis/minha-empresa` | Pronto |
| Troca de empresa (mais de uma empresa na conta) | `GET /api/usuarios/me/acesso` + `GET /api/empresas/{id}/perfil` | Pronto. Filial como conceito próprio não existe na API |
| Perfil da empresa | `PUT /api/empresas/{id}/perfil` | Pronto |
| Imagens do perfil | `POST /api/empresas/{id}/imagens` | Pronto |
| Canais | `GET/POST/PUT/DELETE /api/empresas/{id}/canais` | Pronto via perfil; CRUD de canais disponível |
| Horários | `GET /api/empresas/{id}/horarios`, `PUT /api/empresas/{id}/horarios/semana` | Pronto |
| Catálogo | `POST /api/empresas/{id}/imagens` + `PUT /api/empresas/{id}/perfil` | Pronto |
| Atendentes | `GET/POST/PUT/DELETE /api/empresas/{id}/atendentes`, `PUT .../atendentes/ordem` | Pronto |
| Usuários e acesso | `GET/POST/DELETE /api/empresas/{id}/gerentes` | Pronto para Owner e Tenant; Gerente comum não administra gerentes |
| Publicações: histórico, dashboard e relatórios | `GET /api/empresas/{id}/conteudos?tipo=` | Pronto; somente consulta. A criação fica no aplicativo Elo |
| Galeria e catálogo | `GET/PUT/DELETE /api/empresas/{id}/produtos` | Pronto. No catálogo = `Publicado`; inativo = `Disponivel = false`; excluir usa a exclusão lógica da API |
| Importar produtos por planilha | `POST /api/empresas/{id}/produtos/importacao` | Pronto na API local, ainda não publicado em produção. Contrato em `API_IMPORTACAO_PRODUTOS.md`; modelos gerados por `gerar-modelo-produtos.ps1` |
| Cupons resgatados: totais e lista de quem resgatou | `GET /api/empresas/{id}/cupons` + `GET /api/empresas/{id}/cupons/resgates?cupomId=&situacao=&pagina=&tamanhoPagina=` | Pronto; somente consulta |
| Vitrine Elo (página compartilhável paga) | — | Bloqueado: editor, publicação e cobrança não desenvolvidos |
| Métricas de publicação (visualizações, cliques) | — | Bloqueado: a API não registra esses dados |
| Redes sociais | `/api/tenant/.../social/*` | Bloqueado: não existe nos controllers atuais |
| Analytics | `/api/tenant/.../social/.../metrics` | Bloqueado: não existe nos controllers atuais |
| Relatórios CSV | `/api/tenant/.../social/reports/export` | Bloqueado: não existe nos controllers atuais |

## Limites preservados

- O painel nunca acessa o banco diretamente.
- Cada leitura e gravação usa o token da sessão e é autorizada pela API.
- Não foram criados botões OAuth ou métricas fictícias.
- Tenant e Gerente usam o mesmo painel; operações que a API recusar exibem erro de permissão.
- Atendentes são contatos da empresa, não usuários e não recebem login.
- Gerentes podem operar o próprio vínculo de gestão, mas não podem administrar gerentes, transferir ou excluir a empresa.

## Origem da implementação

Foram auditados o frontend `centro_comecial`, os serviços `lib/api.ts`, `lib/auth.ts`, `services/company-profile.service.ts`, `services/company-content.service.ts` e os controllers reais de `Whera/src/Whera.Api/Controllers`.
