namespace CraApp.Features.UserFeature;

// Query
public record GetUserByIdQuery(int Id) : IQuery<GetUserByIdResult>;

// Result
public record GetUserByIdResult(int Id, string UserName, string Name, string Role);

// Handler
internal class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, GetUserByIdResult>
{
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetUserByIdResult> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(query.Id, cancellationToken);

        if (user == null)
        {
            return null;
        }

        return new GetUserByIdResult(user.Id, user.UserName, user.Name, user.Role.ToString());
    }
}

// Endpoint
public class UserGetByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/user/{id:int}", async (int id, ISender sender) =>
        {
            APIResponse response = new();
            var query = new GetUserByIdQuery(id);
            var user = await sender.Send(query);

            if (user == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorsMessages = new List<string> { "User not found" };
                return Results.NotFound(response);
            }

            response.Result = user;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        })
        .RequireAuthorization("AdminOnly")
        .Produces<APIResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetUserById")
        .WithSummary("Get User by ID")
        .WithDescription("Retrieve a user by their ID");
    }
}
