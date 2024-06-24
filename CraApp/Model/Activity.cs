namespace CraApp.Model;

public class Activity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } 
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Project Project { get; set; }    

    /*public int UserId { get; set; }
    public User User { get; set; }  */

}
