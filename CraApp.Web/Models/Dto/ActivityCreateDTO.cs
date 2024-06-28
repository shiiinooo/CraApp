namespace CraApp.Web.Models.Dto;

public class ActivityCreateDTO
{
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public int Day { get; set; }
    public string Project { get; set; }
    public int MonthlyActivityId { get; set; }
}