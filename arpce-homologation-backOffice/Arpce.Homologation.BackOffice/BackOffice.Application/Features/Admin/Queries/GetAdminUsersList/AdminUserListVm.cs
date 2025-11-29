namespace BackOffice.Application.Features.Admin.Queries.GetAdminUsersList;

public class AdminUserListVm
{
    public int Page { get; set; }
    public int PageTaille { get; set; }
    public int TotalPage { get; set; }
    public List<AdminUserListItemDto> Utilisateurs { get; set; } = new();
}