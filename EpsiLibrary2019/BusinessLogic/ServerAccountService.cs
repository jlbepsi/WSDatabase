using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EpsiLibrary2019.DataAccess;
using EpsiLibrary2019.Model;

namespace EpsiLibrary2019.BusinessLogic
{
    public class ServerAccountService
    {
        private ModelDatabase db = new ModelDatabase();

        /*public List<DatabaseServerUser> Get()
        {
            var list = db.DatabaseServerUsers.OrderBy(su => su.SqlLogin).ThenBy(su => su.ServerId);

            return list.ToList();
        }*/

        public List<DatabaseServerUser> GetAccounts(int serverId)
        {
            var list = db.DatabaseServerUsers.Where(su => su.ServerId == serverId);

            return list.ToList();
        }

        public DatabaseServerUser GetAccount(int serverId, string sqlLogin)
        {
            return db.DatabaseServerUsers.Find(serverId, sqlLogin);
        }

        public DatabaseServerUser AddAccount(ServerAccount serverAccount)
        {
            DatabaseServerUser databaseServerUser = new DatabaseServerUser
            {
                ServerId = serverAccount.ServerId,
                UserLogin = serverAccount.UserLogin
            };

            string serverName = "Serveur non trouvé";
            try
            {
                // Obtention du serveur
                DatabaseServerName databaseServerName = this.db.DatabaseServerNames.Find(serverAccount.ServerId);
                if (databaseServerName == null)
                    return null;

                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.ServerTypeId, databaseServerName.IPLocale);
                // Création du login SQL
                databaseServerUser.SqlLogin = management.MakeSqlLogin(serverAccount.UserLogin);
                // Ajout du login SQL sur le serveur
                management.AddOrUpdateUser(databaseServerUser.SqlLogin, serverAccount.Password);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Erreur dans l'ajout du compte utilisateur {0} sur le serveur '{1}'", serverAccount.ToString(), serverName), ex);
            }
             
            // Ajout de l'utilisateur pour cette BD
            try
            {
                db.DatabaseServerUsers.Add(databaseServerUser);
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DatabaseException(string.Format("Erreur dans l'ajout du compte utilisateur {0} dans le référentiel", serverAccount.ToString()), ex);
            }

            return databaseServerUser;
        }

        public bool UpdateAccount(ServerAccount serverAccount)
        {
            DatabaseServerUser databaseServerUser = null;
            DatabaseServerName databaseServerName = null;
            string serverName = "Serveur non trouvé";

            try
            {
                // Obtention du serveur
                databaseServerName = this.db.DatabaseServerNames.Find(serverAccount.ServerId);
                if (databaseServerName == null)
                    return false;
                serverName = databaseServerName.Name;

                // Obtention du login SQL de l'utilisateur
                databaseServerUser = this.db.DatabaseServerUsers.SingleOrDefault(su => su.ServerId == serverAccount.ServerId && su.UserLogin.Equals(serverAccount.UserLogin, StringComparison.InvariantCultureIgnoreCase));
                if (databaseServerUser == null)
                    return false;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(String.Format("Erreur dans l'obtention du compte utilisateur {0} dans le référentiel", serverAccount.ToString()), ex);
            }

            try
            {
                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.ServerTypeId, databaseServerName.IPLocale);
                // Modification du mot de passe du login SQL sur le serveur
                management.AddOrUpdateUser(databaseServerUser.SqlLogin, serverAccount.Password);
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format("Erreur dans la modification du compte utilisateur {0} sur le serveur '{1}'", serverAccount.ToString(), serverName));
            }

            return true;
        }

        public DatabaseServerUser RemoveAccount(int serverId, string userLogin)
        {
            DatabaseServerUser databaseServerUser = null;
            DatabaseServerName databaseServerName = null;
            string serverName = "Serveur non trouvé";

            try
            {
                // Obtention du serveur
                databaseServerName = this.db.DatabaseServerNames.Find(serverId);
                if (databaseServerName == null)
                    return null;
                serverName = databaseServerName.Name;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(String.Format("Erreur dans l'obtention du compte utilisateur {0} du serveur '{1}' dans le référentiel", userLogin, serverName), ex);
            }

            try
            {
                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.ServerTypeId, databaseServerName.IPLocale);
                // Modification du mot de passe du login SQL sur le serveur
                management.RemoveUser(userLogin);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(String.Format("Erreur dans la suppression du compte utilisateur {0} sur le serveur '{1}'", userLogin, serverName), ex);
            }

            try
            {
                databaseServerUser = this.db.DatabaseServerUsers.SingleOrDefault(su => su.ServerId == serverId && su.UserLogin.Equals(userLogin, StringComparison.InvariantCultureIgnoreCase));
                if (databaseServerUser == null)
                {
                    return null;
                }

                db.DatabaseServerUsers.Remove(databaseServerUser);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format("Erreur dans la suppression du compte utilisateur {0} sur le serveur '{1}' dans le référentiel", userLogin, serverName));
            }

            return databaseServerUser;
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }

        private bool Exists(int id)
        {
            return db.DatabaseDBs.Count(e => e.ServerId == id) > 0;
        }
    }
}
