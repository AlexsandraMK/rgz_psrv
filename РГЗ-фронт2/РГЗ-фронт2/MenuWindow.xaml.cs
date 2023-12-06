using System;
using System.Windows;

namespace РГЗ_фронт
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class CustomMenuWindow : Window
    {
        public bool isAuth = false;
        public MainWindow MainChild = null;
        internal bool isConnect;

        public CustomMenuWindow()
        {
            InitializeComponent();
            Client.CreatePort();
        }

        private void GoAuth(Object sender, EventArgs e)
        {
            if(isConnect == false)
            {
                MessageBox.Show("Необходимо подключится к COM-порту");
            }
            else
            {
                AuthWindow window = new AuthWindow
                {
                    Owner = this
                };
                window.ShowDialog();
            }
        }

        private void GoSettingsComPort(Object sender, EventArgs e)
        {
            SettingsComPortWindow window = new SettingsComPortWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void GoShowImages(Object sender, EventArgs e)
        {
            if (isConnect == false)
            {
                MessageBox.Show("Необходимо подключится к COM-порту");
            }
            else if (isAuth == false)
            {
                MessageBox.Show("Необходимо авторизоваться");
            }
            else
            {
                this.Visibility = Visibility.Hidden;
                if (MainChild == null)
                    MainChild = new MainWindow
                    {
                        Owner = this
                    };
                MainChild.ShowDialog();
            }  
        }

        private void GoHelp(Object sender, EventArgs e)
        {
            Help window = new Help
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void GoExit(Object sender, EventArgs e)
        {
            Client.ClosePort();
            this.Close();
        }
    }
}
