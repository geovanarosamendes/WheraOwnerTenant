// Passo a passo de validar cupom: troca a tela do celular conforme o passo escolhido.
(function () {
    var telas = document.querySelectorAll('[data-tela-passo]');
    var itens = document.querySelectorAll('[data-item-passo]');
    if (!telas.length) return;
    var anterior = document.querySelector('[data-passo-anterior]');
    var proximo = document.querySelector('[data-passo-proximo]');
    var contagem = document.querySelector('[data-passo-contagem]');
    var celular = document.querySelector('.celular-app');
    var atual = 0;

    function mostrar(passo) {
        atual = passo;
        Array.prototype.forEach.call(telas, function (tela, i) { tela.hidden = i !== passo; });
        Array.prototype.forEach.call(itens, function (item, i) {
            item.classList.toggle('selecionado', i === passo);
            if (i === passo) item.setAttribute('aria-current', 'step'); else item.removeAttribute('aria-current');
        });
        celular.scrollTop = 0;
        anterior.disabled = passo === 0;
        proximo.disabled = passo === telas.length - 1;
        contagem.textContent = 'Passo ' + (passo + 1) + ' de ' + telas.length;
    }

    document.addEventListener('click', function (evento) {
        var botao = evento.target.closest ? evento.target.closest('[data-ir-passo]') : null;
        if (botao) mostrar(parseInt(botao.getAttribute('data-ir-passo'), 10));
    });
    anterior.addEventListener('click', function () { mostrar(atual - 1); });
    proximo.addEventListener('click', function () { mostrar(atual + 1); });
    mostrar(0);
})();
