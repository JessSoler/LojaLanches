﻿using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using NuGet.Protocol.Plugins;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LanchesMac.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //usado para gerenciar operações de usuário, como criação e autenticação
        private readonly UserManager<IdentityUser> _userManager;
        //usado para gerenciar o processo de login
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signManager)
        {
            _userManager = userManager;
            _signInManager = signManager;
        }

        //implementar login, registro e logout
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            var actualUrl = HttpContext.Request.Path;
            string ReturnUrl = HttpContext.Request.Body.ToString();

            return View(new LoginViewModel()
            {
                ReturnUrl = returnUrl
            });
        }

        //usado para processar os dados de login submetidos pelo usuário.
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByNameAsync(loginVM.UserName);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginVM.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        this.ModelState.AddModelError("Login", "Falha ao realizar o login, verifique o usuário/senha usadaos");
                    }
                    return Redirect(loginVM.ReturnUrl);
                }
            }
            ModelState.AddModelError("", "Usuário/Senha inválidos ou não localizados!!");
            return View(loginVM);
        }

        //permite que os usuários acessem uma página de registro
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //usado para processar os dados de registro submetidos pelo usuário.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel registroVM)
        {

            if (ModelState.IsValid)
            {
                var user = new IdentityUser() { UserName = registroVM.UserName };
                var result = await _userManager.CreateAsync(user, registroVM.Password);

                if (result.Succeeded)
                {
                    //// Adiciona o usuário padrão ao perfil Member
                    await _userManager.AddToRoleAsync(user, "Member");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("LoggedIn", "Account");
                }
                else
                {
                    this.ModelState.AddModelError("Registro", "Falha ao realizar o registro, verifique o usuário/senha usados");
                }

            }
            return View(registroVM);
        }

        //permite que os usuários acessem a página "LoggedIn" após um registro bem-sucedido
        [AllowAnonymous]
        public ViewResult LoggedIn() => View();

        //permite que os usuários façam logout de suas contas
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}