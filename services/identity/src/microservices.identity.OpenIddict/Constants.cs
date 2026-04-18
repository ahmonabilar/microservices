namespace microservices.identity.Services;

public static class Constants
{
    public const string RegistrationSession = "registration";

    public const string ResetPasswordSession = "resetpassword";

    public class SessionProperties
    {
        public const string UserId = "user_id";
        public const string Code = "code";
        public const string ReturnUrl = "return_url";
        public const string Step = "step";
        public const string ResetPasswordToken = "reset_password_token";
    }

    public class Step
    {
        public const string ValidateRegistrationCode = "ValidateRegistrationCode";
        public const string CompleteRegistration = "CompleteRegistration";
        public const string ValidateResetPasswordCode = "ValidateResetPasswordCode";
        public static string SetPassword = "SetPassword";
    }
}