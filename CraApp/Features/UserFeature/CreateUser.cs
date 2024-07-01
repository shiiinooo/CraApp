namespace CraApp.Features.UserFeature;

// Command
public record CreateUserCommand(string UserName, string Name, string Password, string Role) : ICommand<CreateUserResult>;

// Result
public record CreateUserResult(int Id, string UserName, string Name, string Role);

// Handler
internal class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IUserRepository _repository;

    public CreateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Validate the input
        if (string.IsNullOrWhiteSpace(command.UserName) || string.IsNullOrWhiteSpace(command.Name) 
         || string.IsNullOrWhiteSpace(command.Password) || string.IsNullOrWhiteSpace(command.Role))
        {
            throw new ArgumentException("UserName and Name cannot be empty.");
        }

        // Check for duplicate user
        var existingUser = await _repository.FindByUserNameAsync(command.UserName, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with this username already exists.");
        }

        var newUser = new User
        {
            UserName = command.UserName,
            Name = command.Name,
            Password = command.Password,
            Role = command.Role,
            MonthlyActivities = new List<Model.MonthlyActivities>()
        };

        await _repository.CreateAsync(newUser, cancellationToken);
        await _repository.SaveAsync();

        return new CreateUserResult(newUser.Id, newUser.UserName, newUser.Name, newUser.Role);
    }
}
// Endpoint
public class UsersPostEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Create User Endpoint
        app.MapPost("/users", async (ISender sender, CreateUserCommand command) =>
        {
            APIResponse response = new();
            try
            {
                var result = await sender.Send(command);

                response.Result = result;
                return Results.Created($"/users/{result.Id}", response);
            }
            catch (ArgumentException ex)
            {
                response.ErrorsMessages = new List<string> { ex.Message };
                return Results.BadRequest(response);
            }
            catch (InvalidOperationException ex)
            {
                response.ErrorsMessages = new List<string> { ex.Message };
                return Results.Conflict(response);
            }
            catch (Exception ex)
            {
                response.ErrorsMessages = new List<string> { ex.Message };
                return Results.Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
        .RequireAuthorization("AdminOnly") // Apply the AdminOnly policy
        .Produces<APIResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithName("CreateUser")
        .WithSummary("Create User")
        .WithDescription("Create a new user");
    }
}


