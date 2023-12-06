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
            
        }

        private void GoExit(Object sender, EventArgs e)
        {
            Client.ClosePort();
            this.Close();
        }
    }
}
