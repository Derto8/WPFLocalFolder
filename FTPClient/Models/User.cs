using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace FTPClient.Models
{
    internal class User
    {
        public ObjectId Id { get; set; }
        public int IdUser { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string src { get; set; }
        public string temp_src { get; set; }
        public User(int id, string login, string password, string src)
        {
            IdUser = id;
            this.login = login;
            this.password = password;
            this.src = src;
            temp_src = src;
        }
    }
}
