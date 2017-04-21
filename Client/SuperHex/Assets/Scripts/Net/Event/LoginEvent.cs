public class LoginEvent:BaseEvent
{
    public const string LoginRequest_SUCCESS = "loginRequestSuccess";
    public const string LOGIN_SUCCESS_ForUI = "loginSuccessForUI";
    public const string LOGIN_ERROR = "loginError";
    public const string LOGIN_ROOM_ERROR = "loginRoomError";
    public const string LOGIN_SUCCESS = "loginSuccess";
    public const string UPDATE_ACCOUNT = "UpdateAccount";
    public const string UPDATE_PASSWORD = "UpdatePassword";
    public const string UPDATE_NAME = "UpdateName";

    public LoginEvent(string type, object eventObj):base(type, eventObj)
    {

    }
}
