using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;

using EpsiLibrary2019.Utilitaires;

namespace EpsiLibrary2019.DataAccess
{
    public class DatabaseManagementOracle : DatabaseManagement
    {
        #region Builders
        public DatabaseManagementOracle(string connectionString)
            : base(connectionString)
        {
            connection = new OracleConnection(connectionString);
        }
        #endregion


        private OracleConnection GetSqlConnection() { return this.connection as OracleConnection; }

        public override bool TestUserConnection(string databaseName, string sqlLogin, string password)
        {
            /* TODO : A Refaire Oracle management */
            string connectionString = string.Format("server={0};user={1};password={2};database={3};port=3306;",
                                                    this.connection.DataSource, sqlLogin, password, databaseName);

            bool result = true;
            OracleConnection connectionTest = new OracleConnection(connectionString);
            try
            {
                connectionTest.Open();
            }
            catch (OracleException)
            {
                result = false;
            }
            finally
            {
                connectionTest.Close();
            }

            return result;
        }

        #region User Management
        // Vérifie l'existence d'un utilisateur
        public override bool ExistsSqlLoginInDatabase(string sqlLogin, string databaseName)
        {
            try
            {
                Open();
                string storeProcedure = "ExistsUserInDB";
                OracleCommand cmd = new OracleCommand(storeProcedure, GetSqlConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("@dbName", databaseName));
                cmd.Parameters.Add(new OracleParameter("@userName", sqlLogin));
                OracleParameter exists = cmd.Parameters.Add("@userExists", OracleDbType.Int32);
                exists.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                return Convert.ToBoolean(exists.Value);
            }
            catch (OracleException ex)
            {
                LogManager.GetLogger().Error(ex);

                throw new DatabaseException(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        // Vérifie l'existence d'un utilisateur
        public override bool ExistsSqlLogin(string sqlLogin)
        {
            try
            {
                Open();
                string storeProcedure = "ExistsUser";
                OracleCommand cmd = new OracleCommand(storeProcedure, GetSqlConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("@userName", sqlLogin));
                OracleParameter exists = cmd.Parameters.Add("@userExists", OracleDbType.Int32);
                exists.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                return Convert.ToBoolean(exists.Value);
            }
            catch (OracleException ex)
            {
                LogManager.GetLogger().Error(ex);

                throw new DatabaseException(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        public override void AddOrUpdateUser(string sqlLogin, string password)
        {
            try
            {
                Open();
                string storeProcedure = "AddOrUpdateUser";
                OracleCommand cmd = new OracleCommand(storeProcedure, GetSqlConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("@userName", sqlLogin));
                cmd.Parameters.Add(new OracleParameter("@userPassword", password));

                cmd.ExecuteNonQuery();

                InternalExecuteNonQuery(String.Format("GRANT USAGE ON * . * TO '{0}'@'%' IDENTIFIED BY '{1}' WITH MAX_QUERIES_PER_HOUR 0 MAX_CONNECTIONS_PER_HOUR 0 MAX_UPDATES_PER_HOUR 0 MAX_USER_CONNECTIONS 0 ;", sqlLogin, password));
            }
            catch (OracleException ex)
            {
                LogManager.GetLogger().Error(ex);

                throw new DatabaseException(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        public override void RemoveUser(string sqlLogin)
        {
            string storeProcedure = "DropUser";
            OracleCommand cmd = new OracleCommand(storeProcedure, GetSqlConnection());
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.Add(new OracleParameter("@userName", sqlLogin));

                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                LogManager.GetLogger().Error(ex);

                throw new DatabaseException(ex.Message);
            }
            finally
            {
                Close();
            }
        }
        #endregion

        #region Database Service
        public override bool ExistsDatabase(string databaseName)
        {
            try
            {
                Open();
                OracleCommand cmd = new OracleCommand("ExistDB", GetSqlConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("@dbName", databaseName));
                OracleParameter exists = cmd.Parameters.Add("@dbExists", OracleDbType.Int32, 11);
                exists.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                return Convert.ToBoolean(exists.Value);
            }
            catch (OracleException ex)
            {
                LogManager.GetLogger().Error(ex);

                throw new DatabaseException(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        public override int GetDatabaseServerType() { return DatabaseValues.MYSQL_TYPE; }


        // Liste tous les base de données de l'utilisateur, format renvoyé
        public override List<string> ListDatabases(string sqlLogin)
        {
            List<string> liste = new List<string>();
            try
            {
                Open();

                string storeProcedure = "ListDatabases";
                OracleCommand cmd = new OracleCommand(storeProcedure, GetSqlConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("@userName", sqlLogin));
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        liste.Add(reader.GetString(0));
                    }
                }

            }
            catch (OracleException ex)
            {
                LogManager.GetLogger().Error(ex);

                throw new DatabaseException(ex.Message);
            }
            finally
            {
                Close();
            }
            return liste;
        }

        // Créé une base de données si elle n'existe pas
        public override void CreateDatabase(string databaseName, string sqlLogin)
        {
            try
            {
                Open();

                InternalExecuteNonQuery(String.Format("CREATE DATABASE IF NOT EXISTS {0} ;", databaseName));
                InternalExecuteNonQuery(String.Format("GRANT {0} ON {1}.* TO '{2}'@'%' ;", "ALL", databaseName, sqlLogin));
            }
            catch (OracleException ex)
            {
                LogManager.GetLogger().Error(ex);

                throw new DatabaseException(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        // Supprime une base de donnée (mais pas l'utilisateur associé)
        public override void RemoveDatabase(string databaseName)
        {
            try
            {
                Open();

                Open();
                OracleCommand cmd = new OracleCommand("DropDB", GetSqlConnection());
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("@dbName", databaseName));
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                LogManager.GetLogger().Error(ex);

                throw new DatabaseException(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        #endregion

        #region Contributeurs

        // Donne les droits rights à l'utilisateur sqlLogin sur la base de données databaseName
        protected override void AddOrUpdateContributor(string databaseName, string sqlLogin, int groupType, string password, bool doUpdate)
        {
            // L'utilisateur sqlLogin doit exister

            // Ajout des droits
            string mysqlRights = "";
            switch (groupType)
            {
                case EpsiLibrary2019.DataAccess.DatabaseValues.ADMINISTRATEUR: mysqlRights = "SELECT , UPDATE , INSERT , DELETE , CREATE , DROP , INDEX , ALTER , CREATE VIEW , SHOW VIEW , EXECUTE"; break;
                case EpsiLibrary2019.DataAccess.DatabaseValues.CRUD: mysqlRights = "SELECT, UPDATE, INSERT, DELETE, EXECUTE"; break;
                case EpsiLibrary2019.DataAccess.DatabaseValues.SELECT: mysqlRights = "SELECT"; break;
            }

            try
            {
                Open();
                if (doUpdate)
                {
                    InternalExecuteNonQuery(String.Format("REVOKE ALL PRIVILEGES ON {0}.* FROM '{1}'@'%';", databaseName, sqlLogin));
                }
                InternalExecuteNonQuery(String.Format("GRANT {0} ON {1}.* TO '{2}'@'%' ;", mysqlRights, databaseName, sqlLogin));


                if (!String.IsNullOrWhiteSpace(password))
                {
                    string storeProcedure = "UpdateUserPassword";
                    OracleCommand cmd = new OracleCommand(storeProcedure, GetSqlConnection());
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter("@userName", sqlLogin));
                    cmd.Parameters.Add(new OracleParameter("@userPassword", password));


                    OracleParameter userUpdated = cmd.Parameters.Add("@userUpdated", OracleDbType.Int32);
                    userUpdated.Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (OracleException ex)
            {
                LogManager.GetLogger().Error(ex);

                throw new DatabaseException(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        // Supprime les droits rights à l'utilisateur user sur la base de données databaseName
        public override void RemoveContributorFromDatabase(string databaseName, string sqlLogin)
        {
            try
            {
                Open();
                if (string.IsNullOrWhiteSpace(databaseName))
                    databaseName = "*";

                InternalExecuteNonQuery(String.Format("REVOKE ALL ON {0}.* FROM '{1}'@'%' ;", databaseName, sqlLogin));
            }
            catch { }
        }

        public override string MakeSqlLogin(string userLogin)
        {
            int indexPoint = userLogin.IndexOf('.');
            int indexTiret = userLogin.IndexOf('-');

            string sqlLogin = userLogin;
            // Suppression du -
            if (indexTiret > 0)
            {
                sqlLogin = userLogin.Substring(0, indexTiret) + userLogin.Substring(indexTiret + 1);
            }
            // Suppression du .
            if (indexPoint > 0)
            {
                sqlLogin = sqlLogin.Substring(0, indexPoint) + sqlLogin.Substring(indexPoint + 1);
            }

            return sqlLogin.ToUpper();
        }

        #endregion
    }
}
