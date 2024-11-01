using System.ComponentModel.DataAnnotations;

namespace SampleSecureWeb.ViewModels
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(12, ErrorMessage = "Password harus minimal 12 karakter.")]
        // Hapus aturan RegularExpression
        public string Password { get; set; }

        [Required(ErrorMessage = "Konfirmasi Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Konfirmasi Password")]
        [Compare("Password", ErrorMessage = "Password dan konfirmasi password tidak sesuai.")]
        public string? ConfirmPassword { get; set; }
    }
}
