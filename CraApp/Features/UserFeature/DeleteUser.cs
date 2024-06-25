namespace CraApp.Features.UserFeature;

// Command
public record DeleteUserCommand(int Id) : ICommand;

// Handler
internal class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _repository;

    public DeleteUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        // Check if the user exists
        var existingUser = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (existingUser == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        // Delete the user
        _repository.DeleteAsync(existingUser);
        

        return Unit.Value;
    }
}

public class UsersDeleteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Delete User Endpoint
        app.MapDelete("/users/{id:int}", async (ISender sender, int id) =>
        {
            APIResponse response = new();
            try
            {
                await sender.Send(new DeleteUserCommand(id));

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.NoContent;
                return Results.NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                response.IsSuccess = false;
                response.ErrorsMessages = new List<string> { ex.Message };
                response.StatusCode = HttpStatusCode.NotFound;
                return Results.NotFound(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorsMessages = new List<string> { ex.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
                return Results.Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
        .Produces<APIResponse>(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithName("DeleteUser")
        .WithSummary("Delete User")
        .WithDescription("Delete a user by ID");
    }
}
