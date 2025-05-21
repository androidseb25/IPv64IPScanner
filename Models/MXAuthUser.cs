namespace IPv64IPScanner;

public class Features
{
}

public class Membership
{
    public string MemberType { get; set; }
}

public class MXAuthUser
{
    public string UserId { get; set; }
    public object UserName { get; set; }
    public object FirstName { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsMasquerade { get; set; }
    public bool IsPaidUser { get; set; }
    public bool IsLoggedIn { get; set; }
    public string MxVisitorUid { get; set; }
    public string TempAuthKey { get; set; }
    public bool IsPastDue { get; set; }
    public object BouncedEmailOn { get; set; }
    public int NumDomainHealthMonitors { get; set; }
    public int NumDisabledMonitors { get; set; }
    public object XID { get; set; }
    public string AGID { get; set; }
    public Membership Membership { get; set; }
    public Features Features { get; set; }
    public string CognitoSub { get; set; }
    public bool HasBetaAccess { get; set; }
    public bool IsOnTrial { get; set; }
    public int PermissionType { get; set; }
}