using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace РГЗ_фронт
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Image> photos = new ObservableCollection<Image>();
        private ObservableCollection<string> files;
        private string selectedItem;

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
            string[] v = Client.FormMessage("pwd", null);
            Path.Text = v[0];

            string[] filesFromServer = Client.FormMessage("ls", null); 

            files = new ObservableCollection<string>(filesFromServer);

            ViewerDirectory.ItemsSource = files;
            ViewerPhotos.ItemsSource = photos;
        }

        private void GetPhotos_Click(object sender, RoutedEventArgs e)
        {
            ClearPhotos();

            try
            {
                // Получение фотографий с сервера
                string path = "./" + selectedItem;    // Относительный путь к папке с фотографиями или к фотографии
                
                string[] v = Client.FormMessage("pwd", null);
                string pwd = v[0]; // v[0];
                pwd = pwd.Trim(new char[] { '.' }) + "/"; // результат "/photoBase/dir/"
                
                string[] base64Strings;
                Stopwatch stopWatch = new Stopwatch();
                TimeSpan ts;
                if (selectedItem.Contains(".")) // Если имеет расширение -> файл
                {
                    stopWatch.Start();
                    base64Strings = new string[] { Client.FormMessage("get", selectedItem)[0] };
                    stopWatch.Stop();
                    ts = stopWatch.Elapsed;
                    ViewerPhotos.Style = (Style)Resources["OnePhoto"];
                    TempFilePhotoVewing.Text = "Из файла - " + pwd + selectedItem;
                }
                else   // Если не имеет расширение -> папка
                {
                    stopWatch.Start();
                    base64Strings = Client.FormMessage("get2", selectedItem);
                    stopWatch.Stop();
                    ts = stopWatch.Elapsed;
                    ViewerPhotos.Style = (Style)Resources["ManyPhotos"];
                    TempFilePhotoVewing.Text = "Из папки - " + pwd + selectedItem;
                }

                string elapsedTime = String.Format("{0:00}:{1:00}", ts.Seconds, ts.Milliseconds / 10);
                //MessageBox.Show(elapsedTime);

                Console.WriteLine("RunTime " + elapsedTime);
                // Запись фотографий
                foreach (string base64String in base64Strings)
                {
                    AddPhoto(base64String);
                }
            }
            catch (System.NullReferenceException ex)
            {
                if (selectedItem == null)
                {
                    MessageBox.Show("Не был выбран элемент(папка/файл), который необходимо просмотреть.");
                }
            }


        }

        private void ViewerDirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = (string)(sender as ListBox).SelectedItem;
        }

        private void ClearFiles()
        {
            files.Clear();
        }
        private void AddFile(string fileFromServer)
        {
            files.Add(fileFromServer);
        }

        private void ViewerDirectory_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                string directoryToGo = (string)(sender as ListBox).SelectedItem;

                Client.FormMessage("cd", directoryToGo);

                string[] v = Client.FormMessage("pwd", null);
                string pwd = v[0];
                pwd = pwd.Trim(new char[] { '.' });

                ClearFiles();

                if (!pwd.Equals("photoBase"))
                {
                    AddFile("../");
                }

                Path.Text = pwd;

                string[] filesFromServer = Client.FormMessage("ls", null);

                foreach (string fileFromServer in filesFromServer)
                    AddFile(fileFromServer);
            }
            catch
            {

            }

        }

        private void CloseWindow(object sender, EventArgs e)
        {
            ((this.Owner)as CustomMenuWindow).MainChild = null;
            this.Owner.Visibility = Visibility.Visible;
        }

        private void GoReturn(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            this.Owner.Visibility = Visibility.Visible;
        }


    }
}
