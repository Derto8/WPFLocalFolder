using ClientGUI.Classies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ClientGUI.Pages
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        MainWindow mainWindow;
        string command = "cd";
        string currentDirectory = "cd";
        string nameFile = "";
        public Main(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            StaticData.Socket = MainWindow.ConnectServer();
            List<string> data = MainWindow.CD(command);
            CreateFolders(data);
        }

        private void CreateFolders(List<string> folders)
        {
            int margin = 0;

            for (int i = 0; i < folders.Count; i++)
            {
               // char[] charFolderI = folders[i].ToCharArray().Where(n => !char.IsDigit(n) && !char.IsControl(n)).ToArray();              
               // string folderI = new string(charFolderI).Replace(" ", "");

                BrushConverter bc = new BrushConverter();
                Grid global = new Grid();
                global.Height = 140;
                global.Width = 140;
                global.HorizontalAlignment = HorizontalAlignment.Left;
                global.VerticalAlignment = VerticalAlignment.Top;
                global.Background = (Brush)bc.ConvertFrom("#FFECECEC");
                global.Margin = new Thickness(margin, 0, 0, 0);
              //  global.Name = folderI;
               // global.MouseLeftButtonDown += grid_MouseDoubleClick;
                Button button = new Button();
                button.Height = 140;
                button.Width = 140;
                button.Content = folders[i];
                button.MouseDoubleClick += Button_MouseDoubleClick;
                button.Opacity = 0;
                global.Children.Add(button);

                Image logo = new Image();
                logo.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\Images\folder.png"));
                logo.HorizontalAlignment = HorizontalAlignment.Center;
                logo.Height = 100;
                logo.Margin = new Thickness(0, 0, 0, 0);
                logo.VerticalAlignment = VerticalAlignment.Top;
                logo.Width = 140;
                global.Children.Add(logo);

                Label name = new Label();
                name.Content = folders[i];
                name.MouseDoubleClick += Label_MouseDoubleClick;
                name.HorizontalAlignment = HorizontalAlignment.Center;
                name.VerticalAlignment = VerticalAlignment.Top;
                name.Margin = new Thickness(0, 100, 0, 0);
                name.FontWeight = FontWeights.Bold;
                global.Children.Add(name);

                parrent.Children.Add(global);
                margin += 150;
            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            nameFile = ((Label)sender).Content.ToString();
            //nameFile = @"C:\Авиатехникум\image.txt";
        }

        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            parrent.Children.Clear();
            string name = ((Button)sender).Content.ToString();
            if (name.Last() != '/')
                name = name + "/";
            
            StaticData.Socket = MainWindow.ConnectServer();
            // List<string> data = MainWindow.CD(currentDirectory + @$" \{name}/");
            List<string> data = MainWindow.CD(command + " " + name);
            CreateFolders(data);
        }

        private void BackCD(object sender, RoutedEventArgs e)
        {
            parrent.Children.Clear();
            StaticData.Socket = MainWindow.ConnectServer();
            List<string> data = MainWindow.CD(command + " ..");
            CreateFolders(data);
        }

        private void Download(object sender, RoutedEventArgs e)
        {
            StaticData.Socket = MainWindow.ConnectServer();
            MainWindow.Get("get " + nameFile);
            MessageBox.Show("Файл скачан");
        }

        private void AddFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            // openDialog.Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF, *.TXT)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf, *.txt";
            openDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            try
            {
                if (openDialog.ShowDialog() != true)
                {
                    MessageBox.Show("Не удалось открыть фотографию");
                    return;
                }
            }
            catch { }
            string path = openDialog.FileName;
            string[] p = path.Split(@"\");
            StaticData.Socket = MainWindow.ConnectServer();
            MainWindow.Set("set " + p.Last());

            StaticData.Socket = MainWindow.ConnectServer();
            List<string> data = MainWindow.CD("cd");
            CreateFolders(data);
        }
    }
}
