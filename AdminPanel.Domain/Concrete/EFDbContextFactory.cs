using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Domain.Concrete
{
    public static class EFDbContextFactory
    {
        public static EFDbContext Create(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<EFDbContext>();
            builder.UseSqlServer(connectionString);
            return new EFDbContext(builder.Options);
        }
    }
}
