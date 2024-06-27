
namespace CraApp.Features.Activity;

public record UpdateActivityCommand(int Id, TimeSpan StartTime, TimeSpan EndTime, int Day, String Project, int MonthlyActivitiesId) : ICommand<UpdateActivityResult>;
public record UpdateActivityResult(int Id, TimeSpan StartTime, TimeSpan EndTime, int Day, String Project, int MonthlyActivitiesId);

public class UpdateActivity : ICarterModule 
{
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/activity", UpdateActivityHandler)
            .WithName("UpdateActivity");
    }

    private async Task<IResult> UpdateActivityHandler(ISender sender, [FromBody] ActivityDTO activityDTO )
    {
        var command = activityDTO.Adapt<UpdateActivityCommand>();
        APIResponse APIResponse = await CreateActivity.ActivityValidator(activityDTO);
        if (APIResponse.IsSuccess)
        {
            var result = await sender.Send(command);
            APIResponse.IsSuccess = true;
            APIResponse.Result = result;
            APIResponse.StatusCode = HttpStatusCode.NoContent;
            return Results.Created($"api/activity/", APIResponse);

        }
        return Results.BadRequest(APIResponse);
    }
}

internal class UpdateActivityHandler(IActivityRepository _repo) : ICommandHandler<UpdateActivityCommand, UpdateActivityResult>
{
    public async Task<UpdateActivityResult> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = await _repo.GetById(command.Id);
        
        if (activity == null) {
            throw new KeyNotFoundException();
        }
        activity.StartTime = command.StartTime;
        activity.EndTime = command.EndTime;
        activity.Day = command.Day;
        activity.Project = (Project)Enum.Parse(typeof(Project),command.Project);

        await _repo.SaveAsync();
        
        return new UpdateActivityResult(activity.Id, activity.StartTime, activity.EndTime, activity.Day, activity.Project.ToString(), activity.MonthlyActivitiesId);

    }
}