using OpenQA.Selenium;
using StreamLinkBussinessLayer;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace StreamLink
{
    public class Program
    {
        public static Session ReadSessionFromFile(string fileName)
        {
            Session session = null;
            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        session = Session.Get(values[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }

            return session;
        }

        public static void WriteSessionToFile(string fileName, string sessionId)
        {
            try
            {
                using (var writer = new StreamWriter(fileName))
                {
                    writer.WriteLine(sessionId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing CSV file: {ex.Message}");
            }
        }

        public static void CreateSession(int userId)
        {
            Session session = new Session(userId);

            if(session.Save())
            {
                WriteSessionToFile("session.csv", session.SessionId);
            }
        }
        public static void Login()
        {
            Session session = null;
            User user = null;

            if ((session = ReadSessionFromFile("session.csv")) != null)
            {
                user = User.Get(session.UserId);
                Console.WriteLine(user.ToString());

                return;
            }

            string username = String.Empty;
            string password = String.Empty;

            Console.WriteLine("Enter username: ");
            username = Console.ReadLine();

            Console.WriteLine("Enter password: ");
            password = Console.ReadLine();

            if((user = User.IsCorrectCredentials(username, password)) != null)
            {
                CreateSession(user.UserId);
                Console.WriteLine(user.ToString());

            }
            else
            {
                Console.WriteLine("Wrong credentials");
            }
        }
        static int Main(string[] args)
        {
            Login();

            Source src = Source.Get(1);

            src.Save();

            return 0;
        }
    }
}
 