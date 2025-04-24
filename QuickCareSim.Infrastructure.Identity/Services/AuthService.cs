using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using QuickCareSim.Application.Interfaces.Services.Core;
using QuickCareSim.Application.ViewModels.Auth;
using QuickCareSim.Domain.Enums;
using QuickCareSim.Infrastructure.Identity.Entities;
using QuickCareSim.Infrastructure.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Encodings.Web;

namespace QuickCareSim.Infrastructure.Identity.Services
{
    internal class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IEmailService emailService, IMapper mapper, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, HtmlEncoder htmlEncoder,
            IHttpContextAccessor httpContextAccessor)
        {
            _emailService = emailService;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _htmlEncoder = htmlEncoder;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RegisterAsync(RegisterViewModel user)
        {
            if (user.Password != user.RepeatPassword)
                throw new Exception("Passwords do not match");

            var existingUserByEmail = await _userManager.FindByEmailAsync(user.Email);
            if (existingUserByEmail != null)
                throw new Exception("Correo ya registrado");

            var existingUserByUserName = await _userManager.FindByNameAsync(user.UserName);
            if (existingUserByUserName != null)
                throw new Exception("Nombre de usuario ya registrado");

            var newUser = _mapper.Map<ApplicationUser>(user);
            newUser.EmailConfirmed = false;

            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded)
                throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var activationLink = GenerateActivationLink(user.Email, token);
            await SendActivationEmailAsync(user.Email, activationLink);
        }

        private string GenerateActivationLink(string email, string token)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                throw new InvalidOperationException("HttpContext is not available.");

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/Auth/activate?email={email}&token={_htmlEncoder.Encode(token)}";
        }

        private async Task SendActivationEmailAsync(string email, string activationLink)
        {
            string subject = "¡Bienvenido a QuickCareSim! 🌟";
            string body =
                $@"<div style='font-family: Arial, sans-serif; text-align: center; padding: 20px;'>
                <h1 style='color: #4CAF50;'>¡Estamos emocionados de tenerte con nosotros!</h1>
                <p>Gracias por registrarte en <strong>QuickCareSim</strong>. Solo queda un último paso para empezar a disfrutar de nuestros servicios.</p>
                <p>Haz clic en el botón de abajo para activar tu cuenta:</p>
                <a href='{activationLink}' style='display: inline-block; padding: 10px 20px; margin: 20px 0; color: white; background-color: #4CAF50; text-decoration: none; border-radius: 5px;'>Activar cuenta</a>
                <p>Si no solicitaste esta cuenta, puedes ignorar este mensaje.</p>
                <p style='color: #888;'>Gracias por confiar en <strong>QuickCareSim</strong>. ¡ Te esperamos !</p>
        </div>";

            await _emailService.SendEmailAsync(email, subject, body);
        }


        public async Task<LoginResult> LoginAsync(LoginViewModel user)
        {
            var userToLogin = await _userManager.FindByEmailAsync(user.Username)
                              ?? await _userManager.FindByNameAsync(user.Username);

            if (userToLogin == null)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos"
                };
            }

            if (!userToLogin.EmailConfirmed)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Usuario no confirmado, revisa tu correo"
                };
            }

            var result = await _signInManager.PasswordSignInAsync(userToLogin.UserName!, user.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos."
                };
            }

            var roles = await _userManager.GetRolesAsync(userToLogin);

            return new LoginResult
            {
                isAdmin = roles.Contains(Roles.DOCTOR.ToString()),
                Success = true
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> ActivateUserAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return false;

            user.EmailConfirmed = true;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}