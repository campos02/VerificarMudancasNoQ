using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI_VerificarMudancasNoQ
{
    public partial class LoginSenha : Window
    {
        private MainWindow main;
        private Verificacao verificacao;
        public LoginSenha()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            verificacao = new Verificacao(textbox1.Text, passwordbox1.Password);
            main = new MainWindow(verificacao);
            verificacao.MainWindow = main;
            textbox1.Text = "";
            passwordbox1.Password = "";
        }
    }
}
