using CraApp.Web.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraApp.Web.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    public UserController(IUserService userService, IAuthService authService, IMapper mapper)
    {
        _userService = userService;
        _authService = authService;
        _mapper = mapper;
    }

    public async Task<IActionResult> IndexUser()
    {
        List<UserDTO> list = new();

        var response = await _userService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<UserDTO>>(Convert.ToString(response.Result));
        }
        return View(list);
    }
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateUser()
    {
        return View();
    }
   
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(RegisterationRequestDTO model)
    {
        APIResponse result = await _authService.RegisterAsync<APIResponse>(model);
        if (result != null && result.IsSuccess)
        {
            TempData["success"] = "User created successfully";
            return RedirectToAction("IndexUser");
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateUser(int userId)
    {
        var response = await _userService.GetAsync<APIResponse>(userId, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {

            UserDTO model = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(response.Result));
            return View(_mapper.Map<UserUpdateDTO>(model));
        }
        return NotFound();
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser(UserUpdateDTO model)
    {
        if (ModelState.IsValid)
        {
            TempData["success"] = "User updated successfully";
            var response = await _userService.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexUser));
            }
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }
   
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var response = await _userService.GetAsync<APIResponse>(userId, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            UserDTO model = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(response.Result));
            return View(model);
        }
        return NotFound();
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(UserDTO model)
    {

        var response = await _userService.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "User deleted successfully";
            return RedirectToAction(nameof(IndexUser));
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }

}
