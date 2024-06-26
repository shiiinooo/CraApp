namespace CraApp.Model.DTO;

public class ActivityDTO
{
    public int Id { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Day{ get; set; }
    public String Project { get; set; }
    public int MonthlyActivitiesId { get; set; } 
}
