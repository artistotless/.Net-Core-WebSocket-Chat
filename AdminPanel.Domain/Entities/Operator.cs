
namespace AdminPanel.Domain.Entities
{
    public class Operator
    {
        public int OperatorID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
    }
}
