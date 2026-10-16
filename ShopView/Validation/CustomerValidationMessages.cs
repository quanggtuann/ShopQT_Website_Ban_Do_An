using System.Globalization;

namespace ShopView.Validation;

public static class CustomerValidationMessages
{
    private static bool IsVietnamese => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "vi";

    public static string Required => IsVietnamese
        ? "Vui lòng nhập thông tin này."
        : "This field is required.";

    public static string Password => IsVietnamese
        ? "Mật khẩu phải có ít nhất 7 ký tự, gồm chữ thường, chữ hoa và chữ số."
        : "Password must be at least 7 characters and include a lowercase letter, an uppercase letter, and a digit.";

    public static string PasswordConfirmation => IsVietnamese
        ? "Mật khẩu xác nhận không khớp."
        : "Password confirmation does not match.";

    public static string Phone => IsVietnamese
        ? "Số điện thoại phải gồm đúng 10 chữ số và bắt đầu bằng 0."
        : "Phone number must contain exactly 10 digits and start with 0.";

    public static string Email => IsVietnamese
        ? "Vui lòng nhập email hợp lệ với đuôi @gmail.com."
        : "Enter a valid email address ending in @gmail.com.";

    public static string RegistrationFailed => IsVietnamese
        ? "Không thể đăng ký tài khoản. Vui lòng thử lại."
        : "Could not create your account. Please try again.";

    public static string EmailExists => IsVietnamese
        ? "Email này đã được sử dụng."
        : "This email is already in use.";

    public static string UsernameExists => IsVietnamese
        ? "Tên đăng nhập này đã được sử dụng."
        : "This username is already in use.";
}
