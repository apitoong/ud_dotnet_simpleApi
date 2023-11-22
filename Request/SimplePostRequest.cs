using System.ComponentModel.DataAnnotations;

namespace simpleApi.Request;

public class SimplePostRequest
{
    [Required(ErrorMessage = "title tidak boleh kosong")]
    //
    public string title { get; set; }

    [Required(ErrorMessage = "body tidak boleh kosong")]
    //
    public string body { get; set; }

    [Required(ErrorMessage = "userId tidak boleh kosong")]
    //
    public int userId { get; set; }
}