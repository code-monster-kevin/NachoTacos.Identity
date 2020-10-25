namespace NachoTacos.Identity.Model.Constants
{
    public enum LoginStatus
    {
        SUCCEEDED = 1,
        IS_LOCKED_OUT = 2,
        IS_NOT_ALLOWED = 3,
        REQUIRES_TWO_FACTOR = 4,
        FAILED_LOGIN = 5
    }
}
