using System.Windows;

namespace РГЗ_фронт2
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

                // TODO: Ira auth

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
