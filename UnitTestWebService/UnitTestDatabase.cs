using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using EpsiLibrary2019.BusinessLogic;
using EpsiLibrary2019.DataAccess;
using EpsiLibrary2019.Model;

namespace UnitTestWebService
{
    [TestClass]
    public class UnitTestDatabase
    {
        private static int serverId;

        private static DatabaseService service;

        [ClassInitialize]
        public static void InitialisationTests(TestContext context)
        {
            service = new DatabaseService();

            // serverId = DatabaseValues.DBMOCK_TYPE;
            serverId = 4; // SQL Server, @IP .161

            string projectDirectory = System.IO.Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "");
            EpsiLibrary2019.Utilitaires.ConfigurationManager.MAIN_CONFIG = System.IO.Path.Combine(projectDirectory, "epsiconfiguration.xml");

            /*// Ajout d'un compte pour les tests
            // Cela constitue aussi un test !
            ServerAccountModel serverAccount = new ServerAccountModel()
            {
                ServerId = UnitTestDatabase.serverId,
                UserLogin = "UnitTest",
                Password = "123ABC"
            };

            DatabaseServerUser databaseServerUser = service.AddAccount(serverAccount);
            Assert.IsNotNull(databaseServerUser);*/
        }

        [TestMethod]
        public void TestGetAllDatabases()
        {
            List<DatabaseDB> liste = service.GetDatabases();

            Assert.IsTrue(liste.Count > 0);
        }

        [TestMethod]
        public void TestAddDatabase()
        {
            DatabaseModel database = new DatabaseModel
            {
                ServerId = UnitTestDatabase.serverId,
                NomBD = "DBUbunitTest",
                UserLogin = "test.v8",
                Commentaire = "Depuis les Tests Unitaires"
            };
            DatabaseDB databaseDB = service.AddDatabase(database);

            Assert.IsNotNull(databaseDB);
            Assert.IsTrue(databaseDB.Id > 0);
            Assert.IsNotNull(databaseDB.DateCreation);

            Assert.IsTrue(databaseDB.DatabaseGroupUsers.Count == 1);
        }

        [TestMethod]
        public void TestUpdateDatabase()
        {
            DatabaseModel database = new DatabaseModel
            {
                Id = 1,
                Commentaire = "UpdateDatabase depuis les Tests Unitaires"
            };
            Assert.IsTrue(service.UpdateDatabase(1, database));
        }

        [TestMethod]
        public void TestRemoveDatabase()
        {
            //Assert.IsNotNull(service.RemoveDatabase(2));
        }

        [TestMethod]
        public void TestAddContributor()
        {
            GroupUserModel groupUserModel = new GroupUserModel
            {
                DbId = 11,
                SqlLogin = "ajoutTestUnit",
                Password = "123ABC",
                GroupType = 2
            };
            DatabaseGroupUser databaseGroupUser = service.AddContributor("test.v8", groupUserModel);

            Assert.IsNotNull(databaseGroupUser);
            Assert.IsNotNull(databaseGroupUser.SqlLogin);
            Assert.IsTrue(databaseGroupUser.AddedByUserLogin.Equals("test.v8"));
            Assert.IsNull(databaseGroupUser.UserLogin);
            Assert.IsTrue(databaseGroupUser.GroupType == groupUserModel.GroupType);
        }

        [TestMethod]
        public void TestUpdateContributor()
        {
            GroupUserModel groupUserModel = new GroupUserModel
            {
                DbId = 11,
                SqlLogin = "ajoutTestUnit",
                Password = "456ABC",
                GroupType = DatabaseValues.CRUD
            };
            Assert.IsTrue(service.UpdateContributor(groupUserModel));
        }
    }
}
