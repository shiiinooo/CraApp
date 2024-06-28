namespace CraApp.Web;

public static class SD
{
    public enum ApiType
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public static string SessionToken = "JWTToken";
    public static int UserId;
    public static int MonthlyActivitiesId;
    public static int MonthlyActivitiesMonth;
}