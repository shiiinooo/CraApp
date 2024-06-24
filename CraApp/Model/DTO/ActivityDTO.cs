namespace CraApp.Model.DTO;

public class ActivityDTO
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Project Project { get; set; }
}
