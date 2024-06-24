namespace CraApp.Model;

public class APIResponse
{
    public APIResponse()
    {
        ErrorsMessages = new List<string>();
    }

    public bool IsSuccess { get; set; }
    public Object Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public List<string> ErrorsMessages { get; set; }
}

