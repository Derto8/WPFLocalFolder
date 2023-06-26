using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ClientGUI.Classies
{
    internal static class StaticData
    {
        public static IPAddress Ip { get; set; }
        public static int Port { get; set; }
        public static string Login { get; set; }
        public static string Password { get; set; }
        public static Socket Socket { get; set; }
    }
}
