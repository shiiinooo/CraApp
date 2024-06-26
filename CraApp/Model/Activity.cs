namespace CraApp.Model;

public class Activity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
    [Required]
    public int Day { get; set; }
    [Required]
    public Project Project { get; set; }

    [ForeignKey("MonthlyActivitiesId")]
    public int MonthlyActivitiesId {  get; set; }
    public MonthlyActivities MonthlyActivities { get; set; }

}
