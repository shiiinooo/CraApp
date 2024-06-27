
using CraApp.Features.MonthlyActivities;
using CraApp.Model;

namespace CraApp.Features.Activity;


public record GetActivitiesByMonthAndUserQuery(int UserId, int Month) : IQuery<IEnumerable<MonthlyActivityResult>>;

public record MonthlyActivityResult(int Id, int Year, int Month, IEnumerable<ActivityResult> Activities);

public record ActivityResult(int Id, TimeSpan StartTime, TimeSpan EndTime, int Day, string Project);

internal class GetActivitiesByMonthAndUserQueryHandler : IRequestHandler<GetActivitiesByMonthAndUserQuery, IEnumerable<MonthlyActivityResult>>
{
    private readonly IUserRepository _repository;

    public GetActivitiesByMonthAndUserQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MonthlyActivityResult>> Handle(GetActivitiesByMonthAndUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdWithActivitiesAsync(query.UserId, cancellationToken);

        if (user == null || user.MonthlyActivities == null || !user.MonthlyActivities.Any())
        {
            return Enumerable.Empty<MonthlyActivityResult>();
        }

        var monthlyActivities = user.MonthlyActivities
            .Where(ma => ma.Month == query.Month)
            .Select(ma => new MonthlyActivityResult(
                ma.Id,
                ma.Year,
                ma.Month,
                ma.Activities.Select(a => new ActivityResult(
                    a.Id,
                    a.StartTime,
                    a.EndTime,
                    a.Day,
                    Enum.GetName(a.Project)
                ))
            )).ToList();

        return monthlyActivities;
    }
}

public class ActivitiesByMonthAndUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{userId:int}/activities/{month:int}", async (int userId, int month, ISender sender) =>
        {
            var query = new GetActivitiesByMonthAndUserQuery(userId, month);
            var response = await sender.Send(query);

            if (response == null || !response.Any())
            {
                var notFoundResponse = new APIResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorsMessages = new List<string> { "No activities found for this user and month." }
                };
                return Results.NotFound(notFoundResponse);
            }

            var successResponse = new APIResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = response
            };
            return Results.Ok(successResponse);
        })
        .Produces<APIResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetActivitiesByMonthAndUser")
        .WithSummary("Get Activities By Month And User")
        .WithDescription("Retrieve activities for a specific month of a user.");
    }
}