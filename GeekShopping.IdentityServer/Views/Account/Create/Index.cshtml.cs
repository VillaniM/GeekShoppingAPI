using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using GeekShopping.IdentityServer.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GeekShopping.IdentityServer.MainModule.Create;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events; // ✅ adicionado

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public Index(
        IIdentityServerInteractionService interaction,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IEventService events) // ✅ adicionado
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _interaction = interaction;
        _events = events; // ✅ adicionado
    }

    public IActionResult OnGet(string? returnUrl)
    {
        Input = new InputModel { ReturnUrl = returnUrl };
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        if (Input.Button != "create")
        {
            if (context != null)
            {
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                if (context.IsNativeClient())
                    return this.LoadingPage(Input.ReturnUrl);

                return Redirect(Input.ReturnUrl ?? "~/");
            }
            else
            {
                return Redirect("~/");
            }
        }

        if (await _userManager.FindByNameAsync(Input.Username!) != null)
        {
            ModelState.AddModelError("Input.Username", "Invalid username");
        }

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                Input.Username!,
                Input.Password!,
                false, // isPersistent: false
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return Page();
            }

            var user = await _userManager.FindByNameAsync(Input.Username!);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return Page();
            }

            // ✅ Corrigido: await + ordem correta dos parâmetros + sem parêntese sobrando
            await _events.RaiseAsync(new UserLoginSuccessEvent(
                provider: "local",        // provider (tipo de login)
                providerUserId: user.Id,  // providerUserId
                subjectId: user.Id,       // subjectId
                name: user.UserName,     // name (display name)
                clientId: context?.Client.ClientId));

            // ✅ Corrigido: UserName com N maiúsculo
            var isuser = new IdentityServerUser(user.Id)
            {
                DisplayName = user.UserName
            };

            await HttpContext.SignInAsync(isuser);

            if (context != null)
            {
                if (context.IsNativeClient())
                    return this.LoadingPage(Input.ReturnUrl);

                return Redirect(Input.ReturnUrl ?? "~/");
            }

            if (Url.IsLocalUrl(Input.ReturnUrl))
                return Redirect(Input.ReturnUrl);
            else if (string.IsNullOrEmpty(Input.ReturnUrl))
                return Redirect("~/");
            else
                throw new ArgumentException("invalid return URL");
        }

        return Page();
    }
}