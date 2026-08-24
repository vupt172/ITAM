using ITAM.Domain.Entities.Identity;

public class CurrentUserContext : ICurrentUserContext
{
    private User? _user;
    public User? Instance => _user;
    public void Set(User user) => _user = user;
    public void Clear() => _user = null;
    public bool HasFeature(string code) =>
        _user?.UserRoles.SelectMany(ur => ur.Role.RoleFeatures)
             .Any(rf => rf.Feature.Code == code) ?? false;
}