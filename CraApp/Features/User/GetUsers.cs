﻿namespace CraApp.Features.User;

// Query
public record GetUsersQuery() : IQuery<IEnumerable<GetUserResult>>;

//Result
public record GetUserResult(int Id, string UserName, string Name);

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
        var users = await _repository.GetUsersAsync(cancellationToken);

        return users.Select(u => new GetUserResult(u.Id, u.UserName, u.Name)).ToList();
    }
}

// Endpoint
public class UsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender) =>
        {
            var query = new GetUsersQuery();
            var users = await sender.Send(query);

            if (users == null || !users.Any())
            {
                return Results.NotFound();
            }

            return Results.Ok(users);
        })
        .Produces<IEnumerable<UserDTO>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetUsers")
        .WithSummary("Get Users")
        .WithDescription("Retrieve all users");
    }
}