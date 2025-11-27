namespace QvrProClient.Api;

/// <summary>
/// Known QVR Pro endpoints used by the high-level client.
/// </summary>
public static class QvrProRoutes
{
    public const string Login = "/cgi-bin/authLogin.cgi";
    public const string Logout = "/cgi-bin/authLogout.cgi";
    public const string Cameras = "/qvrpro/api/cameras";
    public const string Recordings = "/qvrpro/api/recordings";
    public const string Qplay = "/qvrpro/apis/qplay.cgi";
}
