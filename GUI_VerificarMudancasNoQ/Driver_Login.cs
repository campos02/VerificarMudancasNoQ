using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Windows;

namespace GUI_VerificarMudancasNoQ
{
    public class Driver_Login
    {
        private IWebDriver _driver;
        public IWebDriver driver { get => _driver; }
        private string login_do_Q;
        private string senha_do_Q;
        public string navegador { get; set; }
        public string pagina { get; set; }
        public string intervalo_string { get; set; }
        private int intervalo;

        
        public Driver_Login(string login, string senha)
        {
            login_do_Q = login;
            senha_do_Q = senha;
        }

        //Função que verifica a escolha entre Chrome e Firefox e inicia a nova instância do driver escolhido com a opção headless
        public void iniciar_driver()
        {
            ler_config();
            try
            {
                if (navegador == "chrome")
                {
                    ChromeOptions c_options = new ChromeOptions();
                    c_options.AddArgument("--headless");
                    var driverService = ChromeDriverService.CreateDefaultService();
                    driverService.HideCommandPromptWindow = true;
                    _driver = new ChromeDriver(driverService, c_options);
                }
                if (navegador == "firefox")
                {
                    FirefoxOptions f_options = new FirefoxOptions();
                    f_options.AddArgument("--headless");
                    var driverService = FirefoxDriverService.CreateDefaultService();
                    driverService.HideCommandPromptWindow = true;
                    _driver = new FirefoxDriver(driverService, f_options);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Exceção");
            }
            fazer_login();
        }

        /*Função que pede o login e senha, entra na página de login do QAcadêmico, preenche os respectivos formulários e vai para a 
         página configurada*/ 
        public void fazer_login()
        {
            _driver.Url = "http://qacademico.ifsul.edu.br//qacademico/index.asp?t=1001";
            IWebElement caixadelogin = _driver.FindElement(By.Id("txtLogin"));
            caixadelogin.SendKeys(login_do_Q);
            IWebElement caixadesenha = _driver.FindElement(By.Id("txtSenha"));
            caixadesenha.SendKeys(senha_do_Q);
            IWebElement Logar = _driver.FindElement(By.Id("btnOk"));
            Logar.Click();
            System.Threading.Thread.Sleep(2000);//Esperar 2 segundos até a pagina inicial carregar
            _driver.Url = pagina;
        }

        //Função que lê o arquivo de configuração para obter as opções
        public void ler_config()
        {
            pagina = Properties.Settings.Default.Pagina;
            intervalo_string = Properties.Settings.Default.Intervalo;
            int.TryParse(intervalo_string, out intervalo);
            navegador = Properties.Settings.Default.Navegador;
        }

        //Função que faz o programa dormir no intervalo configurado e depois atualiza a página
        public void sleep_refresh()
        {
            System.Threading.Thread.Sleep(intervalo);
            _driver.Navigate().Refresh();
        }

        //Fecha o driver
        public void fechar()
        {
            driver.Quit();
        }
    }
}
