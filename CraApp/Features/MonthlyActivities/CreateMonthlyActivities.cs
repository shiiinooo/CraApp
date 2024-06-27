
using static CraApp.Features.Activity.CreateActivity;

namespace CraApp.Features.MonthlyActivities;

public record CreateMonthlyActivitiesCommand(int Year, int Month, ICollection<ActivityDTO> Activities) : ICommand<CreateMonthlyActivitiesResult>;
public record CreateMonthlyActivitiesResult(int id, int year, int month, ICollection<ActivityDTO> Activities);
public class CreateMonthlyActivities : ICarterModule
{
   
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/monthlyActivities", CreateMonthlyActivitiesHandler);
    }

    private async Task<IResult> CreateMonthlyActivitiesHandler(ISender sender,[FromBody] MonthlyActivitiesDTO monthlyActivities)
    {
        APIResponse APIResponse = new APIResponse();
        CreateMonthlyActivitiesCommand command = monthlyActivities.Adapt<CreateMonthlyActivitiesCommand>();
        foreach ( ActivityDTO activityDTO in monthlyActivities.Activities)
        {
           
            var temp = await ActivityValidator(activityDTO);
            int DaysInMonth = DateTime.DaysInMonth(monthlyActivities.Year, monthlyActivities.Month);
            if (DaysInMonth < activityDTO.Day || activityDTO.Day <= 0)
            {
                APIResponse.IsSuccess = false;
                APIResponse.StatusCode = HttpStatusCode.BadRequest;
                APIResponse.ErrorsMessages = new List<string> { "Day cannot exceded month max number of days or be less than zero. " };
                return Results.BadRequest(APIResponse);
            }
            if (!temp.IsSuccess)
            {
                APIResponse.IsSuccess = false;
                APIResponse.StatusCode = temp.StatusCode;
                APIResponse.ErrorsMessages = temp.ErrorsMessages;
                return Results.BadRequest(APIResponse);
            }
        }
        var result = await sender.Send(command);
        APIResponse.IsSuccess = true;
        APIResponse.Result = result;
        APIResponse.StatusCode = HttpStatusCode.NoContent;
        return Results.Ok(APIResponse);
    }
}

internal class CreateMonthlyActivitiesHandler : ICommandHandler<CreateMonthlyActivitiesCommand, CreateMonthlyActivitiesResult>
{
    private readonly IMonthlyActivitiesRepository _monthlyActivitiesRepository;
    private readonly IActivityRepository _activityRepository;


    public CreateMonthlyActivitiesHandler(IMonthlyActivitiesRepository monthlyActivitiesRepository, IActivityRepository activityRepository)
    {
        _monthlyActivitiesRepository = monthlyActivitiesRepository;
        _activityRepository = activityRepository;
    }

    public async Task<CreateMonthlyActivitiesResult> Handle(CreateMonthlyActivitiesCommand command, CancellationToken cancellationToken)
    {
        var montlyActivities = command.Adapt<Model.MonthlyActivities>();
        await _monthlyActivitiesRepository.CreateAsync(montlyActivities, cancellationToken);
        return new CreateMonthlyActivitiesResult(montlyActivities.Id, montlyActivities.Year, montlyActivities.Month, montlyActivities.Activities.Adapt<ICollection<ActivityDTO>>());
    }
}