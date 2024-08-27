
namespace shopping.api.Shared.Constant
{
    public static class Message
    {
        public static readonly string Success = "SUCCESS";
        public static readonly string LoginFail = "LOGIN_FAIL";
        public static readonly string DataNotFound = "DATA_NOT_FOUND";
        public static readonly string ClickFail = "CLICK_FAIL";
        public static readonly string UpdateMemberFail = "UPDATE_MEMBER_FAIL";
        public static readonly string Fail = "FAIL";
        public static readonly string NoData = "NO_DATA";
        public static readonly string OutOfStock = "OUT_OF_STOCK";
        public static readonly string EditFail = "EDIT_FAIL";
        public static readonly string Ngi000 = "NG_I_0000";
        public static readonly string Ngf9101 = "NG_F_9101";
        public static readonly string Ngf9106 = "NG_F_9106";
        public static readonly string Ngf9107 = "NG_F_9107";
        public static readonly string ChangePasswordSuccess = "CHANGE_PASSWORD_SUCCESS";
        public static readonly string IngUpSuccess = "ING_UP_SUCCESS";
        public static readonly string EditProfileSuccess = "EDIT_PROFILE_SUCCESS";
        public static readonly string Ngf9999 = "NG_F_9999";
        public static readonly string ServerError = "SERVER_ERROR";
        public static readonly string DeleteFail = "DELETE_FAIL";

    }
    public struct LoginMessage
    {
        public static readonly string LoginSuccess = "LOGIN_SUCCESS";
        public static readonly string UserOrPasswordIncorrect = "USER_OR_PASSWORD_INCORRECT";
        public static readonly string UserDisable = "USER_DISABLE";
        public static readonly string RefreshTokenExpired = "Refresh token expired please sign out and sing in again.";
        public static readonly string UserOrPasswordIsIncorrect = "User or Password is incorrect.";
    }
}