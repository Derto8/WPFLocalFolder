using ClientGUI.Classies;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace ClientGUI.Pages
{
    /// <summary>
    /// Логика взаимодействия для ConnectToServer.xaml
    /// </summary>
    public partial class ConnectToServer : Page
    {
        MainWindow mainWindow;
        public ConnectToServer(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                // решить проблему с подключением.
                StaticData.Ip = IPAddress.Parse(Ip.Text);
                StaticData.Port = int.Parse(Port.Text);
                StaticData.Socket = MainWindow.ConnectServer();
                mainWindow.OpenPage(MainWindow.pages.auth);
            }
            catch
            {
                MessageBox.Show("Вы ввели некорректные данные!");
            }
        }
    }
}
