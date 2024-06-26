namespace CraApp.Features.UserFeature;

// Query
public record GetUsersQuery() : IQuery<IEnumerable<GetUserResult>>;

//Result
public record GetUserResult(int Id, string UserName, string Name, string Role);

// Handler
internal class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IEnumerable<GetUserResult>>
{
    private readonly IUserRepository _repository;

    public GetUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GetUserResult>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync(cancellationToken);

        if (users == null || !users.Any())
        {
            return Enumerable.Empty<GetUserResult>();
        }

        return users.Select(u => new GetUserResult(u.Id, u.UserName, u.Name, u.Role)).ToList();
    }
}


// Endpoint
public class UsersGetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender) =>
        {
            APIResponse response = new();
            var query = new GetUsersQuery();
            var users = await sender.Send(query);

            response.Result = users;
            response.IsSuccess = true;
            response.StatusCode = users.Any() ? HttpStatusCode.OK : HttpStatusCode.NotFound;

            return Results.Ok(response);
        })
        .RequireAuthorization("AdminOnly")
        .Produces<APIResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetUsers")
        .WithSummary("Get Users")
        .WithDescription("Retrieve all users");
    }
}
