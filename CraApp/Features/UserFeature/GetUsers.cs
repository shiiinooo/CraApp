namespace CraApp.Features.UserFeature;

// Query
public record GetUsersQuery() : IQuery<IEnumerable<GetUsersResult>>;

//Result
public record GetUsersResult(int Id, string UserName, string Name, Role Role);

// Handler
internal class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IEnumerable<GetUsersResult>>
{
    private readonly IUserRepository _repository;

    public GetUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GetUsersResult>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync(cancellationToken);

        if (users == null || !users.Any())
        {
            return Enumerable.Empty<GetUsersResult>();
        }

        return users.Select(u => new GetUsersResult(u.Id, u.UserName, u.Name, u.Role)).ToList();
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
