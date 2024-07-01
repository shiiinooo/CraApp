namespace CraApp.Features.UserFeature;

// Command
public record UpdateUserCommand(int Id, string UserName, string Name, string Role) : ICommand<UpdateUserResult>;

// Result
public record UpdateUserResult(int Id, string UserName, string Name, string Role);

// Handler
internal class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IUserRepository _repository;

    public UpdateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        // Validate the input
        if (string.IsNullOrWhiteSpace(command.UserName) || string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("UserName and Name cannot be empty.");
        }

        // Check if the user exists
        var existingUser = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with ID {command.Id} not found.");
        }

        // Update the user details
        existingUser.UserName = command.UserName;
        existingUser.Name = command.Name;
        existingUser.Role = (Role)Enum.Parse(typeof(Role), command.Role);

        await _repository.UpdateAsync(existingUser, cancellationToken);
        await _repository.SaveAsync();

        return new UpdateUserResult(existingUser.Id, existingUser.UserName, existingUser.Name, existingUser.Role.ToString());
    }
}

// Endpoint
public class UsersPutEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Update User Endpoint
        app.MapPut("/users/{id:int}", async (ISender sender, int id, UpdateUserCommand command) =>
        {
            APIResponse response = new();
            try
            {
                // Ensure the ID in command matches the route parameter
                if (command.Id != id)
                {
                    response.IsSuccess = false;
                    response.ErrorsMessages = new List<string> { "Route parameter ID does not match command ID." };
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                var result = await sender.Send(command);

                response.Result = result;
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }
            catch (ArgumentException ex)
            {
                response.IsSuccess = false;
                response.ErrorsMessages = new List<string> { ex.Message };
                response.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(response);
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
        .RequireAuthorization("AdminOnly")
        .Produces<APIResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithName("UpdateUser")
        .WithSummary("Update User")
        .WithDescription("Update an existing user");
    }
}

