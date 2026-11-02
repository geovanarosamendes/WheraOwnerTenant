# Gera Arquivos/modelo-produtos.xlsx e .csv conforme ImportacaoProdutosService da API.
# Se a API mudar colunas ou limites, ajuste aqui e rode de novo: powershell -File docs\gerar-modelo-produtos.ps1
# Sem acentos literais: o PowerShell 5.1 le este arquivo como ANSI. Acentos vao como entidades XML.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem
$pasta = Join-Path (Split-Path $PSScriptRoot -Parent) 'Arquivos'
New-Item -ItemType Directory -Force $pasta | Out-Null
$utf8 = New-Object Text.UTF8Encoding($false)
$colunas = 'nome', 'descricao', 'preco', 'disponivel', 'no_catalogo'
$cabecalhoXml = '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'

[IO.File]::WriteAllText((Join-Path $pasta 'modelo-produtos.csv'), (($colunas -join ';') + "`r`n"), (New-Object Text.UTF8Encoding($true)))

$celulas = ''
for ($i = 0; $i -lt $colunas.Count; $i++) { $celulas += '<c r="' + 'ABCDE'[$i] + '1" s="1" t="inlineStr"><is><t>' + $colunas[$i] + '</t></is></c>' }

function Validacao($tipo, $faixa, $atributos, $formulas, $titulo, $ajuda, $erro) {
    '<dataValidation type="' + $tipo + '" ' + $atributos + ' allowBlank="1" showInputMessage="1" showErrorMessage="1" errorTitle="' + $titulo + '" error="' + $erro + '" promptTitle="' + $titulo + '" prompt="' + $ajuda + '" sqref="' + $faixa + '">' + $formulas + '</dataValidation>'
}
$listaSimNao = '<formula1>&quot;sim,n&#227;o&quot;</formula1>'
$validacoes = @(
    (Validacao 'textLength' 'A2:A1001' 'operator="lessThanOrEqual"' '<formula1>160</formula1>' 'Nome' 'Obrigat&#243;rio. At&#233; 160 caracteres. Nome repetido &#233; ignorado.' 'O nome pode ter at&#233; 160 caracteres.'),
    (Validacao 'textLength' 'B2:B1001' 'operator="lessThanOrEqual"' '<formula1>2000</formula1>' 'Descri&#231;&#227;o' 'Opcional. At&#233; 2000 caracteres.' 'A descri&#231;&#227;o pode ter at&#233; 2000 caracteres.'),
    (Validacao 'decimal' 'C2:C1001' 'operator="between"' '<formula1>0</formula1><formula2>99999999</formula2>' 'Pre&#231;o' 'Opcional. Exemplo: 29,90.' 'Informe um n&#250;mero entre 0 e 99.999.999.'),
    (Validacao 'list' 'D2:D1001' '' $listaSimNao 'Dispon&#237;vel' 'sim ou n&#227;o. Vazio vale sim.' 'Escolha sim ou n&#227;o.'),
    (Validacao 'list' 'E2:E1001' '' $listaSimNao 'No cat&#225;logo' 'sim ou n&#227;o. Vazio vale n&#227;o. Exige dispon&#237;vel.' 'Escolha sim ou n&#227;o.')
) -join ''

$planilha = $cabecalhoXml + '<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><sheetViews><sheetView tabSelected="1" workbookViewId="0"><pane ySplit="1" topLeftCell="A2" activePane="bottomLeft" state="frozen"/><selection pane="bottomLeft" activeCell="A2" sqref="A2"/></sheetView></sheetViews><sheetFormatPr defaultRowHeight="15"/><cols><col min="1" max="1" width="36" customWidth="1"/><col min="2" max="2" width="50" customWidth="1"/><col min="3" max="3" width="12" style="2" customWidth="1"/><col min="4" max="5" width="14" customWidth="1"/></cols><sheetData><row r="1">' + $celulas + '</row></sheetData><dataValidations count="5">' + $validacoes + '</dataValidations></worksheet>'

$estilos = $cabecalhoXml + '<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><numFmts count="1"><numFmt numFmtId="164" formatCode="#,##0.00"/></numFmts><fonts count="2"><font><sz val="11"/><name val="Calibri"/></font><font><b/><sz val="11"/><color rgb="FFFFFFFF"/><name val="Calibri"/></font></fonts><fills count="3"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill><fill><patternFill patternType="solid"><fgColor rgb="FF2563EB"/><bgColor indexed="64"/></patternFill></fill></fills><borders count="1"><border><left/><right/><top/><bottom/><diagonal/></border></borders><cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs><cellXfs count="3"><xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/><xf numFmtId="0" fontId="1" fillId="2" borderId="0" xfId="0" applyFont="1" applyFill="1"/><xf numFmtId="164" fontId="0" fillId="0" borderId="0" xfId="0" applyNumberFormat="1"/></cellXfs><cellStyles count="1"><cellStyle name="Normal" xfId="0" builtinId="0"/></cellStyles></styleSheet>'

$arquivo = Join-Path $pasta 'modelo-produtos.xlsx'
if (Test-Path $arquivo) { Remove-Item -LiteralPath $arquivo -Force -Confirm:$false }
$zip = [IO.Compression.ZipFile]::Open($arquivo, 'Create')
function Entrada($nome, $conteudo) { $e = $zip.CreateEntry($nome); $s = $e.Open(); $b = $utf8.GetBytes($conteudo); $s.Write($b, 0, $b.Length); $s.Dispose() }
Entrada '[Content_Types].xml' ($cabecalhoXml + '<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/><Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/></Types>')
Entrada '_rels/.rels' ($cabecalhoXml + '<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>')
Entrada 'xl/workbook.xml' ($cabecalhoXml + '<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><bookViews><workbookView/></bookViews><sheets><sheet name="Produtos" sheetId="1" r:id="rId1"/></sheets></workbook>')
Entrada 'xl/_rels/workbook.xml.rels' ($cabecalhoXml + '<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/><Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/></Relationships>')
Entrada 'xl/worksheets/sheet1.xml' $planilha
Entrada 'xl/styles.xml' $estilos
$zip.Dispose()
"Modelos gerados em $pasta"
