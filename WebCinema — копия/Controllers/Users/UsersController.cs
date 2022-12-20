using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.InteropServices;
using WebCinema.ViewModel.Users;

namespace WebCinema.Controllers.Users
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<IdentityUser> _signInManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser newIdentityUser = new IdentityUser { Email = model.Email, UserName = model.Email };
                var result = await _userManager.CreateAsync(newIdentityUser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newIdentityUser, "employee");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(newIdentityUser);
                    await _userManager.ConfirmEmailAsync(newIdentityUser, code);
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(id);
            if(identityUser == null)
            {
                return NotFound();
            }
            DeleteUserViewModel model = new DeleteUserViewModel
            {
                Email = identityUser.Email,
                Id = identityUser.Id,
                UserRoles = await _userManager.GetRolesAsync(identityUser)
            }; 
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(DeleteUserViewModel model)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(model.Id);
            if(identityUser != null) 
            {
                IdentityResult result = await _userManager.DeleteAsync(identityUser);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangeUserRole(string id)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser == null)
            {
                return NotFound();
            }
            ChangeUserRoleViewModel model = new ChangeUserRoleViewModel
            {
                Id = identityUser.Id,
                AllRoles = _roleManager.Roles.ToList(),
                UserRoles = await _userManager.GetRolesAsync(identityUser),
                Email = identityUser.Email,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(string id, List<string> roles)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser != null)
            {
                var userRoles = await _userManager.GetRolesAsync(identityUser);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(identityUser, addedRoles);
                await _userManager.RemoveFromRolesAsync(identityUser, removedRoles);

                var result = await _userManager.UpdateAsync(identityUser);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(identityUser);
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(string id)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(id);
            if(identityUser == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel
            {
                Id = identityUser.Id,
                Email = identityUser.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityUser identityUser = await _userManager.FindByIdAsync(model.Id);
                if(identityUser != null)
                {
                    identityUser.Email = model.Email;
                    var _userValidator = HttpContext.RequestServices.GetService(typeof(IUserValidator<IdentityUser>)) as IUserValidator<IdentityUser>;
                    IdentityResult result_user = await _userValidator.ValidateAsync(_userManager, identityUser);

                    var _passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<IdentityUser>)) as IPasswordValidator<IdentityUser>;
                    var _passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<IdentityUser>)) as IPasswordHasher<IdentityUser>;

                    IdentityResult result_password = await _passwordValidator.ValidateAsync(_userManager, identityUser, model.NewPassword);
                    if (result_password.Succeeded && result_user.Succeeded)
                    {
                        identityUser.PasswordHash = _passwordHasher.HashPassword(identityUser, model.NewPassword);
                        await _userManager.UpdateAsync(identityUser);
                        await _signInManager.RefreshSignInAsync(identityUser);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result_password.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        foreach (var error in result_user.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

    }
}
