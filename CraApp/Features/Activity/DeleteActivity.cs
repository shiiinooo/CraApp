
using Azure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CraApp.Features.Activity;


public record DeleteActivityCommand(int Id) : ICommand<DeleteActivityResult>;
public record DeleteActivityResult(bool IsSuccess);

public class DeleteActivity : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/acitivity/{Id:int}", DeleteActivityHandler);
    }

    private async Task<IResult> DeleteActivityHandler(ISender sender, int id)
    {
        APIResponse APIResponse = new();
        try
        {
           await sender.Send(new DeleteActivityCommand(id));

            APIResponse.IsSuccess = true;
            APIResponse.StatusCode = HttpStatusCode.NoContent;
            return Results.Ok(APIResponse);
        }
        catch (KeyNotFoundException ex)
        {
            APIResponse.IsSuccess = false;
            APIResponse.ErrorsMessages = new List<string> { ex.Message };
            APIResponse.StatusCode = HttpStatusCode.NotFound;
            return Results.NotFound(APIResponse);
        }
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
