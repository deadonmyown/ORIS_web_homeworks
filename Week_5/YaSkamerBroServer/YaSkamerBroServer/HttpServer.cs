using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YaSkamerBroServer
{
    public class HttpServer : IDisposable
    {
        private static object _locker = new object();
        public ServerStatus ServerStatus { get; private set; } = ServerStatus.Stop;
        
        private readonly HttpListener _httpListener;
        
        private volatile ServerSettings _serverSettings;

        private Task listeningTask;
        
        public HttpServer()
        {
            _httpListener = new HttpListener();
        }
        
        public void Start()
        {
            if (ServerStatus == ServerStatus.Start)
            {
                Console.WriteLine("Сервер уже был запущен");
                return;
            }
            
            _serverSettings = ServerFileHandling.ReadJsonSettings("./settings.json");
            
            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add($"http://localhost:{_serverSettings.Port}/");

            Console.WriteLine("запускаем скам сервер");
            _httpListener.Start();
            ServerStatus = ServerStatus.Start;
            Console.WriteLine("скам машина запущена");
            
            listeningTask = Listening();
        }
        
        public void Stop()
        {
            if (ServerStatus == ServerStatus.Stop)
            {
                Console.WriteLine("Сервер уже выключен");
                return;
            }

            Console.WriteLine("останавливаем скам сервер");
            ServerStatus = ServerStatus.Stop;
            lock(_locker)
                _httpListener.Stop();
            try
            {
                listeningTask.Wait();
                Console.WriteLine("Task waited");
            }
            catch
            {
                Console.WriteLine("listeningTask can't dispose");
            }
            Console.WriteLine("скам прекращен");
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        private async Task Listening()
        {
            while (ServerStatus == ServerStatus.Start)
            {
                try
                {
                    HttpListenerContext context = await _httpListener.GetContextAsync();
                    lock (_locker)
                    {
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;
                        byte[] buffer;
                        string format;
                        if (Directory.Exists(_serverSettings.Path))
                        {
                            (buffer, format) =
                                ServerFileHandling.GetFile(request.RawUrl.Replace("%20", " "), _serverSettings);
                            response.Headers.Set("Content-Type", DefineContentType(format));
                            if (buffer == null)
                            {
                                response.StatusCode = (int)HttpStatusCode.NotFound;
                                string err = "404 - not found";

                                buffer = Encoding.UTF8.GetBytes(err);
                            }
                        }
                        else
                        {
                            string err = $"Directory {_serverSettings.Path} not found";
                            buffer = Encoding.UTF8.GetBytes(err);
                        }

                        response.OutputStream.Write(buffer, 0, buffer.Length);
                        response.Close();
                    }
                }
                catch(HttpListenerException e)
                {
                    Console.WriteLine("HttpListenerException, которое непонятно как пофиксить без try-catch");
                }
            }
        }

        private string DefineContentType(string format)
        {
            return format switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".png" => "image/png",
                ".svg" => "image/svg+xml",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".ico" => "image/vnd.microsoft.icon",
                _ => "text/plain"
            };
        }
        
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
