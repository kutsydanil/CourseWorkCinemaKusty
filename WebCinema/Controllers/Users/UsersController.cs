using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;
using WebCinema.Models.Users;

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
                var user_list = _userManager.Users.ToList();
                var isNew = true;
                foreach (var u in user_list)
                {
                    if (u.Email == model.Email && u.UserName == model.Email)
                    {
                        isNew = false;
                        break;
                    }
                }
                if (isNew)
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
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь уже существует.");
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
                if (identityUser != null)
                {
                    if (!await _userManager.IsInRoleAsync(identityUser, "admin"))
                    {
                        IdentityResult result = await _userManager.DeleteAsync(identityUser);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var users = _userManager.GetUsersInRoleAsync("admin").Result;
                        if (users.Count != 1)
                        {
                            IdentityResult result = await _userManager.DeleteAsync(identityUser);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            model = new DeleteUserViewModel
                            {
                                Email = identityUser.Email,
                                Id = identityUser.Id,
                                UserRoles = await _userManager.GetRolesAsync(identityUser)
                            };
                            ModelState.AddModelError(string.Empty, "Это последний пользователь с admin. Удалить невозможно.");
                            return View(model);
                        }
                    }
                }
                return View(model); 
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

        //Todo Сделать проверку на администратора: проверку при удалении на то, что администратор последний я сделал (т.е. удалить невозможно)..Нужно такую же вещь сделать и с изменение ролей(нельзя у админа убрать роль admin, если он последний в системе)
        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(string id, List<string> roles)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(id);
            if (identityUser != null)
            {
                var userRoles = await _userManager.GetRolesAsync(identityUser);
                var user_list = await _userManager.GetUsersInRoleAsync("admin");
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);


                if (await _userManager.IsInRoleAsync(identityUser, "admin") && user_list.Count == 1 && removedRoles.Contains("admin"))
                {
                    ModelState.AddModelError(string.Empty, "Невозможно убрать роль admin для последнего администратора системы");
                    ChangeUserRoleViewModel model = new ChangeUserRoleViewModel
                    {
                        Id = identityUser.Id,
                        AllRoles = _roleManager.Roles.ToList(),
                        UserRoles = await _userManager.GetRolesAsync(identityUser),
                        Email = identityUser.Email,
                    };
                    return View(model);
                }
                else
                {
                    await _userManager.AddToRolesAsync(identityUser, addedRoles);
                    await _userManager.RemoveFromRolesAsync(identityUser, removedRoles);
                    var result = await _userManager.UpdateAsync(identityUser);
                    if (result.Succeeded)
                    {
                        var currentUserId = _userManager.GetUserId(HttpContext.User);
                        if (currentUserId == identityUser.Id)
                        {
                            await _signInManager.RefreshSignInAsync(await _userManager.FindByIdAsync(currentUserId));
                        }
                        return RedirectToAction("Index");
                    }
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
                    if(identityUser.Email == model.Email)
                    {
                        var changePasswordResult = await _userManager.ChangePasswordAsync(identityUser, model.OldPassword, model.NewPassword);
                        if (!changePasswordResult.Succeeded)
                        {
                            foreach (var error in changePasswordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                        else
                        {
                            var currentUserId = _userManager.GetUserId(HttpContext.User);
                            if(currentUserId == identityUser.Id)
                            {
                                await _signInManager.RefreshSignInAsync(await _userManager.FindByIdAsync(currentUserId));
                            }
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        var user_list = _userManager.Users.ToList();
                        var isNew = true;
                        foreach (var u in user_list)
                        {
                            if (u.Email == model.Email && u.UserName == model.Email)
                            {
                                isNew = false;
                                break;
                            }
                        }
                        if (isNew)
                        {
      
                            var code = await _userManager.GenerateChangeEmailTokenAsync(identityUser, model.Email);
                            var result = await _userManager.ChangeEmailAsync(identityUser, model.Email, code);
                            var setUserNameResult = await _userManager.SetUserNameAsync(identityUser, model.Email);
                            var changePasswordResult = await _userManager.ChangePasswordAsync(identityUser, model.OldPassword, model.NewPassword);

                            if (result.Succeeded && setUserNameResult.Succeeded && changePasswordResult.Succeeded)
                            {
                                var currentUserId = _userManager.GetUserId(HttpContext.User);
                                if(currentUserId == identityUser.Id)
                                {
                                    await _signInManager.RefreshSignInAsync(await _userManager.FindByIdAsync(currentUserId));
                                }
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                foreach (var error in changePasswordResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                                foreach (var error in setUserNameResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                            }
                        }
                    }  
                }
            }
            return View(model);
        }

    }
}
