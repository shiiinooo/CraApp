namespace CraApp.Web.Models.Dto;

public class MonthlyActivityDTO
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public List<ActivityDTO> Activities { get; set; } = new List<ActivityDTO>();
}