namespace CraApp.Features.MonthlyActivities;

public record GetUserMonthlyActivitiesQuery(int UserId) : IQuery<IEnumerable<MonthlyActivityResult>>;

public record MonthlyActivityResult(int Id, int Year, int Month, IEnumerable<ActivityResult> Activities);

public record ActivityResult(int Id, TimeSpan StartTime, TimeSpan EndTime, int Day, String Project);


internal class GetUserMonthlyActivitiesQueryHandler : IQueryHandler<GetUserMonthlyActivitiesQuery, IEnumerable<MonthlyActivityResult>>
{
    private readonly IUserRepository _repository;

    public GetUserMonthlyActivitiesQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MonthlyActivityResult>> Handle(GetUserMonthlyActivitiesQuery query, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdWithActivitiesAsync(query.UserId, cancellationToken);

        if (user == null || user.MonthlyActivities == null || !user.MonthlyActivities.Any())
        {
            return Enumerable.Empty<MonthlyActivityResult>();
        }

        return user.MonthlyActivities.Select(ma => new MonthlyActivityResult(
            ma.Id,
            ma.Year,
            ma.Month,
            ma.Activities.Select(a => new ActivityResult(a.Id, a.StartTime, a.EndTime, a.Day, Enum.GetName(a.Project)))
        )).ToList();
    }
}


public class UserMonthlyActivitiesGetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{userId:int}/monthly-activities", async (int userId, ISender sender) =>
        {
            APIResponse response = new();
            var query = new GetUserMonthlyActivitiesQuery(userId);
            var monthlyActivities = await sender.Send(query);

            if (monthlyActivities == null || !monthlyActivities.Any())
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorsMessages = new List<string> { "No monthly activities found for this user." };
                return Results.NotFound(response);
            }

            response.Result = monthlyActivities;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        })
        //.RequireAuthorization("AdminOnly")
        .Produces<APIResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetUserMonthlyActivities")
        .WithSummary("Get User Monthly Activities")
        .WithDescription("Retrieve the monthly activities of a user.")
        .WithTags("MonthlyActivities");
    }
}
