using Microsoft.AspNetCore.Mvc;

namespace CraApp.Web.Controllers;

public class ActivitiesController : Controller
{
    private readonly IActivityService _activityService;

    public ActivitiesController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    public IActionResult CreateActivities()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateActivities(ActivityCreateDTO model)
    {
        model.MonthlyActivityId = SD.MonthlyActivitiesId;
        var response = await _activityService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Activity created successfully";
            return RedirectToAction("IndexMonthlyActivities", "MonthlyActivities");
        }
        TempData["error"] = "Error encountered.";
        return View(model);
    }
}