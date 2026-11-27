namespace Application.Constants
{
    public static class SmsMessagesConstants
    {
        public static string SignUpOtp(string otpCode)
            => $"كود تأكيد إنشاء حسابك في Yalla هو: {otpCode}. لا تشارك الكود مع أي شخص.";

        public static string LoginOtp(string otpCode)
            => $"كود تسجيل الدخول إلى Yalla هو: {otpCode}. لا تشارك الكود مع أي شخص.";

        public static string ResetPasswordOtp(string otpCode)
            => $"كود إعادة تعيين كلمة المرور في Yalla هو: {otpCode}. لا تشارك الكود مع أي شخص.";
    }
}