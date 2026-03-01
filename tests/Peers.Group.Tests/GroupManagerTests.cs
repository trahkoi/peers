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
    public async Task AddMember_ReturnsMemberWithExpectedFields()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");
        var groupId = created.GroupId;
        const string memberName = "Coach Carter";

        var result = await sut.AddMember(groupId, memberName, GroupRole.Coach);

        Assert.NotEqual(Guid.Empty, result.MemberId);
        Assert.Equal(memberName, result.DisplayName);
        Assert.Equal(GroupRole.Coach, result.Role);
    }

    [Fact]
    public async Task AddMember_WithEmptyName_ThrowsValidationException()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");
        var groupId = created.GroupId;

        await Assert.ThrowsAsync<GroupValidationException>(
            () => sut.AddMember(groupId, " ", GroupRole.Admin));
    }

    [Fact]
    public async Task RemoveMember_LastAdmin_ThrowsConflictException()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");
        var groupId = created.GroupId;
        var admin = await sut.AddMember(groupId, "Admin", GroupRole.Admin);

        await Assert.ThrowsAsync<GroupConflictException>(() => sut.RemoveMember(groupId, admin.MemberId));
    }

    [Fact]
    public async Task ChangeMemberRole_DemoteLastAdmin_ThrowsConflictException()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");
        var groupId = created.GroupId;
        var admin = await sut.AddMember(groupId, "Admin", GroupRole.Admin);

        await Assert.ThrowsAsync<GroupConflictException>(
            () => sut.ChangeMemberRole(groupId, admin.MemberId, GroupRole.Member));
    }

    [Fact]
    public async Task ListMembers_ReturnsMembersWithExpectedFields()
    {
        var sut = new global::Peers.Group.GroupManager();
        var created = await sut.CreateGroup("Alpha");
        var groupId = created.GroupId;
        var member = await sut.AddMember(groupId, "Coach Carter", GroupRole.Coach);

        var result = await sut.ListMembers(groupId);

        var listedMember = Assert.Single(result);
        Assert.Equal(member.MemberId, listedMember.MemberId);
        Assert.Equal("Coach Carter", listedMember.DisplayName);
        Assert.Equal(GroupRole.Coach, listedMember.Role);
    }

}
