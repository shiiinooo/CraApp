namespace CraApp.Features.Activity;

public class CreateActivity : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/activity", CreateActivityHandler)
            .WithName("CreateActivity");
    }

    private async Task CreateActivityHandler([FromBody] ActivityDTO activityDTO)
    {
        
    }
}
