using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ValidationException = FluentValidation.ValidationException;

namespace CraApp.Features.UserFeature;

// Command
public record CreateUserCommand(string UserName, string Name, string Password, string Role) : ICommand<CreateUserResult>;

// Result
public record CreateUserResult(int Id, string UserName, string Name, string Role);

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().NotNull().WithMessage("UserName is required");
        RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Name is required");
        RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("Password is required");
        RuleFor(x => x.Role).NotEmpty().NotNull().WithMessage("Role is required");
    }
}

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
        await CheckForDuplicateUser(command, cancellationToken);

        var newUser = new User
        {
            UserName = command.UserName,
            Name = command.Name,
            Password = command.Password,
            Role = (Role)Enum.Parse(typeof(Role), command.Role),
            MonthlyActivities = new List<Model.MonthlyActivities>()
        };

        await _repository.CreateAsync(newUser, cancellationToken);
        //await _repository.SaveAsync();

        return new CreateUserResult(newUser.Id, newUser.UserName, newUser.Name, newUser.Role.ToString());
    }

    private async Task CheckForDuplicateUser(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await _repository.FindByUserNameAsync(command.UserName, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with this username already exists.");
        }
    }
}
// Endpoint
public class UsersPostEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users", CreateUser)
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

    private async Task<IResult> CreateUser(ISender sender, [FromBody] CreateUserCommand command)
    {
        APIResponse response = new();
        try
        {
            var result = await sender.Send(command);
            response.Result = result;
            return Results.Created($"/users", response);
        }
        catch (ValidationException ex)
        {
            response.ErrorsMessages = ex.Errors.Select(e => e.ErrorMessage).ToList();
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
    }






}


