namespace YaSkamerBroServer;

public class HttpDELETE : HttpMethodAttribute
{
    public HttpDELETE(string methodUri) : base(HttpMethod.Delete, methodUri)
    {
    }
    
    public HttpDELETE() : this("") {}
}