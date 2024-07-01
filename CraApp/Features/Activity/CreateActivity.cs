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
        var validationResponse = await ActivityValidator(activityDTO);

        if (validationResponse.ErrorsMessages.Count == 0)
        {
            var result = await sender.Send(command);
            var response = new APIResponse
            {
                Result = result
            };
            return Results.Created($"api/activity/", response);
        }

        return Results.BadRequest(validationResponse);

    }

    private static readonly int maxHours = 10;
    public static async Task<APIResponse> ActivityValidator(ActivityDTO activityDTO)
    {
        var response = new APIResponse();

        if (activityDTO.StartTime > activityDTO.EndTime)
        {
            response.ErrorsMessages.Add("End Time must be greater than Start Time");
        }
        if ((activityDTO.EndTime - activityDTO.StartTime).TotalHours > maxHours)
        {
            response.ErrorsMessages.Add("Illegal hours of work");
        }

        return await Task.FromResult(response);
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
