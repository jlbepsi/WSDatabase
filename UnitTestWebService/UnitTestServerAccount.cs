using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EpsiLibrary2019.BusinessLogic;
using EpsiLibrary2019.DataAccess;
using EpsiLibrary2019.Model;
/*
 * Aides HTML
 * 
 * https://www.automatetheplanet.com/mstest-cheat-sheet/
 * 
 * https://docs.microsoft.com/fr-fr/dotnet/core/testing/unit-testing-best-practices
 * https://docs.microsoft.com/fr-fr/visualstudio/test/using-stubs-to-isolate-parts-of-your-application-from-each-other-for-unit-testing?view=vs-2017
 * 
 */

namespace UnitTestWebService
{
    [TestClass]
    public class UnitTestServerAccount
    {
        private static ServerAccountService service;

        [ClassInitialize]
        public static void InitialisationTests(TestContext context)
        {
            service = new ServerAccountService();

            // Ajout d'un compte pour les tests
            // Cela constitue aussi un test !
            ServerAccountModel serverAccount = new ServerAccountModel()
            {
                ServerId = DatabaseValues.DBMOCK_TYPE,
                UserLogin = "UnitTest",
                Password = "123ABC"
            };

            DatabaseServerUser databaseServerUser = service.AddAccount(serverAccount);
            Assert.IsNotNull(databaseServerUser);
        }

        [TestMethod]
        public void TestGetAllAccounts()
        {
            List<DatabaseServerUser> liste = service.GetAccountsByServerId(0);

            Assert.IsTrue(liste.Count > 0);
        }

        [TestMethod]
        public void TestGetOneAccount()
        {
            string login = "test.v8";
            DatabaseServerUser databaseServerUser = service.GetAccountByServerLogin(0, login);

            Assert.IsNotNull(databaseServerUser);
            Assert.IsTrue(databaseServerUser.UserLogin.Equals(login, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsNotNull(databaseServerUser.SqlLogin);
        }

        [TestMethod]
        public void TestAddAccount()
        {
            DateTime dateDuJour = DateTime.Now;
            string login = string.Format("Test-{0}{1}{2}.{3}{4}", dateDuJour.Year, dateDuJour.Month, dateDuJour.Day, dateDuJour.Second, dateDuJour.Millisecond);
            ServerAccountModel serverAccount = new ServerAccountModel()
            {
                ServerId = DatabaseValues.DBMOCK_TYPE,
                UserLogin = login,
                Password = "123ABC"
            };

            DatabaseServerUser databaseServerUser = service.AddAccount(serverAccount);

            Assert.IsNotNull(databaseServerUser);
            Assert.IsFalse(string.IsNullOrWhiteSpace(databaseServerUser.SqlLogin));
        }

        [TestMethod]
        public void TestUpdateAccount()
        {
            ServerAccountModel serverAccount = new ServerAccountModel()
            {
                ServerId = DatabaseValues.DBMOCK_TYPE,
                UserLogin = "test.v8",
                Password = "123ABC"
            };

            Assert.IsTrue(service.UpdateAccount(serverAccount));
        }

        [TestMethod]
        public void TestRemoveAccount()
        {
            DatabaseServerUser databaseServerUser = service.RemoveAccount(DatabaseValues.DBMOCK_TYPE, "UnitTest");

            Assert.IsNotNull(databaseServerUser);
        }
    }
}
