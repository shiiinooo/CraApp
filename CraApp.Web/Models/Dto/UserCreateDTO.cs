namespace CraApp.Web.Models.Dto;

public class UserCreateDTO
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}
