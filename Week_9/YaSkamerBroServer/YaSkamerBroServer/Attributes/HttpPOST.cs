namespace YaSkamerBroServer;

public class HttpPOST: HttpMethodAttribute
{
    public HttpPOST(string methodUri) : base(HttpMethod.Post, methodUri)
    {
    }
    
    public HttpPOST() : this("") {}
}