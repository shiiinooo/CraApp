﻿using CraApp.Web.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraApp.Web.Controllers;

public class MonthlyActivitiesController : Controller
{
    private readonly IMonthlyActivitiesService _monthlyActivityService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    public MonthlyActivitiesController(IMonthlyActivitiesService monthlyActivityService, IUserService userService, IMapper mapper)
    {
        _monthlyActivityService = monthlyActivityService;
        _userService = userService;
        _mapper = mapper;
    }

    //[Authorize(Roles = "admin")]
    public async Task<IActionResult> IndexMonthlyActivities()
    {
        List<MonthlyActivitiesDTO> list = new();

        //var response = await _monthlyActivityService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
        var response = await _userService.GetUserWithActivitiesAsync<APIResponse>(SD.UserId, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<MonthlyActivitiesDTO>>(Convert.ToString(response.Result));
        }
        return View(list);
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateMonthlyActivity()
    {
        return View();
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMonthlyActivity(MonthlyActivityCreateDTO model)
    {
        var response = await _monthlyActivityService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Monthly activity created successfully";
            return RedirectToAction("IndexMonthlyActivities");
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateMonthlyActivity(int id)
    {
        var response = await _monthlyActivityService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            MonthlyActivitiesDTO model = JsonConvert.DeserializeObject<MonthlyActivitiesDTO>(Convert.ToString(response.Result));
            return View(_mapper.Map<MonthlyActivityUpdateDTO>(model));
        }
        return NotFound();
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateMonthlyActivity(MonthlyActivityUpdateDTO model)
    {
        if (ModelState.IsValid)
        {
            var response = await _monthlyActivityService.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Monthly activity updated successfully";
                return RedirectToAction(nameof(IndexMonthlyActivities));
            }
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteMonthlyActivity(int id)
    {
        var response = await _monthlyActivityService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            MonthlyActivitiesDTO model = JsonConvert.DeserializeObject<MonthlyActivitiesDTO>(Convert.ToString(response.Result));
            return View(model);
        }
        return NotFound();
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMonthlyActivity(MonthlyActivitiesDTO model)
    {
        var response = await _monthlyActivityService.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Monthly activity deleted successfully";
            return RedirectToAction(nameof(IndexMonthlyActivities));
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }


   // [Authorize(Roles = "admin")]
    public async Task<IActionResult> ActivitiesByMonthAndUser(int userId, int month)
    {
        var response = await _monthlyActivityService.GetActivitiesByMonthAndUser<APIResponse>(userId, month, HttpContext.Session.GetString(SD.SessionToken));

        if (response != null && response.IsSuccess)
        {
            var activitiesDTO = JsonConvert.DeserializeObject<IEnumerable<MonthlyActivitiesDTO>>(Convert.ToString(response.Result));
            return View(activitiesDTO);
        }

        TempData["error"] = "No activities found for this user and month.";
        return RedirectToAction(nameof(IndexMonthlyActivities));
    }






}
