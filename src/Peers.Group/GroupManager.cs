namespace Peers.Group;

public sealed class GroupManager : IGroupManager
{
    private readonly Dictionary<Guid, GroupRecord> _groups = new();

    public Task<Group> CreateGroup(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new GroupValidationException("Group name must not be empty.");
        }

        var id = Guid.NewGuid();
        var group = new GroupRecord(id, name.Trim());
        _groups[id] = group;
        return Task.FromResult(ToGroup(group));
    }

    public Task<Group> GetGroup(Guid groupId)
    {
        if (!_groups.TryGetValue(groupId, out var group))
        {
            throw new GroupNotFoundException("Group was not found.");
        }

        return Task.FromResult(ToGroup(group));
    }

    public Task<Group> UpdateGroup(Guid groupId, string name)
    {
        if (!_groups.TryGetValue(groupId, out var group))
        {
            throw new GroupNotFoundException("Group was not found.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new GroupValidationException("Group name must not be empty.");
        }

        group.Name = name.Trim();
        return Task.FromResult(ToGroup(group));
    }

    public Task<Member> AddMember(Guid groupId, string name, GroupRole role)
    {
        if (!_groups.TryGetValue(groupId, out var group))
        {
            throw new GroupNotFoundException("Group was not found.");
        }

        if (string.IsNullOrWhiteSpace(name) || !Enum.IsDefined(role))
        {
            throw new GroupValidationException("Member input is invalid.");
        }

        var memberId = Guid.NewGuid();
        var membership = new MembershipRecord(groupId, memberId, name.Trim(), role);
        group.Members[memberId] = membership;
        return Task.FromResult(ToMember(membership));
    }

    public Task RemoveMember(Guid groupId, Guid memberId)
    {
        if (!_groups.TryGetValue(groupId, out var group))
        {
            throw new GroupNotFoundException("Group was not found.");
        }

        if (!group.Members.TryGetValue(memberId, out var membership))
        {
            throw new GroupNotFoundException("Member was not found in group.");
        }

        if (membership.Role == GroupRole.Admin && CountAdmins(group) == 1)
        {
            throw new GroupConflictException("Cannot remove last admin.");
        }

        group.Members.Remove(memberId);
        return Task.CompletedTask;
    }

    public Task<Member> ChangeMemberRole(Guid groupId, Guid memberId, GroupRole newRole)
    {
        if (!_groups.TryGetValue(groupId, out var group))
        {
            throw new GroupNotFoundException("Group was not found.");
        }

        if (!group.Members.TryGetValue(memberId, out var membership))
        {
            throw new GroupNotFoundException("Member was not found in group.");
        }

        if (!Enum.IsDefined(newRole))
        {
            throw new GroupValidationException("Role is invalid.");
        }

        if (membership.Role == GroupRole.Admin &&
            newRole != GroupRole.Admin &&
            CountAdmins(group) == 1)
        {
            throw new GroupConflictException("Cannot demote last admin.");
        }

        membership.Role = newRole;
        return Task.FromResult(ToMember(membership));
    }

    public Task<IReadOnlyList<Member>> ListMembers(Guid groupId)
    {
        if (!_groups.TryGetValue(groupId, out var group))
        {
            throw new GroupNotFoundException("Group was not found.");
        }

        var members = group.Members.Values
            .OrderBy(m => m.MemberId)
            .Select(ToMember)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<Member>>(members);
    }

    private static int CountAdmins(GroupRecord group) =>
        group.Members.Values.Count(m => m.Role == GroupRole.Admin);

    private static Group ToGroup(GroupRecord group) =>
        new(group.GroupId, group.Name);

    private static Member ToMember(MembershipRecord membership) =>
        new(membership.MemberId, membership.DisplayName, membership.Role);

    private sealed class GroupRecord(Guid groupId, string name)
    {
        public Guid GroupId { get; } = groupId;
        public string Name { get; set; } = name;
        public Dictionary<Guid, MembershipRecord> Members { get; } = new();
    }

    private sealed class MembershipRecord(Guid groupId, Guid memberId, string displayName, GroupRole role)
    {
        public Guid GroupId { get; } = groupId;
        public Guid MemberId { get; } = memberId;
        public string DisplayName { get; } = displayName;
        public GroupRole Role { get; set; } = role;
    }
}
