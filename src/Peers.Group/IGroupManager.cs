namespace Peers.Group;

public interface IGroupManager
{
    Task<Group> CreateGroup(string name);
    Task<Group> GetGroup(Guid groupId);
    Task<Group> UpdateGroup(Guid groupId, string name);
    Task<Member> AddMember(Guid groupId, Guid memberId, GroupRole role);
    Task RemoveMember(Guid groupId, Guid memberId);
    Task<Member> ChangeMemberRole(Guid groupId, Guid memberId, GroupRole newRole);
    Task<IReadOnlyList<Member>> ListMembers(Guid groupId);
}
