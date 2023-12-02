namespace simpleApi.Models;

public class GlobalResponse
{
    public string Code { get; set; }
    public bool Status { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}