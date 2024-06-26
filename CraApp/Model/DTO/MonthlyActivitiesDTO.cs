namespace CraApp.Model.DTO;

public class MonthlyActivitiesDTO
{
    public int Id { get; set; } 
    public int Year { get; set; }
    public int Month { get; set; }

    public ICollection<ActivityDTO> Activities { get; set; }
}
