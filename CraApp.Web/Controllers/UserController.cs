using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraApp.Web.Controllers;

public class UserController : Controller
{
    private readonly IUserService _villaService;
    private readonly IMapper _mapper;
    public UserController(IUserService villaService, IMapper mapper)
    {
        _villaService = villaService;
        _mapper = mapper;
    }

    public async Task<IActionResult> IndexUser()
    {
        List<UserDTO> list = new();

        var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
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
    public async Task<IActionResult> CreateUser(UserCreateDTO model)
    {
        if (ModelState.IsValid)
        {

            var response = await _villaService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa created successfully";
                return RedirectToAction(nameof(IndexUser));
            }
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateUser(int villaId)
    {
        var response = await _villaService.GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
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
            TempData["success"] = "Villa updated successfully";
            var response = await _villaService.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexUser));
            }
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUser(int villaId)
    {
        var response = await _villaService.GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
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
    public async Task<IActionResult> DeleteVilla(UserDTO model)
    {

        var response = await _villaService.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Villa deleted successfully";
            return RedirectToAction(nameof(IndexUser));
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }

}
