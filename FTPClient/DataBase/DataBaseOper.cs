using DnsClient;
using FTPClient.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FTPClient.DataBase
{
    internal class DataBaseOper
    {
        private static MongoClient client = new MongoClient("mongodb://localhost:27017");
        private static IMongoDatabase database = client.GetDatabase("ClientServer");
        //private static IMongoDatabase database = client.GetDatabase("Practica4");

        private IMongoCollection<User> GetCollectionUsers()
        {
            return database.GetCollection<User>("Users");
        }
        private IMongoCollection<BsonDocument> GetCollectionCommand()
        {
            return database.GetCollection<BsonDocument>("CommandsUsers");
        }
        private int AutoIncrementId()
        {
            IMongoCollection<User> users = GetCollectionUsers();
            return Convert.ToInt32(users.CountDocuments(new BsonDocument()));
        }
        public List<User> GetUsers()
        {
            IMongoCollection<User> users = GetCollectionUsers();
            return users.Find(new BsonDocument()).ToList();
        }

        public User AddUser(string login, string password)
        {
            IMongoCollection<User> users = GetCollectionUsers();
            User user = new User(AutoIncrementId(), login, password, @"C:\Авиатехникум");
            users.InsertOne(user);
            Console.WriteLine("Пользователь был зарегистрирован");
            return user;
        }

        public void AddCommandUser(string Command, string Login)
        {
            IMongoCollection<BsonDocument> commandCollection = GetCollectionCommand();
            BsonDocument userCommand = new BsonDocument()
            {
                {"login", $"{Login}" },
                {"command", $"{Command}" }
            };
            commandCollection.InsertOne(userCommand);
            Console.WriteLine($"Команда пользователя: {Login}, была сохранена");
        }

        public User GetUser(string login, string password)
        {
            IMongoCollection<User> users = GetCollectionUsers();
            User user = users.Find(new BsonDocument { { "login", $"{login}" }, { "password", $"{password}" } }).FirstOrDefault();
            return user;
        }
    }
}
