using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class ViewModelSend
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public ViewModelSend(string Message, int Id)
        {
            this.Message = Message;
            this.Id = Id;
        }
    }
}
