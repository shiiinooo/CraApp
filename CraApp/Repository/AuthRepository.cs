using MapsterMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CraApp.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private string secretKey;
    public AuthRepository(AppDbContext db, IMapper mapper, IConfiguration configuration)
    {
        _db = db;
        _mapper = mapper;
        _configuration = configuration;
        secretKey = _configuration.GetValue<string>("ApiSettings:Secret");
    }
    public bool IsUniqueUser(string username)
    {
        var user = _db.Users.FirstOrDefault(user => user.UserName == username);
        if (user == null)
        {
            return true;
        }
        return false;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
    {
        var user = await _db.Users.SingleOrDefaultAsync(user => user.UserName == loginRequestDTO.UserName && user.Password == loginRequestDTO.Password);
        Console.WriteLine(user);
        if (user == null)
        {
            return null;
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        LoginResponseDTO loginResponseDTO = new()
        {
            User = _mapper.Map<UserDTO>(user),
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
        return loginResponseDTO;
    }


    public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
    {
        User userObj = _mapper.Map<User>(registrationRequestDTO);

        // Use the role from the request DTO, or set a default if necessary
        if (string.IsNullOrEmpty(userObj.Role.ToString()))
        {
            userObj.Role = Role.user; // Set default role if none provided
        }

        _db.Users.Add(userObj);
        await _db.SaveChangesAsync(); // Use async version for non-blocking operation

        userObj.Password = ""; // Clear the password field

        return _mapper.Map<UserDTO>(userObj);
    }

}