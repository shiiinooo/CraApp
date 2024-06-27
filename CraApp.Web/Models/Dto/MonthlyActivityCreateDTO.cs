namespace CraApp.Web.Models.Dto
{
    public class MonthlyActivityCreateDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<ActivityDTO> Activities { get; set; } = new List<ActivityDTO>();
        public int UserId { get; set; }
    }

    
}
