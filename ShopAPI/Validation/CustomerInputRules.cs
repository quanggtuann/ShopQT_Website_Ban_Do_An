using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ShopAPI.Validation;

public static class CustomerInputRules
{
    public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{7,}$";
    public const string PhonePattern = @"^0[0-9]{9}$";
    public const string EmailPattern = @"^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@[Gg][Mm][Aa][Ii][Ll]\.[Cc][Oo][Mm]$";

    public static bool IsValidPassword(string? value) =>
        !string.IsNullOrEmpty(value) && Regex.IsMatch(value, PasswordPattern, RegexOptions.CultureInvariant);

    public static bool IsValidPhone(string? value) =>
        !string.IsNullOrEmpty(value) && Regex.IsMatch(value, PhonePattern, RegexOptions.CultureInvariant);

    public static bool IsValidEmail(string? value) =>
        !string.IsNullOrEmpty(value)
        && Regex.IsMatch(value, EmailPattern, RegexOptions.CultureInvariant)
        && new EmailAddressAttribute().IsValid(value);
}
