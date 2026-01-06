using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Users()
    {
        return View(_userManager.Users.ToList());
    }

    public async Task<IActionResult> MakeAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        return RedirectToAction(nameof(UsersRole));
    }

    public async Task<IActionResult> RemoveAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }
        return RedirectToAction(nameof(UsersRole));
    }

    public async Task<IActionResult> UsersRole()
    {
        var users = _userManager.Users.ToList();
        var model = new List<(IdentityUser User, IList<string> Roles)>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            model.Add((user, roles));
        }

        return View(model);
    }


}
