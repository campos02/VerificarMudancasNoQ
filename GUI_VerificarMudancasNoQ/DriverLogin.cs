using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Configuration;

namespace GUI_VerificarMudancasNoQ
{
    public class DriverLogin
    {
        private IWebDriver driver;
        public IWebDriver Driver => driver;
        private string loginDoQ;
        private string senhaDoQ;
        public string Navegador { get; set; }
        public string Pagina { get; set; }
        public string IntervaloString { get; set; }
        private int intervalo;

        
        public DriverLogin(string login, string senha)
        {
            loginDoQ = login;
            senhaDoQ = senha;
        }

        //Função que verifica a escolha entre Chrome e Firefox e inicia a nova instância do driver escolhido com a opção headless
        public void IniciarDriver()
        {
            LerConfig();
            if (Navegador == "chrome")
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--headless");
                var driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                driver = new ChromeDriver(driverService, chromeOptions);
            }
            if (Navegador == "firefox")
            {
                FirefoxOptions firefoxOptions = new FirefoxOptions();
                firefoxOptions.AddArgument("--headless");
                var driverService = FirefoxDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                driver = new FirefoxDriver(driverService, firefoxOptions);
            }
            FazerLogin();
        }

        /*Função que pede o login e senha, entra na página de login do QAcadêmico, preenche os respectivos formulários e vai para a 
         página configurada*/ 
        public void FazerLogin()
        {
            driver.Url = "http://qacademico.ifsul.edu.br//qacademico/index.asp?t=1001";
            IWebElement caixaDeLogin = driver.FindElement(By.Id("txtLogin"));
            caixaDeLogin.SendKeys(loginDoQ);
            IWebElement caixaDeSenha = driver.FindElement(By.Id("txtSenha"));
            caixaDeSenha.SendKeys(senhaDoQ);
            IWebElement logar = driver.FindElement(By.Id("btnOk"));
            logar.Click();
            System.Threading.Thread.Sleep(2000);//Esperar 2 segundos até a pagina inicial carregar
            driver.Url = Pagina;
        }

        //Função que lê o arquivo de configuração para obter as opções
        public void LerConfig()
        {
            var appSettings = ConfigurationManager.AppSettings;
            Pagina = appSettings["Pagina"];
            IntervaloString = appSettings["Intervalo"];
            int.TryParse(IntervaloString, out intervalo);
            Navegador = appSettings["Navegador"];
        }

        //Função que faz o programa dormir no intervalo configurado e depois atualiza a página
        public void SleepRefresh()
        {
            System.Threading.Thread.Sleep(intervalo);
            driver.Navigate().Refresh();
        }

        //Fecha o driver
        public void Fechar()
        {
            driver.Quit();
        }
    }
}
