namespace CraApp.Features.Activity;

public record GetActivityResult(TimeSpan StartTime, TimeSpan EndTime, String Project);
public record GetAcitivityQuery() : IQuery<IEnumerable<GetActivityResult>>;


//public record GetActivityRequest();
//public record GetActivityResponse(IEnumerable<ActivityDTO> Activities);

public class GetActivity : ICarterModule
{


    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/activity", GetActivityHandler)
            .WithName("GetActivity")
            .Produces<APIResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Retrieving All Activities")
            .WithTags("Activity");
    }

    private async Task<IResult> GetActivityHandler(ISender sender)
    {
        APIResponse aPIResponse = new();
        var query = new GetAcitivityQuery();
        var result = await sender.Send(query);

        aPIResponse.Result = result;

        return Results.Ok(aPIResponse);
    }


}
internal class GetActivitiesHandler : IQueryHandler<GetAcitivityQuery, IEnumerable<GetActivityResult>>
{
    private readonly IActivityRepository _activityRepository;
    public GetActivitiesHandler(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<IEnumerable<GetActivityResult>> Handle(GetAcitivityQuery request, CancellationToken cancellationToken)
    {
        var activities = await _activityRepository.GetAllAsync(cancellationToken);
        var activityResult = activities.Select(u => new GetActivityResult(u.StartTime, u.EndTime, Enum.GetName(u.Project))).ToList();
        return activityResult;
    }
}