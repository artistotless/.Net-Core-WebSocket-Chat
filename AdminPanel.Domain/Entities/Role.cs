
namespace AdminPanel.Domain.Entities
{
    public class Role
    {
        public int RoleID { get; set; }
        public string Title { get; set; }
        public Permission Permission { get; set; }
    }
}
