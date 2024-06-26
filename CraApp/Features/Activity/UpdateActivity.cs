﻿
namespace CraApp.Features.Activity;

public record UpdateActivityCommand(int Id, TimeSpan StartTime, TimeSpan EndTime, int Day, String Project, int MonthlyActivitiesId) : ICommand<UpdateActivityResult>;
public record UpdateActivityResult(int Id, TimeSpan StartTime, TimeSpan EndTime, int Day, String Project, int MonthlyActivitiesId);

public class UpdateActivity : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/activity", UpdateActivityHandler)
            .WithName("UpdateActivity")
            .Produces<APIResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Activity ")
            .WithTags("Activity")
            ;
    }

    private async Task<IResult> UpdateActivityHandler(ISender sender, [FromBody] ActivityDTO activityDTO)
    {
        var command = activityDTO.Adapt<UpdateActivityCommand>();
        var validationResponse = await CreateActivity.ActivityValidator(activityDTO);

        if (validationResponse.ErrorsMessages.Count == 0)
        {
            var result = await sender.Send(command);
            var response = new APIResponse
            {
                Result = result
            };
            return Results.Ok(response);
        }

        return Results.BadRequest(validationResponse);
    }

}
    internal class UpdateActivityHandler(IActivityRepository _repo) : ICommandHandler<UpdateActivityCommand, UpdateActivityResult>
    {
        public async Task<UpdateActivityResult> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
        {
            var activity = await _repo.GetById(command.Id);

            if (activity == null)
            {
                throw new KeyNotFoundException();
            }
            activity.StartTime = command.StartTime;
            activity.EndTime = command.EndTime;
            activity.Day = command.Day;
            activity.Project = (Project)Enum.Parse(typeof(Project), command.Project);

            await _repo.SaveAsync();

            return new UpdateActivityResult(activity.Id, activity.StartTime, activity.EndTime, activity.Day, activity.Project.ToString(), activity.MonthlyActivitiesId);

        }
    }
