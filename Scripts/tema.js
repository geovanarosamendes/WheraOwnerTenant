// Carregado no <head>, antes do CSS, para a página não piscar no tema errado.
// A preferência fica neste navegador: a API não guarda esse dado.
(function () {
    var chave = 'elo.tema';
    var sistema = window.matchMedia('(prefers-color-scheme: dark)');
    function obter() { try { return window.localStorage.getItem(chave) || 'sistema'; } catch (e) { return 'sistema'; } }
    function aplicar() {
        var preferencia = obter();
        var escuro = preferencia === 'escuro' || (preferencia === 'sistema' && sistema.matches);
        document.documentElement.setAttribute('data-tema', escuro ? 'escuro' : 'claro');
    }
    aplicar();
    if (sistema.addEventListener) sistema.addEventListener('change', aplicar);
    window.EloTema = {
        obter: obter,
        definir: function (valor) {
            try { window.localStorage.setItem(chave, valor); } catch (e) { }
            aplicar();
        }
    };
}());
