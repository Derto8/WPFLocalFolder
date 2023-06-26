using ClientGUI.Classies;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int Id = -1;
        public MainWindow()
        {
            InitializeComponent();
            OpenPage(pages.connect);
        }

        public enum pages
        {
            connect,
            auth,
            main
        }
        public void OpenPage(pages _pages)
        {
            if (_pages == pages.auth)
                frame.Navigate(new Pages.Authorisation(this));
            if (_pages == pages.connect)
                frame.Navigate(new Pages.ConnectToServer(this));
            if (_pages == pages.main)
                frame.Navigate(new Pages.Main(this));
        }

        public static List<string> CD(string message)
        {
            ViewModelMessage viewModelMessage = GetSetMessage(message);
            List<string> FoldersFiles = new List<string>();
            FoldersFiles = JsonConvert.DeserializeObject<List<string>>(viewModelMessage.Data);
            return FoldersFiles;
        }

        public static void Get(string message)
        {
            ViewModelSend viewModelSend = new ViewModelSend(message, Id);
            ViewModelMessage viewModelMessage = GetSetMessage(message);
            if (viewModelMessage.Command == "file")
            {
                FileServer(viewModelSend, viewModelMessage);
            }
        }

        public static void Set(string message)
        {
            ViewModelSend viewModelSend = new ViewModelSend(message, Id);
            string[] DataMessage = message.Split(new string[1] { " " }, StringSplitOptions.None);
            string NameFile = "";
            for (int i = 1; i < DataMessage.Length; i++)
                if (NameFile == "")
                    NameFile += DataMessage[i];
                else
                    NameFile += " " + DataMessage[i];

            string[] m = message.Split(" ");

            if (File.Exists(m[1]))
            {
                FileInfo FileInfo = new FileInfo(m[1]);
                FileInfoFTP NewFileInfo = new FileInfoFTP(File.ReadAllBytes(m[1]), FileInfo.Name);
                viewModelSend = new ViewModelSend(JsonConvert.SerializeObject(NewFileInfo), Id);
                byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
                int BytesSend = StaticData.Socket.Send(messageByte);
                byte[] bytes = new byte[10485760];
                int BytesRec = StaticData.Socket.Receive(bytes);
                string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
            }
            else
            {
                MessageBox.Show("Указанный файл не существует");
                return;
            }
            //StaticData.Socket = ConnectServer();
            //ViewModelMessage viewModelMessage = GetSetMessage(message);
            //if (viewModelMessage.Command == "file")
            //{
            //    FileServer(viewModelSend, viewModelMessage);
            //}
        }

        private static void FileServer(ViewModelSend viewModelSend, ViewModelMessage viewModelMessage)
        {
            string[] DataMes = viewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
            string getFile = "";
            for (int i = 1; i < DataMes.Length; i++)
                if (getFile == "")
                    getFile = DataMes[i];
                else
                    getFile += " " + DataMes[i];
            byte[] byteFile = JsonConvert.DeserializeObject<byte[]>(viewModelMessage.Data);
            File.WriteAllBytes(getFile, byteFile);
        }

        public static Socket ConnectServer()
        {
            IPEndPoint endPoint = new IPEndPoint(StaticData.Ip, StaticData.Port);
            Socket socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            socket.Connect(endPoint);
            return socket;
        }
        
        public static ViewModelMessage GetSetMessage(string message)
        {
            ViewModelSend viewModelSend = new ViewModelSend(message, Id);
            byte[] messageByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelSend));
            int BytesSend = StaticData.Socket.Send(messageByte);
            byte[] bytes = new byte[10485760];
            int BytesRec = StaticData.Socket.Receive(bytes);
            string messageServer = Encoding.UTF8.GetString(bytes, 0, BytesRec);
            ViewModelMessage viewModelMessage = JsonConvert.DeserializeObject<ViewModelMessage>(messageServer);
            if (viewModelMessage.Command == "autorization")
                Id = int.Parse(viewModelMessage.Data); 
            return viewModelMessage;
        }
    }
}
