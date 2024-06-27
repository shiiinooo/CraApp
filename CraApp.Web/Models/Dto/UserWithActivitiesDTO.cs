namespace CraApp.Web.Models.Dto;

public class UserWithActivitiesDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public List<MonthlyActivityDTO> MonthlyActivities { get; set; } = new List<MonthlyActivityDTO>();
}
