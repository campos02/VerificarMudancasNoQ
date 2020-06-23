# GUI_VerificarMudancasNoQ
Programa que verifica e notifica mudanças em alguma página do QAcadêmico

Esse programa utiliza o Chromedriver http://chromedriver.chromium.org ou o Geckodriver(firefox) https://github.com/mozilla/geckodriver/releases.

## Uso

1. Baixe o driver do chrome ou firefox(links acima). O executável pode tanto ser posto na mesma pasta deste programa quanto colocado em outra pasta a partir da adição na variável PATH do Windows.

2. Por padrão o navegador utilizado é o chrome, o intervalo de verificação é de 300000 ms e a página do Q verificada é http://qacademico.ifsul.edu.br//qacademico/index.asp?t=2071.
Após o início do programa é possível editar estes abrindo a janela de configurações a partir do ícone na área de notificações. O intervalo deverá ser configurado em milissegundos.

3. Ao abrir GUI_VerificarMudancasNoQ.exe e entre seu login e senha do Q. 

O programa, durante toda sua execução, fica na área de notificação e começa a fazer a verificar de acordo com o arquivo de texto em intervalos com a duração configurada.
Caso o arquivo não exista, ele será criado. Na ocorrência de em alguma verificação constarem mudanças na página, uma notificação aparecerá, acompanhada de um som.
