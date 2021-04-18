using OpenQA.Selenium;
using System;
using System.Windows;
using System.Threading.Tasks;

namespace GUI_VerificarMudancasNoQ
{
    public class Verificacao
    {
        private DriverLogin login;
        public DriverLogin Login => login;
        private int mudancas = 0;
        private string notas;
        private const string arquivo = "textodoq.log";
        private System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.light);
        public MainWindow MainWindow { get; set; }

        public Verificacao(string loginJanela, string senhaJanela)
        {
            login = new DriverLogin(loginJanela, senhaJanela);
        }

        public Verificacao(string loginJanela, string senhaJanela, MainWindow mainWindow)
        {
            login = new DriverLogin(loginJanela, senhaJanela);
            this.MainWindow = mainWindow;
        }

        private void Notificacao()
        {
            player.Play();
            MainWindow.NotificacaoDeMudancas();
        }

        /*Função que compara um texto fornecido com o texto do arquivo, notifica e
        incrementa o contador de mudanças caso sejam diferentes e o arquivo não esteja em branco*/
        private void VerificarAnterior(string texto)
        {
            string arquivo = "";
            //Tenta ler o arquivo e o cria caso não exista
            try
            {
                arquivo = System.IO.File.ReadAllText(Verificacao.arquivo);
            }
            catch (System.IO.FileNotFoundException)
            {
                System.IO.File.Create(Verificacao.arquivo).Close();
            }
            //Comparação e notificação
            if (texto != arquivo && arquivo != "")
            {
                Notificacao();
                mudancas++;
            }
        }

        public void VerificarAcesso()
        {
            int indexNegado = 1; //Index onde fica o texto obtido da página de acesso negado
            var elementos = login.Driver.FindElements(By.TagName("td"));
            //Caso se encontre na página de acesso negado, lança uma exceção de acesso negado
            if (elementos[indexNegado].Text.Contains("Negado"))
            {
                throw new AcessoNegadoException("Verifique login e senha!");
            }
        }

        /*Função que obtem o texto da página configurada e escreve no arquivo o texto obtido*/
        public void VerificarTexto()
        {
            int indexNegado = 1; //Index onde fica o texto obtido da página de acesso negado
            int indexPaginas = 10; //Index onde geralmente fica o texto obtido da tag das páginas
            var elementos = login.Driver.FindElements(By.TagName("td"));
            //Caso se encontre na página de acesso negado, faz login novamente e retorna a função
            if (elementos[indexNegado].Text.Contains("Negado"))
            {
                login.FazerLogin();
                return;
            }
            if (elementos.Count >= indexPaginas) { notas = elementos[indexPaginas].Text; }
            else { return; }
            notas = notas.Replace('\n', ' '); //Para melhor leitura do arquivo depois
            VerificarAnterior(notas);
            //Salva o texto da página no arquivo e chama a função sleep_refresh
            System.IO.File.WriteAllText(arquivo, notas);
            login.SleepRefresh();
        }

        //Inicia o driver e verifica o acesso à pagina
        public void IniciarVerificarAcesso()
        {
            login.IniciarDriver();
            VerificarAcesso();
        }
    }
}
