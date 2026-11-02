// Comportamentos de interface do painel. Nenhuma regra de permissão fica aqui: a API confere tudo.
(function () {
    var body = document.body;

    // Menu: recolhido no computador, gaveta no celular.
    var chave = 'whera.menu.compacto';
    var btnRecolher = document.getElementById('btnMenu');
    var btnAbrir = document.getElementById('btnAbrirMenu');
    var fundo = document.getElementById('fundoMenu');
    function lerPreferencia() { try { return window.localStorage.getItem(chave) === '1'; } catch (e) { return false; } }
    function aplicarRecolhido(recolhido) {
        body.classList.toggle('menu-recolhido', recolhido);
        // Menu recolhido mostra só ícones: os itens do submenu ficam sempre visíveis.
        Array.prototype.forEach.call(document.querySelectorAll('.submenu'), function (submenu) {
            submenu.open = recolhido || submenu.getAttribute('data-ativo') === '1';
        });
        if (btnRecolher) {
            btnRecolher.setAttribute('aria-expanded', recolhido ? 'false' : 'true');
            btnRecolher.setAttribute('aria-label', recolhido ? 'Expandir menu' : 'Recolher menu');
        }
    }
    function fecharGaveta() {
        body.classList.remove('menu-aberto');
        if (btnAbrir) btnAbrir.setAttribute('aria-expanded', 'false');
    }
    aplicarRecolhido(lerPreferencia());
    if (btnRecolher) btnRecolher.addEventListener('click', function () {
        var recolhido = !body.classList.contains('menu-recolhido');
        try { window.localStorage.setItem(chave, recolhido ? '1' : '0'); } catch (e) { }
        aplicarRecolhido(recolhido);
    });
    if (btnAbrir) btnAbrir.addEventListener('click', function () {
        body.classList.add('menu-aberto');
        btnAbrir.setAttribute('aria-expanded', 'true');
        var primeiro = document.querySelector('.lateral a');
        if (primeiro) primeiro.focus();
    });
    if (fundo) fundo.addEventListener('click', fecharGaveta);
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && body.classList.contains('menu-aberto')) { fecharGaveta(); if (btnAbrir) btnAbrir.focus(); }
    });

    // Confirmação de ações destrutivas: <a data-confirmar="Pergunta" data-confirmar-acao="Remover">.
    var dialogo = document.getElementById('dlgConfirmar');
    var pendente = null;
    if (dialogo && dialogo.showModal) {
        document.addEventListener('click', function (e) {
            var alvo = e.target.closest ? e.target.closest('[data-confirmar]') : null;
            if (!alvo) return;
            if (alvo === pendente) { pendente = null; return; }
            e.preventDefault();
            e.stopImmediatePropagation();
            document.getElementById('dlgConfirmarTitulo').textContent = alvo.getAttribute('data-confirmar');
            document.getElementById('dlgConfirmarSim').textContent = alvo.getAttribute('data-confirmar-acao') || 'Confirmar';
            dialogo.alvo = alvo;
            dialogo.showModal();
        }, true);
        document.getElementById('dlgConfirmarNao').addEventListener('click', function () { dialogo.close(); });
        document.getElementById('dlgConfirmarSim').addEventListener('click', function () {
            var alvo = dialogo.alvo;
            dialogo.close();
            pendente = alvo;
            alvo.click();
        });
    }

    // Aviso de alterações não salvas nos formulários com data-alteracoes.
    var enviando = false;
    // Pelo primeiro formulário: dentro da MasterPage, o ASP.NET troca o id do form.
    var form = document.forms[0];
    var alterado = false;
    if (form) form.addEventListener('submit', function () { enviando = true; });
    if (typeof window.__doPostBack === 'function') {
        var postBackOriginal = window.__doPostBack;
        window.__doPostBack = function (a, b) { enviando = true; return postBackOriginal(a, b); };
    }
    function marcarAlterado(area) {
        alterado = true;
        var aviso = area.querySelector('[data-aviso-alteracao]');
        if (aviso) {
            aviso.textContent = 'Você tem alterações não salvas.';
            aviso.parentNode.classList.add('alterado');
        }
    }
    Array.prototype.forEach.call(document.querySelectorAll('[data-alteracoes]'), function (area) {
        area.addEventListener('input', function () { marcarAlterado(area); });
        area.addEventListener('change', function () { marcarAlterado(area); });
    });
    window.addEventListener('beforeunload', function (e) {
        if (alterado && !enviando) { e.preventDefault(); e.returnValue = ''; }
    });

    // Prévia da imagem escolhida, antes do envio.
    Array.prototype.forEach.call(document.querySelectorAll('[data-previa-arquivo]'), function (entrada) {
        var original = null;
        var tipos = ['image/jpeg', 'image/png', 'image/webp'];
        entrada.addEventListener('change', function () {
            var slot = entrada.closest('.slot');
            var imagem = slot.querySelector('.slot-quadro img');
            var erro = slot.querySelector('.slot-erro');
            var arquivo = entrada.files && entrada.files[0];
            if (original === null) original = imagem.getAttribute('src') || '';
            if (erro) erro.textContent = '';
            if (arquivo && (tipos.indexOf(arquivo.type) < 0 || arquivo.size > 5 * 1024 * 1024)) {
                if (erro) erro.textContent = 'Escolha uma imagem JPG, PNG ou WebP de até 5 MB.';
                entrada.value = '';
                arquivo = null;
            }
            if (arquivo) imagem.src = URL.createObjectURL(arquivo);
            else if (original) imagem.src = original;
            else imagem.removeAttribute('src');
        });
    });

    // Tema: a escolha vale na hora e fica neste navegador.
    var opcoesTema = document.getElementById('opcoesTema');
    if (opcoesTema && window.EloTema) {
        Array.prototype.forEach.call(opcoesTema.querySelectorAll('input'), function (opcao) {
            opcao.checked = opcao.value === window.EloTema.obter();
            opcao.addEventListener('change', function () { window.EloTema.definir(opcao.value); });
        });
    }

    // Galeria de produtos: cada ação só fica ativa se servir a algum produto selecionado.
    var barraSelecao = document.getElementById('barraSelecao');
    if (barraSelecao) {
        var marcas = Array.prototype.slice.call(document.querySelectorAll('.produto-marca input'));
        var todos = document.getElementById('chkTodos');
        var total = document.getElementById('totalSelecionados');
        var regras = {
            adicionar: function (p) { return p.dataset.inativo === '0' && p.dataset.noCatalogo === '0'; },
            remover: function (p) { return p.dataset.inativo === '0' && p.dataset.noCatalogo === '1'; },
            inativar: function (p) { return p.dataset.inativo === '0'; },
            reativar: function (p) { return p.dataset.inativo === '1'; },
            excluir: function () { return true; }
        };
        var atualizarSelecao = function () {
            var selecionados = marcas.filter(function (m) { return m.checked; }).map(function (m) { return m.closest('.produto'); });
            marcas.forEach(function (m) { m.closest('.produto').classList.toggle('selecionado', m.checked); });
            total.textContent = selecionados.length === 0 ? 'Nenhum selecionado' : selecionados.length === 1 ? '1 selecionado' : selecionados.length + ' selecionados';
            todos.checked = marcas.length > 0 && selecionados.length === marcas.length;
            Array.prototype.forEach.call(barraSelecao.querySelectorAll('[data-acao]'), function (botao) {
                botao.disabled = !selecionados.some(regras[botao.getAttribute('data-acao')]);
            });
        };
        marcas.forEach(function (m) { m.addEventListener('change', atualizarSelecao); });
        todos.addEventListener('change', function () { marcas.forEach(function (m) { m.checked = todos.checked; }); atualizarSelecao(); });
        atualizarSelecao();
    }

    // Horários: abre e fecha os campos conforme o dia e o almoço, e repete a segunda nos dias úteis.
    function atualizarDia(dia) {
        var aberto = dia.querySelector('.dia-nome input');
        var almoco = dia.querySelector('.dia-almoco input');
        dia.classList.toggle('fechado', !aberto.checked);
        dia.classList.toggle('com-almoco', !!almoco && almoco.checked);
    }
    Array.prototype.forEach.call(document.querySelectorAll('.dia'), function (dia) {
        dia.addEventListener('change', function () { atualizarDia(dia); });
    });
    var repetir = document.getElementById('btnRepetirSegunda');
    if (repetir) repetir.addEventListener('click', function () {
        var origem = document.querySelector('.dia[data-dia="Segunda"]');
        ['Terca', 'Quarta', 'Quinta', 'Sexta'].forEach(function (nome) {
            var dia = document.querySelector('.dia[data-dia="' + nome + '"]');
            if (dia.querySelector('.dia-fixos') || origem.querySelector('.dia-fixos')) return;
            var campos = dia.querySelectorAll('input:not([type=hidden])');
            Array.prototype.forEach.call(origem.querySelectorAll('input:not([type=hidden])'), function (campo, i) {
                if (campo.type === 'checkbox') campos[i].checked = campo.checked;
                else campos[i].value = campo.value;
            });
            atualizarDia(dia);
        });
        marcarAlterado(repetir.closest('[data-alteracoes]'));
    });
}());
