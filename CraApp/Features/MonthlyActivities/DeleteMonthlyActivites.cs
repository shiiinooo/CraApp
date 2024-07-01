

using CraApp.Features.Activity;

namespace CraApp.Features.MonthlyActivities;
public record DeleteMonthlyActivitiesCommand(int Id) : ICommand<DeleteMonthlyActivitiesResult>;
public record DeleteMonthlyActivitiesResult(bool IsSuccess);
public class DeleteMonthlyActivites : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/monthlyActivities/{Id:int}", DeleteMonthlyActivitiesHandler)
            .WithName("DeleteMonthlyActivities")
            .Produces<APIResponse>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Deleting Monthly Activities")
            .WithTags("MonthlyActivities");
    }

    private async Task<IResult> DeleteMonthlyActivitiesHandler(ISender sender, int Id)
    {
        var response = new APIResponse();
        var result = await sender.Send(new DeleteMonthlyActivitiesCommand(Id));

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        response.ErrorsMessages.Add("Key Not Found");
        return Results.NotFound(response);
    }

}
internal class DeleteMonthlyActivitiesHandler(IMonthlyActivitiesRepository _monthlyActivitiesRepository) : ICommandHandler<DeleteMonthlyActivitiesCommand, DeleteMonthlyActivitiesResult>
{
    public async Task<DeleteMonthlyActivitiesResult> Handle(DeleteMonthlyActivitiesCommand command, CancellationToken cancellationToken)
    {
        var activity = await _monthlyActivitiesRepository.GetByIdAsync(command.Id);

        if (activity == null)
        {
            return new DeleteMonthlyActivitiesResult(false);
        }

        await _monthlyActivitiesRepository.DeleteAsync(activity);
        return new DeleteMonthlyActivitiesResult(true);
    }
}
