namespace CraApp.Features.UserFeature;

// Command
public record CreateUserCommand(string UserName, string Name) : ICommand<CreateUserResult>;

// Result
public record CreateUserResult(int Id, string UserName, string Name);

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
        if (string.IsNullOrWhiteSpace(command.UserName) || string.IsNullOrWhiteSpace(command.Name))
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
            Name = command.Name
        };

        await _repository.CreateAsync(newUser, cancellationToken);
        await _repository.SaveAsync();

        return new CreateUserResult(newUser.Id, newUser.UserName, newUser.Name);
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
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                return Results.Created($"/users/{result.Id}", response);
            }
            catch (ArgumentException ex)
            {
                response.IsSuccess = false;
                response.ErrorsMessages = new List<string> { ex.Message };
                response.StatusCode = HttpStatusCode.BadRequest;
                return Results.BadRequest(response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.ErrorsMessages = new List<string> { ex.Message };
                response.StatusCode = HttpStatusCode.Conflict;
                return Results.Conflict(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorsMessages = new List<string> { ex.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
                return Results.Problem(detail: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
        .Produces<APIResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithName("CreateUser")
        .WithSummary("Create User")
        .WithDescription("Create a new user");
    }
}

