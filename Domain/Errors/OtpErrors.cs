using System.Net;

namespace Domain.Errors
{
    public static class OtpErrors
    {
        public static Error CooldownActive =>
            new("Otp.CooldownActive", "من فضلك انتظر 30 ثانية قبل طلب رمز تحقق جديد.", HttpStatusCode.TooManyRequests);

        public static Error DailyLimitExceeded =>
            new("Otp.DailyLimitExceeded", "لقد تجاوزت الحد الأقصى لطلبات رمز التحقق اليوم. يرجى المحاولة مرة أخرى بعد انتهاء فترة الانتظار.", HttpStatusCode.TooManyRequests);

        public static Error SmsProviderFailed =>
            new("Otp.SmsProviderFailed", "فشل إرسال رمز التحقق، برجاء المحاولة مرة أخرى.", HttpStatusCode.BadGateway);

        public static Error InvalidPhoneNumber =>
            new("Otp.InvalidPhoneNumber", "رقم الهاتف غير صحيح.", HttpStatusCode.BadRequest);

        public static Error ServiceBusy =>
            new("Otp.ServiceBusy", "الخدمة غير متاحة حاليًا، برجاء المحاولة لاحقًا.", HttpStatusCode.ServiceUnavailable);

        public static Error InvalidOtpCode =>
            new("Otp.InvalidOtpCode", "كود التحقق غير صحيح", HttpStatusCode.BadRequest);
    }
}