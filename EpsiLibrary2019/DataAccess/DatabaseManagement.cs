﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpsiLibrary2019.DataAccess
{
    public abstract class DatabaseManagement
    {
        #region Attributes
        // Connection à la BD
        private string connectionString;
        protected DbConnection connection;
        #endregion
        
        #region Builders

        public DatabaseManagement(string connectionString)
        {
            this.connectionString = connectionString;
        }
        #endregion

        #region Connection
        public void Open()
        {
            if (connection != null && connection.State == ConnectionState.Closed)
                connection.Open();
        }
        public void Close()
        {
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }
        #endregion

        #region Base de données

        public abstract int GetDatabaseServerType();

        public abstract bool TestUserConnection(string databaseName, string sqlLogin, string password);
        public abstract bool ExistsSqlLoginInDatabase(string sqlLogin, string databaseName);
        public virtual bool IsCorrectName(string databaseName)
        {
            return EpsiLibrary2019.Utilitaires.RegularExpression.IsCorrectFileName(databaseName);
        }
        public abstract bool ExistsDatabase(string databaseName);

        public abstract void CreateDatabase(string databaseName, string sqlLogin);
        public abstract void RemoveDatabase(string databaseName);
        public abstract List<string> ListDatabases(string sqlLogin);

        #endregion

        #region Utilisateurs
        public abstract string MakeSqlLogin(string userLogin);
        public abstract bool ExistsSqlLogin(string sqlLogin);
        public abstract void AddOrUpdateUser(string sqlLogin, string password);
        public abstract void RemoveUser(string sqlLogin);

        public void AddContributor(string databaseName, string sqlLogin,int groupType,  string password)
        {
            AddOrUpdateContributor(databaseName, sqlLogin, groupType, password, false);
        }
        public void UpdateContributor(string databaseName, string sqlLogin, int groupType, string password)
        {
            AddOrUpdateContributor(databaseName, sqlLogin, groupType, password, true);
        }

        protected abstract void AddOrUpdateContributor(string databaseName, string sqlLogin, int groupType, string password, bool doUpdate);
        public abstract void RemoveContributorFromDatabase(string databaseName, string sqlLogin);

        #endregion


        /** TODO : A compléter */
        public static DatabaseManagement CreateDatabaseManagement(int serverId, string adresseIP)
        {
            EpsiLibrary2019.Utilitaires.ConfigurationManager config = EpsiLibrary2019.Utilitaires.ConfigurationManager.GetConfigurationManager();
            if (config.GetValue("database.mock") == "1" || serverId == DatabaseValues.DBMOCK_TYPE)
            {
                return new MockDatabaseManagement();
            }

            if (serverId == DatabaseValues.MYSQL_TYPE)
            {
                // On fixe l'adresse IP du serveur
                string connectionString = string.Format(config.GetValue("database.server.mysql.connectionstring"), adresseIP);
                return new DatabaseManagementMySQL(connectionString);
            }
            else if (serverId == DatabaseValues.SQLSERVER_TYPE)
            {
                string connectionString = string.Format(config.GetValue("database.server.sqlserver.connectionstring"), adresseIP);
                return new DatabaseManagementSQLServer(connectionString);
            }
            else if (serverId == DatabaseValues.ORACLE_TYPE)
            {
                string connectionString = string.Format(config.GetValue("database.server.oracle.connectionstring"), adresseIP);
                return new DatabaseManagementOracle(connectionString);
            }

            return null;
        }
    }
}
