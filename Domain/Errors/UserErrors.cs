using System.Net;

namespace Domain.Errors
{
    public static class UserErrors
    {
        public static Error PhoneAlreadyRegistered =>
            new("User.PhoneAlreadyRegistered", "رقم الهاتف مسجل بالفعل بهذا النوع من الحساب.", HttpStatusCode.Conflict);

        public static Error UserNotFound =>
            new("User.UserNotFound", "المستخدم غير موجود.", HttpStatusCode.NotFound);

        public static Error PhoneNumberNotConfirmed =>
            new("User.PhoneNumberNotConfirmed", "رقم الهاتف لم يتم تأكيده بعد.", HttpStatusCode.BadRequest);

        public static Error AccountDisabled =>
            new("User.AccountDisabled", "الحساب معطل.", HttpStatusCode.Forbidden);

        public static Error Unauthorized =>
           new("User.Unauthorized", "غير مصرح", HttpStatusCode.Unauthorized);
    }
}