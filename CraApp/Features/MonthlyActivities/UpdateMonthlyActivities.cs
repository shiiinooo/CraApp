using CraApp.Features.Activity;
using CraApp.Model;
using CraApp.Model.DTO;

namespace CraApp.Features.MonthlyActivities;
public record UpdateMonthlyActivitiesCommand(int Id, int Year, int Month, ICollection<ActivityDTO> Activities, int UserId) : ICommand<UpdateMonthlyActivitiesResult>;
public record UpdateMonthlyActivitiesResult(int Id, int year, int month, ICollection<ActivityDTO> Activities, int UserId);
public class UpdateMonthlyActivities : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/activities", UpdateMonthlyActivitiesHandler)
             .WithName("UpdateMonthlyActivities")
            .Produces<APIResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Monthly Activities")
            .WithTags("MonthlyActivities");
    }

    private async Task<IResult> UpdateMonthlyActivitiesHandler(ISender sender, MonthlyActivitiesDTO monthlyActivitiesDTO)
    {
        var response = new APIResponse();
        var command = monthlyActivitiesDTO.Adapt<UpdateMonthlyActivitiesCommand>();

        foreach (ActivityDTO activityDTO in monthlyActivitiesDTO.Activities)
        {
            var validationResponse = await CreateActivity.ActivityValidator(activityDTO);
            int daysInMonth = DateTime.DaysInMonth(monthlyActivitiesDTO.Year, monthlyActivitiesDTO.Month);

            if (daysInMonth < activityDTO.Day || activityDTO.Day <= 0)
            {
                response.ErrorsMessages.Add("Day cannot exceed the month's max number of days or be less than zero.");
                return Results.BadRequest(response);
            }

            if (validationResponse.ErrorsMessages.Count > 0)
            {
                response.ErrorsMessages.AddRange(validationResponse.ErrorsMessages);
                return Results.BadRequest(response);
            }
        }

        var result = await sender.Send(command);
        response.Result = result;

        return Results.Ok(response);
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

        return new UpdateMonthlyActivitiesResult(monthlyActivities.Id, monthlyActivities.Year, monthlyActivities.Month, monthlyActivities.Activities.Adapt<ICollection<ActivityDTO>>(), monthlyActivities.UserId);
    }
}
