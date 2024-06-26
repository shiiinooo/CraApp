
namespace CraApp.Web.Models.Dto;

public class UserUpdateDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}
