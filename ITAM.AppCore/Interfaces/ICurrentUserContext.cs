using ITAM.Domain.Entities.Identity;

public interface ICurrentUserContext
{
    User? Instance { get; }
    void Set(User user);
    void Clear();
    bool HasFeature(string code);
}