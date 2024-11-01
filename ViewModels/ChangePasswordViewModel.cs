using System.ComponentModel.DataAnnotations;

namespace SampleSecureWeb.ViewModels
{
    public class ChangePasswordViewModel
    {
         [Required]
    public string CurrentPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Password harus minimal {2} karakter dan maksimal {1} karakter.", MinimumLength = 6)]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Konfirmasi Password")]
    [Compare("NewPassword", ErrorMessage = "Password dan konfirmasi password tidak sama.")]
    public string ConfirmNewPassword { get; set; }
    }
}
