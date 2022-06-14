using AdminPanel.Domain.Abstract;
using AdminPanel.Domain.Concrete;

using NUnit.Framework;
using System.Linq;

namespace AdminPanel.Tests
{
    class EFConnectionTests
    {
        private const string connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AdminPanel;Integrated Security=True";

        private EFDbContext GetDbContext()=> EFDbContextFactory.Create(connection);
        private IMessagesRepository GetMessagesRepository() => new MessagesRepository(GetDbContext());
        private IRolesRepository GetRolesRepository() => new RolesRepository(GetDbContext());

        [Test]
        public void Check_Connection()
        {
            EFDbContext context = GetDbContext();
            var list = context.Operators.ToList();
        }

        [Test]
        public void Fetch_From_Database()
        {
            EFDbContext context = GetDbContext();
            int count = context.Operators.Count();
            Assert.IsTrue(count > 0);
        }

        [Test]
        public void Include_Navigation_Property_1()
        {
            var msgRepository = GetMessagesRepository();
            var message = msgRepository.Messages.ToList()[0];
            Assert.IsTrue(message.Operator != null);
            Assert.IsTrue(message.Player != null);
        }

        [Test]
        public void Include_Navigation_Property_2()
        {
            var roleRepository = GetRolesRepository();
            var role1 = roleRepository.Roles.ToList()[0];
            var role2 = roleRepository.Roles.ToList()[1];
            Assert.IsTrue(role1.Permission != null);
            Assert.IsTrue(role2.Permission != null);
        }
    }
}
