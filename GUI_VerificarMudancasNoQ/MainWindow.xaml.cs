﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using OpenQA.Selenium;

namespace GUI_VerificarMudancasNoQ
{
    public partial class MainWindow : Window
    {
        private Verificacao verificacao;
        //inicia o ícone na área de notificação e seus respectivos menus
        System.Windows.Forms.NotifyIcon notifyIcon1 = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenuStrip contextMenu1 = new System.Windows.Forms.ContextMenuStrip();
        System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
        private readonly Regex numerosRegex = new Regex("[^0-9]+");
        public MainWindow(Verificacao verificacao)
        {
            this.verificacao = verificacao;
            InitializeComponent();
            /*configura o ícone na área de notificação e seus menus, adicionando ícone, menu com duas opções e tornando-as funcionais:
            "Configurações" exibe a janela de mesmo nome e "Sair" fecha o programa*/
            notifyIcon1.Icon = Properties.Resources.Icone_Q;
            notifyIcon1.ContextMenuStrip = contextMenu1;
            notifyIcon1.BalloonTipTitle = "Mudanças";
            notifyIcon1.BalloonTipText = "Foram detectadas mudanças na página verificada";
            notifyIcon1.Text = "Iniciando...";
            contextMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem1,
            toolStripMenuItem2});
            contextMenu1.Name = "contextmenu1";
            contextMenu1.Size = new System.Drawing.Size(152, 48);
            toolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            toolStripMenuItem2.Size = new System.Drawing.Size(151, 22);
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem1.Text = "Configurações";
            toolStripMenuItem2.Text = "Sair";
            toolStripMenuItem1.Click += new System.EventHandler(this.Mostrar);
            toolStripMenuItem2.Click += new System.EventHandler(this.Sair);
            notifyIcon1.Visible = true;
            //esconde esta janela e exibe as configs salvas
            this.Hide();
            ExibirConfigsSalvas();
            //Roda a tarefa de iniciar a verificação
            Task.Run(() => IniciarVerificacao());
        }

        private void Mostrar(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void Mostrar(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void Sair(object sender, EventArgs e)
        {
            verificacao.Login.Fechar();
            Application.Current.Shutdown();
        }

        private void Sair(object sender, RoutedEventArgs e)
        {
            verificacao.Login.Fechar();
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SalvarConfigs();
        }

        private void SalvarConfigs()
        {
            /*salva o texto das caixas nas configurações, salvando "chrome" ou "firefox" dependendo do índice da combobox,
            no final mostra uma mensagem dizendo para reiniciar o programa*/
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configs = configFile.AppSettings.Settings;
            configs["Pagina"].Value = textbox1.Text.Trim();
            configs["Intervalo"].Value = textbox2.Text.Trim();
            if (combobox1.SelectedIndex == 0)
            {
                configs["Navegador"].Value = "chrome";
            }
            if (combobox1.SelectedIndex == 1)
            {
                configs["Navegador"].Value = "firefox";
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            MessageBox.Show(this, "Reinicie este programa para que as novas configurações sejam aplicadas");
        }

        private void ExibirConfigsSalvas()
        {
            //pega as configurações de usuário atuais e as coloca em suas respectivas caixas de além e no caso da combobox seleciona a opção correta
            var appSettings = ConfigurationManager.AppSettings;
            textbox1.Text = appSettings["Pagina"];
            textbox2.Text = appSettings["Intervalo"];
            if (appSettings["Navegador"] == "chrome")
            {
                combobox1.SelectedIndex = 0;
            }
            if (appSettings["Navegador"] == "firefox")
            {
                combobox1.SelectedIndex = 1;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ExibirConfigsSalvas();
        }

        public void NotificacaoDeMudancas()
        {
            notifyIcon1.ShowBalloonTip(3000);//mostra o balão por três segundos
        }

        private bool TextoPermitido(string texto)
        {
            return numerosRegex.IsMatch(texto);//retorna true somente se o texto corresponder ao regex
        }

        //lida com o evento somente se a função Texto_Permtido retornar true
        private void ValidacaoNumeros(object sender, TextCompositionEventArgs e)
        {
            e.Handled = TextoPermitido(e.Text);
        }

        //De forma assíncrona muda o text do ícone de notificação para o do parâmetro texto
        public async Task SetTextoIconeNotificacaoAsync(string texto)
        {
            await Task.Run(() => { notifyIcon1.Text = texto; });
        }

        //Se houver exceções de acesso negado ou driver não encontrado as mostra em caixas de mensagem e retorna a função, a encerrando
        public async Task IniciarVerificacao()
        {
            try
            {
                verificacao.IniciarVerificarAcesso();
            }
            catch (AcessoNegadoException)
            {
                MessageBox.Show("Reinicie este programa e verifique login e senha!", "Exceção");
                return;
            }
            catch (DriverServiceNotFoundException)
            {
                MessageBox.Show("Binário de driver do selenium não encontrado! Coloque um binário e reinicie o programa", "Exceção");
                return;
            }
            await SetTextoIconeNotificacaoAsync("Verificando página configurada...");
            while (true)
            {
                verificacao.VerificarTexto();
            }
        }

        ~MainWindow()
        {
            verificacao.Login.Fechar();
        }
    }
}
