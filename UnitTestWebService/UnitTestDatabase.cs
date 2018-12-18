using System;
using EpsiLibrary2019.BusinessLogic;
using EpsiLibrary2019.DataAccess;
using EpsiLibrary2019.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestWebService
{
    [TestClass]
    public class UnitTestDatabase
    {
        private static ServerAccountService service = new ServerAccountService();

        [ClassInitialize]
        public static void InitialisationTests(TestContext context)
        {
            service = new ServerAccountService();
        }

        [TestMethod]
        public void TestAddAccount()
        {
            ServerAccount serverAccount = new ServerAccount()
            {
                ServerId = DatabaseValues.DBMOCK_TYPE,
                UserLogin = "UnitTest",
                Password = "123ABC"
            };

            DatabaseServerUser databaseServerUser = service.AddAccount(serverAccount);

            Assert.IsNotNull(databaseServerUser);
            Assert.IsFalse(string.IsNullOrWhiteSpace(databaseServerUser.SqlLogin));
        }

        [TestMethod]
        public void TestRemoveAccount()
        {
            DatabaseServerUser databaseServerUser = service.RemoveAccount(DatabaseValues.DBMOCK_TYPE, "UnitTest");

            Assert.IsNotNull(databaseServerUser);
        }
    }
}
