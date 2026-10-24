# Owner Tenant — auditoria de telas e contratos

O nome ERA da especificação foi interpretado como Whera, conforme o contexto dos repositórios.

## Contratos confirmados na API Whera

| Tela | Endpoint | Situação |
|---|---|---|
| Login | `POST /api/autenticacao/login` | Pronto |
| Cadastro inicial | `POST /api/pre-cadastros/empresas` | Pronto |
| Vínculo Tenant | `POST /api/pre-cadastros/empresas/{empresaId}/vincular` + `X-Codigo-Continuacao` | Pronto |
| Dashboard / empresa atual | `GET /api/perfis/minha-empresa` | Pronto |
| Perfil da empresa | `PUT /api/empresas/{id}/perfil` | Pronto |
| Imagens do perfil | `POST /api/empresas/{id}/imagens` | Pronto |
| Canais | `GET/POST/PUT/DELETE /api/empresas/{id}/canais` | Pronto via perfil; CRUD de canais disponível |
| Horários | `GET /api/empresas/{id}/horarios`, `PUT /api/empresas/{id}/horarios/semana` | Pronto |
| Catálogo | `POST /api/empresas/{id}/imagens` + `PUT /api/empresas/{id}/perfil` | Pronto |
| Atendentes | `GET/POST/PUT/DELETE /api/empresas/{id}/atendentes`, `PUT .../atendentes/ordem` | Pronto |
| Usuários e acesso | `GET/POST/DELETE /api/empresas/{id}/gerentes` | Pronto para Tenant |
| Publicações | `GET/PUT /api/empresas/{id}/conteudos` | Pronto |
| Mídia de publicação | `POST /api/empresas/{id}/conteudos/midias` | Endpoint confirmado; editor de mídia avançada fica incremental |
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
