namespace CraApp.Web.Models.Dto;

public class ActivityUpdateDTO
{
    public int Id { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Day { get; set; }
    public string Project { get; set; }
    public int MonthlyActivitiesId { get; set; }
}
