using System;
using System.Net;

namespace GoogleHttpServer;

public class HttpServer
{
    private readonly string url;
    private readonly HttpListener listener;

    private bool serverOn = true;
    private HttpListenerContext context;
    private HttpListenerRequest request;
    private HttpListenerResponse response;

    public HttpServer(string url)
    {
        this.url = url;
        listener = new HttpListener();

        listener.Prefixes.Add(url);
    }

    public void Start()
    {
        if (listener.IsListening) return;
        listener.Start();
        
        Console.WriteLine("Waiting for connection...");

        //Receive();
        Handling();
    }

    public void Stop()
    { 
        if (!listener.IsListening) return;
        listener.Stop();
        Console.WriteLine("Connection processing completed");
    }
    
    public void Restart()
    {
        Stop();
        Start();
    }

    /*private void Receive()
    {
        listener.BeginGetContext(new AsyncCallback(Listen), listener);
    }
    
    private void Listen(IAsyncResult result)
    {
        if (listener.IsListening)
        {
            try
            {
                if (!result.IsCompleted)
                    result.AsyncWaitHandle.Dispose();
                context = listener.EndGetContext(result);
                request = context.Request;
                response = context.Response;
                RequestWork();
                response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} - server shut down");
                Restart();
            }
            Receive();
        }
    }*/

    private void Handling()
    {
        while (serverOn)
        {
            Listen();
            Console.WriteLine("Type command (start / stop / restart / exit):");
            string command = ReadValue();
            switch (command)
            {
                case "start":
                    Start();
                    break;
                case "stop":
                    Stop();
                    break;
                case "restart":
                    Restart();
                    break;
                case "exit":
                    Stop();
                    serverOn = false;
                    break;
                default:
                    Console.WriteLine("Wrong type");
                    break;
            }
        }
    }

    private async Task Listen()
    {
        while(listener.IsListening)
        {
            context = await listener.GetContextAsync();
            request = context.Request;
            response = context.Response;
            RequestWork();
            response.Close();
        }
    }

    private static string? ReadValue() => Console.ReadLine();
    
    private void RequestWork()
    {
        //string responseStr = File.ReadAllText( Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\google\index.html");
        string responseStr = TryOutputFile();
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
        
        response.ContentLength64 = buffer.Length;
        Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
    }

    private string TryOutputFile()
    {
        string responseStr = "<html><head><meta charset='utf8'></head><body>Can't find html page. So u see default screen</body></html>";
        string? path =
            (request.Url?.AbsoluteUri.Split('/').Select(s => s.ToString()))?.FirstOrDefault(s =>
                s != "" && s == "google");
        if (path != null)
        {
            Console.WriteLine(string.Join(" ", path));
            string filter = "*.html";
            string[] files =
                Directory.GetFiles(
                    $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}\{path}\", filter);
            if (files.Length > 0)
                responseStr = File.ReadAllText(files[0]);
        }

        return responseStr;
    }
}