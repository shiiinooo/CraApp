
using static CraApp.Features.Activity.CreateActivity;

namespace CraApp.Features.MonthlyActivities;

public record CreateMonthlyActivitiesCommand(int Id, int Year, int Month, ICollection<ActivityDTO> Activities,int UserId) : ICommand<CreateMonthlyActivitiesResult>;
public record CreateMonthlyActivitiesResult(int id, int year, int month, ICollection<ActivityDTO> Activities, int UserId);
public class CreateMonthlyActivities : ICarterModule
{
   
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/monthlyActivities", CreateMonthlyActivitiesHandler)
              .WithName("CreateMonthlyActivities")
           .Produces<APIResponse>(StatusCodes.Status201Created)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .WithSummary("Creating Monthly Activities")
           .WithTags("MonthlyActivities");
    }

    private async Task<IResult> CreateMonthlyActivitiesHandler(ISender sender, [FromBody] MonthlyActivitiesDTO monthlyActivities)
    {
        APIResponse APIResponse = new APIResponse();
        CreateMonthlyActivitiesCommand command = monthlyActivities.Adapt<CreateMonthlyActivitiesCommand>();

        try
        {
            foreach (var activityDTO in monthlyActivities.Activities)
            {
                var temp = await ActivityValidator(activityDTO);
                int daysInMonth = DateTime.DaysInMonth(monthlyActivities.Year, monthlyActivities.Month);
                if (daysInMonth < activityDTO.Day || activityDTO.Day <= 0)
                {
                    APIResponse.IsSuccess = false;
                    APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    APIResponse.ErrorsMessages = new List<string> { "Day cannot exceed the month's max number of days or be less than zero." };
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
            APIResponse.StatusCode = HttpStatusCode.Created;
            return Results.Created($"/api/monthlyActivities/{result.id}", APIResponse);
        }
        catch (InvalidOperationException ex)
        {
            APIResponse.IsSuccess = false;
            APIResponse.StatusCode = HttpStatusCode.BadRequest;
            APIResponse.ErrorsMessages = new List<string> { ex.Message };
            return Results.BadRequest(APIResponse);
        }
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
        bool alreadyExist = await _monthlyActivitiesRepository.IsMonthlyActivitiesExisit(command.UserId, command.Year, command.Month);
        /*if (alreadyExist)
        {
            throw new InvalidOperationException("Monthly activities for this user and month already exist.");
        }*/
        await _monthlyActivitiesRepository.CreateAsync(montlyActivities, cancellationToken);
        return new CreateMonthlyActivitiesResult(montlyActivities.Id, montlyActivities.Year, montlyActivities.Month, montlyActivities.Activities.Adapt<ICollection<ActivityDTO>>(), montlyActivities.UserId);
    }
}