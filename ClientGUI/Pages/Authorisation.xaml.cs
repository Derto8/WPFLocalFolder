using ClientGUI.Classies;
using System;
using System.Collections.Generic;
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

namespace ClientGUI.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorisation.xaml
    /// </summary>
    public partial class Authorisation : Page
    {
        MainWindow mainWindow;
        public Authorisation(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Auth(object sender, RoutedEventArgs e)
        {
            StaticData.Login = Login.Text;
            StaticData.Password = Password.Text;

            MainWindow.GetSetMessage($"connect {StaticData.Login} {StaticData.Password}");

            //if (int.TryParse(StaticData.Port.ToString(), out Port) && IPAddress.TryParse(StaticData.Ip.ToString(), out IpAdress))
            //{
            //    MainWindow.ConnectServer($"connect {login} {password}", null); 
            //}
            mainWindow.OpenPage(MainWindow.pages.main);
        }
    }
}
