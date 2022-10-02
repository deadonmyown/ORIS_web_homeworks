using System;
using System.Net;
using System.IO;
using GoogleHttpServer;
 
namespace NetConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //bool serverOn = true;
            
            HttpServer server = new HttpServer("http://localhost:1337/");
            
            server.Start();

            /*while (serverOn)
            {
                Console.WriteLine("Type command (start / stop / restart / exit):");
                string command = ReadValue();
                switch (command)
                {
                    case "start":
                        server.Start();
                        break;
                    case "stop":
                        server.Stop();
                        break;
                    case "restart":
                        server.Restart();
                        break;
                    case "exit":
                        server.Stop();
                        serverOn = false;
                        break;
                    default:
                        Console.WriteLine("Wrong type");
                        break;
                }
            }*/
        }
        
        //private static string? ReadValue() => Console.ReadLine();
    }
}