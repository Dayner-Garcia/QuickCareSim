
using QuickCareSim.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuickCareSim.Application.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nombre obligatorio")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Apellido Obligatorio")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Numero de telefono obligatorio")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Nombre de usuario obligatorio")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Correo obligatorio")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Contraseña obligatoria")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Debes de repetir la contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string RepeatPassword { get; set; }
        public Roles Role { get; set; }
    }
}
