using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.IdentityServer.Pages.Home.Error;

[AllowAnonymous]
[SecurityHeaders]
public class ErrorController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IWebHostEnvironment _environment;

    public ErrorController(IIdentityServerInteractionService interaction, 
                           IWebHostEnvironment environment)
    {
        _interaction = interaction;
        _environment = environment;
    }

    public async Task<IActionResult> Index(string? errorId)
    {
        var vm = new ViewModel();
        var message = await _interaction.GetErrorContextAsync(errorId);
        if (message != null)
        {
            vm.Error = message;
            if (!_environment.IsDevelopment())
            {
                message.ErrorDescription = null;
            }
        }
        return View(vm);
    }
}