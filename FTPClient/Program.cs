using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;
using FTPClient.DataBase;
using FTPClient.Models;
using Newtonsoft.Json;

namespace FTPClient
{
    internal class Program
    {
        public static List<User> Users = new List<User>();
        public static IPAddress IpAdress;
        public static int Port;
        private static DataBaseOper db = new DataBaseOper();
        private static User User { get; set; }
        static void Main(string[] args)
        {
            Users = db.GetUsers();
            Console.WriteLine("Введите IP адрес сервера: ");
            string sIpAdress = Console.ReadLine();
            Console.WriteLine("Введите порт: ");
            string sPort = Console.ReadLine();
            if (int.TryParse(sPort, out Port) && IPAddress.TryParse(sIpAdress, out IpAdress))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Данные успешно введены. Запускаю сервер.");
                StartServer();
            }
            Console.Read();
        }

        public static User AutorizationUser(string login, string password)
        {
            User user = db.GetUser(login, password);
            if (user == null)
                user = db.AddUser(login, password);
            return user;
        }

        public static List<string> GetDirectory(string src)
        {
            List<string> FoldersFiles = new List<string>();
            if (Directory.Exists(src))
            {
                string[] dirs = Directory.GetDirectories(src);
                foreach (string dir in dirs)
                {
                    string NameDirectory = dir.Replace(src, "");
                    FoldersFiles.Add(@NameDirectory + "/");
                }
                string[] files = Directory.GetFiles(src);
                foreach (string file in files)
                {
                    string NameFile = file.Replace(src, "");
                    FoldersFiles.Add(NameFile);
                }
            }
            return FoldersFiles;
        }

        public static void StartServer()
        {
            IPEndPoint endPoint = new IPEndPoint(IpAdress, Port);
            Socket sListener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
                );

            sListener.Bind(endPoint);
            sListener.Listen(10);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Сервер запущен");
            while (true)
            {
                try
                {
                    Socket Handler = sListener.Accept();
                    string Data = null;
                    byte[] Bytes = new byte[10485760];
                    int BytesRec = Handler.Receive(Bytes);
                    Data += Encoding.UTF8.GetString(Bytes, 0, BytesRec);
                    Console.WriteLine("Сообщение от пользователя: " + Data + "\n");
                    string Reply = "";
                    ViewModelSend viewModelSend = JsonConvert.DeserializeObject<ViewModelSend>(Data);
                    if (viewModelSend != null)
                    {
                        ViewModelMessage viewModelMessage;
                        string[] DataCommand = viewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                        if (DataCommand[0] == "connect")
                        {
                            string[] DataMessage = viewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                            AutorizationUser(DataMessage[1], DataMessage[2]);
                            User = AutorizationUser(DataMessage[1], DataMessage[2]);
                            viewModelMessage = new ViewModelMessage("autorization", User.IdUser.ToString());
                            db.AddCommandUser("autorization", User.login);
                            Reply = JsonConvert.SerializeObject(viewModelMessage);
                            byte[] message = Encoding.UTF8.GetBytes(Reply); 
                            Handler.Send(message);
                        }
                        else if (DataCommand[0] == "cd")
                        {
                            if (viewModelSend.Id != -1)
                            {
                                Users = db.GetUsers();
                                string[] DataMessage = viewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                                List<string> FoldersFiles = new List<string>();
                                if (DataMessage.Length == 1)
                                {
                                    Users[viewModelSend.Id].temp_src = Users[viewModelSend.Id].src;
                                    FoldersFiles = GetDirectory(Users[viewModelSend.Id].src);
                                }
                                else
                                {
                                    string cdFolder = "";
                                    for (int i = 1; i < DataMessage.Length; i++)
                                    {
                                        if (cdFolder == "")
                                            cdFolder += DataMessage[i];
                                        else
                                            cdFolder += " " + DataMessage[i];
                                    }
                                    Users[viewModelSend.Id].temp_src = Users[viewModelSend.Id].temp_src + cdFolder;
                                    FoldersFiles = GetDirectory(Users[viewModelSend.Id].temp_src);
                                }
                                if (FoldersFiles.Count == 0)
                                {
                                    viewModelMessage = new ViewModelMessage("message", "Директория пуста или не существует");
                                    db.AddCommandUser("message", User.login);
                                }
                                else
                                {
                                    viewModelMessage = new ViewModelMessage("cd", JsonConvert.SerializeObject(FoldersFiles));
                                    db.AddCommandUser("cd", User.login);
                                }
                            }
                            else
                            {
                                viewModelMessage = new ViewModelMessage("message", "Необходимо авторизироваться");
                                db.AddCommandUser("message", User.login);
                            }
                            Reply = JsonConvert.SerializeObject(viewModelMessage);
                            byte[] message = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(message);
                        }
                        else if (DataCommand[0] == "get")
                        {
                            if (viewModelSend.Id != -1)
                            {
                                string[] DataMessage = viewModelSend.Message.Split(new string[1] { " " }, StringSplitOptions.None);
                                string getFile = "";
                                for (int i = 1; i < DataMessage.Length; i++)
                                {
                                    if (getFile == "")
                                        getFile += DataMessage[i];
                                    else
                                        getFile += " " + DataMessage[i];
                                }
                                byte[] byteFile = File.ReadAllBytes(Users[viewModelSend.Id].temp_src + getFile);
                                viewModelMessage = new ViewModelMessage("file", JsonConvert.SerializeObject(byteFile));
                                db.AddCommandUser("file", User.login);
                            }
                            else
                            {
                                viewModelMessage = new ViewModelMessage("message", "Необходимо авторизироваться");
                                db.AddCommandUser("message", User.login);
                            }
                            Reply = JsonConvert.SerializeObject(viewModelMessage);
                            byte[] message = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(message);
                        }
                        else
                        {
                            if (viewModelSend.Id != -1)
                            {
                                FileInfoFTP SendFileInfo = JsonConvert.DeserializeObject<FileInfoFTP>(viewModelSend.Message);
                                File.WriteAllBytes(Users[viewModelSend.Id].temp_src + @"\" + SendFileInfo.Name, SendFileInfo.Data);
                                viewModelMessage = new ViewModelMessage("message", "Файл загружен");
                                db.AddCommandUser("message", User.login);
                            }
                            else
                            {
                                viewModelMessage = new ViewModelMessage("message", "Необходимо авторизироваться");
                                db.AddCommandUser("message", User.login);
                            }
                            Reply = JsonConvert.SerializeObject(viewModelMessage);
                            byte[] message = Encoding.UTF8.GetBytes(Reply);
                            Handler.Send(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }
        }
    }
}
