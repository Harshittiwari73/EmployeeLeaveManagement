using EmployeeLeaveManagement.Models;
using EmployeeLeaveManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace EmployeeLeaveManagement.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IJwtTokenService _jwtTokenService;

    public AccountController(IUserService userService, IJwtTokenService jwtTokenService)
    {
        _userService = userService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToDashboard();
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = await _userService.AuthenticateAsync(model.Email, model.Password);
            if (user != null)
            {
                if (!user.IsActive)
                {
                    ModelState.AddModelError("", "Your account is inactive. Please contact admin.");
                    return View(model);
                }

                var token = _jwtTokenService.GenerateToken(user);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = model.RememberMe
        ? DateTime.UtcNow.AddDays(7)
        : DateTime.UtcNow.AddHours(2)
                };
                Response.Cookies.Append("jwtToken", token, cookieOptions);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToDashboard();
            }
            ModelState.AddModelError("", "Invalid email or password.");
        }
        return View(model);
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwtToken");
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToDashboard()
    {
        if (User.IsInRole("Admin")) return RedirectToAction("Index", "Home");
        if (User.IsInRole("Manager")) return RedirectToAction("Index", "LeaveApprovals");
        return RedirectToAction("Index", "LeaveRequests");
    }
}
