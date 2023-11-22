using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace РГЗ_фронт
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Image> photos = new ObservableCollection<Image>();

        public void AddPhoto(string base64string)
        {
            byte[] binaryData = Convert.FromBase64String(base64string);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();

            Image img = new Image();
            img.Source = bi;

            photos.Add(img);
        }

        private void ClearPhotos()
        {
            photos.Clear();
        }


        public MainWindow()
        {
            InitializeComponent();
            ViewerPhotos.ItemsSource = photos;
        }

        private void GetPhotos_Click(object sender, RoutedEventArgs e)
        {
            ClearPhotos();

            // Получение фотографий с сервера
            string path = Path.Text;    // Получение пути к папке с фотографиями
            string[] base64Strings = { }; // TODO: get photo src base64;

            // Запись фотографий
            foreach(string base64String in base64Strings)
            {
                AddPhoto(base64String);
            }
        }
    }
}
