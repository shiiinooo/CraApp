namespace CraApp.Features.Activity;

public class GetActivity : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/activity", GetActivityHandler)
            .WithName("GetActivity");
    }

    private async Task GetActivityHandler()
    {
        return ;
    }


}