using System;
using System.Windows;

namespace РГЗ_фронт2
{
    /// <summary>
    /// Логика взаимодействия для SettingsComPortWindow.xaml
    /// </summary>
    public partial class SettingsComPortWindow : Window
    {
        public SettingsComPortWindow()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = Name.Text;
                int baudRate = Convert.ToInt32(BaudRate.Text);

                // TODO: Ira connect

                ((this.Owner) as CustomMenuWindow).isConnect = true;
                this.Close();
            }
            catch
            {
                ((this.Owner) as CustomMenuWindow).isConnect = false;
            }
        }
    }
}
