# WheraOwnerTenant

Base independente do painel de gestão dos tenants da Whera.

## Setup atual

- ASP.NET Web Forms / .NET Framework 4.8.
- Cliente HTTP server-side em `App_Code/WheraApi.cs`.
- Autenticação e sessão isoladas em `App_Code/AutenticacaoTenant.cs` e `App_Code/SessaoTenant.cs`.
- URL e timeout da API configuráveis em `Web.config` (`WheraApiUrl` e `WheraApiTimeout`).
- Login em duas etapas conforme o contrato atualmente usado pela API Whera.
- Dashboard inicial protegido por sessão, sem dados fictícios ou acesso direto ao banco.
- Telas de dashboard, perfil da empresa, publicações, usuários e acesso, redes sociais, relatórios e configurações.
- Imagens, catálogo, horários e atendentes. O cadastro inicial de empresa pertence ao Centro Comercial; esta aplicação permanece focada no acesso e na operação do Tenant.
- Matriz de contratos e bloqueios em `docs/ENDPOINTS_E_TELAS.md`.
- Prévia local disponível com `WheraDevPreview=true`; ela só aparece em requisições locais, não chama a API e não substitui o login real.

## Executar

Abra `WheraOwnerTenant.sln` no Visual Studio 2022 com o workload de ASP.NET e .NET Framework 4.8. A aplicação usa o mesmo endpoint padrão do projeto WheraOwner:

`https://whera-api.onrender.com`

Para desenvolvimento local, não altere o `Web.config`: defina a variável de ambiente `WHERA_API_URL=http://localhost:5128` na sua máquina, em nível de usuário, e reabra o Visual Studio. Ela tem prioridade sobre `WheraApiUrl` e vale também para o WheraOwner. Para voltar a usar produção, remova a variável.

As telas de tenant e seus endpoints devem ser adicionados somente depois da confirmação dos contratos disponíveis na API.
