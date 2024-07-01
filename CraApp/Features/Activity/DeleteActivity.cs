
using Azure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CraApp.Features.Activity;


public record DeleteActivityCommand(int Id) : ICommand<DeleteActivityResult>;
public record DeleteActivityResult(bool IsSuccess);

public class DeleteActivity : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/activity/{Id:int}", DeleteActivityHandler)
            .WithName("DeleteActivity")
            .Produces<APIResponse>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Deleting Activity")
            .WithTags("Activity"); 
    }

    private async Task<IResult> DeleteActivityHandler(ISender sender, int id)
    {
        APIResponse APIResponse = new();
        var result = await sender.Send(new DeleteActivityCommand(id));
        if (result.IsSuccess)
        {
            return Results.Ok(APIResponse);
        }
        APIResponse.ErrorsMessages = new List<string> { "Key Not Found "};
        return Results.NotFound(APIResponse);

    }
}
internal class DeleteActivityHandler(IActivityRepository _activityRepository) : ICommandHandler<DeleteActivityCommand, DeleteActivityResult>
{
    public async Task<DeleteActivityResult> Handle(DeleteActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = await _activityRepository.GetById(command.Id);

        if (activity == null)
        {
            return new DeleteActivityResult(false);
        }

        await _activityRepository.DeleteAsync(activity);
        return new DeleteActivityResult(true);
    }
}
