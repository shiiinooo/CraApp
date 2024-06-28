namespace CraApp.Features.Activity;


public record CreateActivityCommand(TimeSpan StartTime, TimeSpan EndTime, int Day, String Project, int MonthlyActivitiesId) : ICommand<CreateActivityResult>;
public record CreateActivityResult(int Id, TimeSpan StartTime, TimeSpan EndTime, int Day, String Project);

public class CreateActivity() : ICarterModule
{

    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/activity", CreateActivityHandler)
           .WithName("CreateActivity")
           .Produces<APIResponse>(StatusCodes.Status201Created)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .WithSummary("Creating Activity")
           .WithTags("Activity");
    }

    
    public async Task<IResult> CreateActivityHandler(ISender sender, [FromBody] ActivityDTO activityDTO)
    {

        var command = activityDTO.Adapt<CreateActivityCommand>();

        APIResponse APIResponse = await ActivityValidator(activityDTO);
        if (APIResponse.IsSuccess)
        {
            var result = await sender.Send(command);
            APIResponse.IsSuccess = true;
            APIResponse.Result = result;
            APIResponse.StatusCode = HttpStatusCode.Created;
            return Results.Created($"api/activity/", APIResponse);

        }

        return Results.BadRequest(APIResponse);

    }
    private static readonly int maxHours = 10;
    public static async Task<APIResponse> ActivityValidator(ActivityDTO activityDTO)
    {
        APIResponse APIResponse = new APIResponse();
        APIResponse.IsSuccess = true;

        if (activityDTO.StartTime > activityDTO.EndTime)
        {
            APIResponse.IsSuccess = false;
            APIResponse.StatusCode = HttpStatusCode.BadRequest;
            APIResponse.ErrorsMessages = new List<string> { "End Time must be greater than Start Time" };
            return APIResponse;
        }
        if ((activityDTO.EndTime - activityDTO.StartTime).TotalHours > maxHours)
        {
            APIResponse.IsSuccess = false;
            APIResponse.StatusCode = HttpStatusCode.BadRequest;
            APIResponse.ErrorsMessages = new List<string> { "Illegal hours of work " };
            return APIResponse;
        }
        return APIResponse;
    }
}

internal class CreateActivityHandler : ICommandHandler<CreateActivityCommand, CreateActivityResult>
{
    private readonly IActivityRepository _activityRepository;
   
    public CreateActivityHandler(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<CreateActivityResult> Handle(CreateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = command.Adapt<Model.Activity>();
        await _activityRepository.CreateAsync(activity, cancellationToken);

        return new CreateActivityResult(activity.Id, activity.StartTime, activity.EndTime, activity.Day, activity.Project.ToString());
    }

}
