using CraApp.Features.Activity;
using CraApp.Model;
using CraApp.Model.DTO;

namespace CraApp.Features.MonthlyActivities;
public record UpdateMonthlyActivitiesCommand(int Id, int Year, int Month, ICollection<ActivityDTO> Activities) : ICommand<UpdateMonthlyActivitiesResult>;
public record UpdateMonthlyActivitiesResult(int Id, int year, int month);
public class UpdateMonthlyActivities : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/activities", UpdateMonthlyActivitiesHandler);
    }

    private async Task<IResult> UpdateMonthlyActivitiesHandler(ISender sender, MonthlyActivitiesDTO monthlyActivitiesDTO)
    {
        APIResponse APIResponse = new APIResponse();
        APIResponse.IsSuccess= true;
        var command = monthlyActivitiesDTO.Adapt<UpdateMonthlyActivitiesCommand>();
        foreach(ActivityDTO activityDTO in monthlyActivitiesDTO.Activities)
        {
            var temp = await CreateActivity.ActivityValidator(activityDTO);
            int DaysInMonth = DateTime.DaysInMonth(monthlyActivitiesDTO.Year, monthlyActivitiesDTO.Month);
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

internal class UpdateMonthlyActivitiesHandler(IMonthlyActivitiesRepository _monthlyActivitiesRepository) : 
    ICommandHandler<UpdateMonthlyActivitiesCommand, UpdateMonthlyActivitiesResult>
{
    public async Task<UpdateMonthlyActivitiesResult> Handle(UpdateMonthlyActivitiesCommand command, CancellationToken cancellationToken)
    {
       var monthlyActivities = await _monthlyActivitiesRepository.GetByIdAsync(command.Id);
        if (monthlyActivities == null)
        {
            throw new KeyNotFoundException();
        }
        monthlyActivities.Year = command.Year;
        monthlyActivities.Month = command.Month;
        monthlyActivities.Activities = command.Activities.Adapt<ICollection<Model.Activity>>();

        await _monthlyActivitiesRepository.SaveAsync();

        return new UpdateMonthlyActivitiesResult(monthlyActivities.Id, monthlyActivities.Year, monthlyActivities.Month);
    }
}
