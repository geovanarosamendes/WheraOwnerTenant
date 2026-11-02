// Prévia do aplicativo: abas do Perfil da empresa e o atalho que mostra um recurso no celular.
(function () {
    var celular = document.querySelector('.celular-app');
    if (!celular) return;

    function abrirAba(nome) {
        Array.prototype.forEach.call(celular.querySelectorAll('[data-aba-app]'), function (aba) {
            var ativa = aba.getAttribute('data-aba-app') === nome;
            aba.classList.toggle('ativa', ativa);
            aba.setAttribute('aria-selected', ativa ? 'true' : 'false');
        });
        Array.prototype.forEach.call(celular.querySelectorAll('[data-painel-app]'), function (painel) {
            painel.hidden = painel.getAttribute('data-painel-app') !== nome;
        });
    }

    var calmo = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    function rolar(elemento) { elemento.scrollIntoView({ behavior: calmo ? 'auto' : 'smooth', block: 'center', inline: 'nearest' }); }

    function itemDaLista(chave) { return document.querySelector('[data-recurso-item="' + chave + '"]'); }

    // O resumo escolhido fica destacado até outra escolha.
    function selecionarResumo(chave) {
        Array.prototype.forEach.call(document.querySelectorAll('.recurso.selecionado'), function (item) {
            item.classList.remove('selecionado');
            item.removeAttribute('aria-current');
        });
        var item = itemDaLista(chave);
        if (!item) return null;
        item.classList.add('selecionado');
        item.setAttribute('aria-current', 'true');
        return item;
    }

    function piscar(chave) {
        Array.prototype.forEach.call(celular.querySelectorAll('[data-recurso="' + chave + '"]'), function (alvo) {
            alvo.classList.remove('a-destaque');
            void alvo.offsetWidth;
            alvo.classList.add('a-destaque');
        });
    }

    function alternarEquipe(equipe) { equipe.setAttribute('aria-expanded', equipe.getAttribute('aria-expanded') === 'true' ? 'false' : 'true'); }

    function escolherNoCelular(chave) {
        var item = selecionarResumo(chave);
        piscar(chave);
        if (item) rolar(item);
    }

    // Os componentes também funcionam pelo teclado.
    Array.prototype.forEach.call(celular.querySelectorAll('[data-recurso]'), function (componente) {
        var item = itemDaLista(componente.getAttribute('data-recurso'));
        if (!item) return;
        componente.setAttribute('tabindex', '0');
        componente.setAttribute('role', 'button');
        componente.setAttribute('aria-label', 'Ver explicação: ' + item.querySelector('.recurso-nome').textContent);
    });

    celular.addEventListener('click', function (evento) {
        if (!evento.target.closest) return;
        var aba = evento.target.closest('[data-aba-app]');
        if (aba) {
            var nome = aba.getAttribute('data-aba-app');
            abrirAba(nome);
            var painel = celular.querySelector('[data-painel-app="' + nome + '"]');
            if (painel) escolherNoCelular(painel.getAttribute('data-recurso'));
            return;
        }
        var componente = evento.target.closest('[data-recurso]');
        if (!componente) return;
        // Como no app, a equipe abre e fecha ao tocar no cabeçalho.
        if (evento.target.closest('.a-equipe .a-endereco')) alternarEquipe(componente);
        escolherNoCelular(componente.getAttribute('data-recurso'));
    });

    celular.addEventListener('keydown', function (evento) {
        if (evento.key !== 'Enter' && evento.key !== ' ') return;
        if (!evento.target.hasAttribute('data-recurso')) return;
        evento.preventDefault();
        if (evento.target.classList.contains('a-equipe')) alternarEquipe(evento.target);
        escolherNoCelular(evento.target.getAttribute('data-recurso'));
    });

    document.addEventListener('click', function (evento) {
        var botao = evento.target.closest ? evento.target.closest('[data-ver-recurso]') : null;
        if (!botao) return;
        var chave = botao.getAttribute('data-ver-recurso');
        var alvo = celular.querySelector('[data-recurso="' + chave + '"]');
        if (!alvo) return;
        var painel = alvo.closest('[data-painel-app]');
        if (painel) abrirAba(painel.getAttribute('data-painel-app'));
        selecionarResumo(chave);
        rolar(alvo);
        piscar(chave);
    });
})();
