# Owner Tenant — auditoria de telas e contratos

O nome ERA da especificação foi interpretado como Whera, conforme o contexto dos repositórios.

## Contratos confirmados na API Whera

| Tela | Endpoint | Situação |
|---|---|---|
| Login | `POST /api/autenticacao/login` | Pronto |
| Dashboard / empresa atual | `GET /api/perfis/minha-empresa` | Pronto |
| Perfil da empresa | `PUT /api/empresas/{id}/perfil` | Pronto |
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

## Origem da implementação

Foram auditados o frontend `centro_comecial`, os serviços `lib/api.ts`, `lib/auth.ts`, `services/company-profile.service.ts`, `services/company-content.service.ts` e os controllers reais de `Whera/src/Whera.Api/Controllers`.
