namespace CraApp.Model
{
    public class MonthlyActivities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Year { get; set; }
        public int Month {  get; set; }

        public ICollection<Activity> Activities { get; set; }
    }
}
