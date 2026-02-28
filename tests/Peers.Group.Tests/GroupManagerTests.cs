using Peers.Group;

namespace Peers.Group.Tests;

public class GroupManagerTests
{
    [Fact]
    public async Task CreateGroup_ReturnsSuccess()
    {
        var sut = new global::Peers.Group.GroupManager();

        var result = await sut.CreateGroup("Alpha");

        Assert.Equal("Alpha", result.Name);
    }

    [Fact]
    public async Task CreateGroup_WithEmptyName_ThrowsValidationException()
    {
        var sut = new global::Peers.Group.GroupManager();

        await Assert.ThrowsAsync<GroupValidationException>(() => sut.CreateGroup(" "));
    }

    [Fact]
    public async Task GetGroup_WithUnknownId_ThrowsNotFoundException()
    {
        var sut = new global::Peers.Group.GroupManager();

        await Assert.ThrowsAsync<GroupNotFoundException>(() => sut.GetGroup(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateGroup_UpdatesName()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");

        var result = await sut.UpdateGroup(
            created.GroupId,
            "Beta");

        Assert.Equal("Beta", result.Name);
    }

    [Fact]
    public async Task AddMember_DuplicateMember_ThrowsConflictException()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");
        var groupId = created.GroupId;
        var memberId = Guid.NewGuid();

        var first = await sut.AddMember(groupId, memberId, GroupRole.Admin);

        Assert.Equal(memberId, first.MemberId);
        await Assert.ThrowsAsync<GroupConflictException>(
            () => sut.AddMember(groupId, memberId, GroupRole.Admin));
    }

    [Fact]
    public async Task RemoveMember_LastAdmin_ThrowsConflictException()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");
        var groupId = created.GroupId;
        var adminId = Guid.NewGuid();
        await sut.AddMember(groupId, adminId, GroupRole.Admin);

        await Assert.ThrowsAsync<GroupConflictException>(() => sut.RemoveMember(groupId, adminId));
    }

    [Fact]
    public async Task ChangeMemberRole_DemoteLastAdmin_ThrowsConflictException()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");
        var groupId = created.GroupId;
        var adminId = Guid.NewGuid();
        await sut.AddMember(groupId, adminId, GroupRole.Admin);

        await Assert.ThrowsAsync<GroupConflictException>(
            () => sut.ChangeMemberRole(groupId, adminId, GroupRole.Member));
    }

    [Fact]
    public async Task ListMembers_ReturnsMembersWithExpectedFields()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");
        var groupId = created.GroupId;
        var memberId = Guid.NewGuid();
        await sut.AddMember(groupId, memberId, GroupRole.Coach);

        var result = await sut.ListMembers(groupId);

        var member = Assert.Single(result);
        Assert.Equal(memberId, member.MemberId);
        Assert.Equal(memberId.ToString(), member.DisplayName);
        Assert.Equal(GroupRole.Coach, member.Role);
    }

}
