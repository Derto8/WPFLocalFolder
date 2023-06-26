using System;
using System.Collections.Generic;
using System.Text;

namespace FTPClient.Models
{
    internal class CommandUser
    {
        public int Id { get; set; }
        public string Command { get; set; }
        public User User { get; set; }
    }
}
