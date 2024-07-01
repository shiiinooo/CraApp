using CraApp.Features.Activity;

namespace CraApp.Features.MonthlyActivities;


public record GetMonthlyActivitiesQuery : IQuery<IEnumerable<GetMonthlyActivitiesResult>>;
public record GetMonthlyActivitiesResult(int Year, int Month, ICollection<ActivityDTO> Activities, int UserId);

public class GetMonthlyActivities : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/monthlyActivities", GetMonthlyActivitiesHandler)
             .WithName("GetMonthlyActivities")
            .Produces<APIResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Retrieving Monthly Activites")
            .WithTags("MonthlyActivities");
    }

    private async Task<IResult> GetMonthlyActivitiesHandler(ISender sender)
    {
        APIResponse aPIResponse = new();
        var query = new GetMonthlyActivitiesQuery();
        var result = await sender.Send(query);
        aPIResponse.Result = result;

        return Results.Ok(aPIResponse);
    }
}

internal class GetMonthlyActivitiesHandler : IQueryHandler<GetMonthlyActivitiesQuery, IEnumerable<GetMonthlyActivitiesResult>>
{
    private readonly IMonthlyActivitiesRepository _monthlyActivitiesRepository;
    public GetMonthlyActivitiesHandler(IMonthlyActivitiesRepository monthlyActivitiesRepository)
    {
        _monthlyActivitiesRepository = monthlyActivitiesRepository;
    }

    public async Task<IEnumerable<GetMonthlyActivitiesResult>> Handle(GetMonthlyActivitiesQuery request, CancellationToken cancellationToken)
    {
        var monthlyActivities = await _monthlyActivitiesRepository.GetAllAsyncIncludeActivities(cancellationToken);
        var result = monthlyActivities.Select(u => new GetMonthlyActivitiesResult(u.Year, u.Month, u.Activities.Adapt<ICollection<ActivityDTO>>(), u.UserId));
        return result;
    }
}