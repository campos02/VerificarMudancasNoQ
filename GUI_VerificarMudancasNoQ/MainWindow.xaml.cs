using System;
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

namespace GUI_VerificarMudancasNoQ
{
    public partial class MainWindow : Window
    {
        private Verificacao verificacao;
        //inicia o ícone na área de notificação e seus respectivos menus
        System.Windows.Forms.NotifyIcon notifyicon1 = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenuStrip contextmenu1 = new System.Windows.Forms.ContextMenuStrip();
        System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
        private readonly Regex numeros_regex = new Regex("[^0-9]+");
        public MainWindow(Verificacao m_verificacao)
        {
            verificacao = m_verificacao;
            InitializeComponent();
            /*configura o ícone na área de notificação e seus menus, adicionando ícone, menu com duas opções e tornando-as funcionais:
            "Configurações" exibe a janela de mesmo nome e "Sair" fecha o programa*/
            notifyicon1.Icon = Properties.Resources.Icone_Q;
            notifyicon1.ContextMenuStrip = contextmenu1;
            notifyicon1.BalloonTipTitle = "Mudanças";
            notifyicon1.BalloonTipText = "Foram detectadas mudanças na página verificada";
            notifyicon1.Text = "Iniciando...";
            contextmenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem1,
            toolStripMenuItem2});
            contextmenu1.Name = "contextmenu1";
            contextmenu1.Size = new System.Drawing.Size(152, 48);
            toolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            toolStripMenuItem2.Size = new System.Drawing.Size(151, 22);
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem1.Text = "Configurações";
            toolStripMenuItem2.Text = "Sair";
            toolStripMenuItem1.Click += new System.EventHandler(this.mostrar);
            toolStripMenuItem2.Click += new System.EventHandler(this.Sair);
            notifyicon1.Visible = true;
            //esconde esta janela e exibe as configs salvas
            this.Hide();
            exibir_configs_salvas();
            //roda as tarefas de iniciar o driver e do loop de verificação, se houver exceções em iniciar_driver as mostra em caixas de mensagem e fecha o programa
            Task abrir_site = Task.Run(() => verificacao.login_p.iniciar_driver());
            try
            {
                abrir_site.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    MessageBox.Show(e.Message, "Exceção");
                }
                Application.Current.Shutdown();
            }
            verificacao.verificar_acesso();
            Task loop_verificar = Task.Run(() =>
            {
                while (true)
                {
                    verificacao.verificar_texto();
                }
            });
            notifyicon1.Text = "Verificando página configurada...";
        }

        private void mostrar(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void mostrar(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void Sair(object sender, EventArgs e)
        {
            verificacao.login_p.fechar();
            Application.Current.Shutdown();
        }

        private void Sair(object sender, RoutedEventArgs e)
        {
            verificacao.login_p.fechar();
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Salvar_Configs();
        }

        private void Salvar_Configs()
        {
            /*salva o texto das caixas nas configurações de usuário, salvando "chrome" ou "firefox" dependendo do índice da combobox,
            no final mostra uma mensagem dizendo para reiniciar o programa*/
            Properties.Settings.Default.Pagina = textbox1.Text;
            Properties.Settings.Default.Intervalo = textbox2.Text;
            if (combobox1.SelectedIndex == 0)
            {
                Properties.Settings.Default.Navegador = "chrome";
            }
            if (combobox1.SelectedIndex == 1)
            {
                Properties.Settings.Default.Navegador = "firefox";
            }
            Properties.Settings.Default.Save();
            MessageBox.Show(this, "Reinicie este programa para que as novas configurações sejam aplicadas");
        }

        private void exibir_configs_salvas()
        {
            //pega as configurações de usuário atuais e as coloca em suas respectivas caixas de além e no caso da combobox seleciona a opção correta
            textbox1.Text = Properties.Settings.Default.Pagina;
            textbox2.Text = Properties.Settings.Default.Intervalo;
            if (Properties.Settings.Default.Navegador == "chrome")
            {
                combobox1.SelectedIndex = 0;
            }
            if (Properties.Settings.Default.Navegador == "firefox")
            {
                combobox1.SelectedIndex = 1;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
            exibir_configs_salvas();
        }

        public void notificacao_de_mudancas()
        {
            notifyicon1.ShowBalloonTip(3000);//mostra o balão por três segundos
        }

        private bool Texto_Permitido(string texto)
        {
            return numeros_regex.IsMatch(texto);//retorna true somente se o texto corresponder ao regex
        }

        //lida com o evento somente se a função Texto_Permtido retornar true
        private void Validacao_Numeros(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Texto_Permitido(e.Text);
        }

        ~MainWindow()
        {
            verificacao.login_p.fechar();
        }
    }
}
