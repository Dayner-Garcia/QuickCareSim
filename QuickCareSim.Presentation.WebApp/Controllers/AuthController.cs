using Microsoft.AspNetCore.Mvc;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.ViewModels.Auth;

namespace QuickCareSim.Presentation.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                await _authService.RegisterAsync(model);

                TempData["SucessMessage"] = "Registro exitoso.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("El correo electrónico ya está registrado"))
                    ModelState.AddModelError("Email", ex.Message);

                else if (ex.Message.Contains("El nombre de usuario ya está en uso."))
                    ModelState.AddModelError("UserName", ex.Message);

                else
                    ModelState.AddModelError(string.Empty, ex.Message);

                TempData["ErrorMessage"] = $"Error: {ex.Message}";

                return View(model);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loginResult = await _authService.LoginAsync(model);

            if (!loginResult.Success)
            {
                ViewBag.Error = loginResult.Message;
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Activate(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "El correo electronico o el token esta vacio";
                return RedirectToAction("Login");
            }

            var activationSuccess = await _authService.ActivateUserAsync(email, token);

            if (!activationSuccess)
            {
                TempData["ErrorMessage"] = "El usuario no se pudo activar";
                return RedirectToAction("Login");
            }

            TempData["SucessMessage"] = "El usuario se activo correctamente";
            return RedirectToAction("Login");
        }
    }
}
