using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace РГЗ_фронт
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Auth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = Login.Text;
                string password = Password.Text;

                Client.FormMessage("auth", login + " " + password);

                ((this.Owner) as CustomMenuWindow).isAuth = true;
                this.Close();
            }
            catch
            {
                ((this.Owner) as CustomMenuWindow).isAuth = false;
            }

        }
    }
}
