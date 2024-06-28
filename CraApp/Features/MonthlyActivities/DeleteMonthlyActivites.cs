

using CraApp.Features.Activity;

namespace CraApp.Features.MonthlyActivities;
public record DeleteMonthlyActivitiesCommand(int Id) : ICommand<DeleteMonthlyActivitiesResult>;
public record DeleteMonthlyActivitiesResult(bool IsSuccess);
public class DeleteMonthlyActivites : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/monthlyActivities/{Id:int}", DeleteMonthlyActivitesHandler)
            .WithName("DeleteMonthlyActivities")
            .Produces<APIResponse>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Deleting Monthly Activities")
            .WithTags("MonthlyActivities");
    }

    private async Task<IResult> DeleteMonthlyActivitesHandler(ISender sender, int Id)
    {
        APIResponse APIResponse = new();
        var result = await sender.Send(new DeleteMonthlyActivitiesCommand(Id));
        if (result.IsSuccess)
        {
            APIResponse.IsSuccess = true;
            APIResponse.StatusCode = HttpStatusCode.NoContent;
            return Results.Ok(APIResponse);
        }
        APIResponse.IsSuccess = false;
        APIResponse.ErrorsMessages = new List<string> { "Key Not Found " };
        APIResponse.StatusCode = HttpStatusCode.NotFound;
        return Results.NotFound(APIResponse);

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
