using System.Windows;

namespace РГЗ_фронт
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
                string baudRate = BaudRate.Text;

                Client.SetPortName(name);
                Client.SetPortBaudRate(baudRate);
                Client.OpenPort();
                Client.FormMessage("hello", null);

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
