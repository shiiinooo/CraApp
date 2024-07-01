using System.Text.Json.Serialization;

namespace CraApp.Model;

public class APIResponse
{
    public APIResponse()
    {
        ErrorsMessages = new List<string>();
    }

   
    public Object Result { get; set; }

    public List<string> ErrorsMessages { get; set; }
}
