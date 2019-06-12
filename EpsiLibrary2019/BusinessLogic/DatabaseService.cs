using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpsiLibrary2019.DataAccess;
using EpsiLibrary2019.Model;
using EpsiLibrary2019.Utilitaires;

namespace EpsiLibrary2019.BusinessLogic
{
    public class DatabaseService : BaseService
    {
        public DatabaseService()
            : base()
        {
        }

        public DatabaseService(ServiceEpsiContext contexte)
            : base(contexte)
        {
        }

        #region Gestion des bases de données
        public List<DatabaseDB> GetDatabases()
        {
            IQueryable<DatabaseDB> list = db.DatabaseDBs.OrderBy(db => db.ServerId);
            return list.ToList();
        }

        public List<DatabaseDB> GetDatabasesByServerId(int serverId)
        {
            IQueryable<DatabaseDB> list = db.DatabaseDBs.Where(db => db.ServerId == serverId);
            return list.ToList();
        }

        public List<DatabaseDB> GetDatabasesByServerCode(string serverCode)
        {
            var list = db.DatabaseDBs.Where(db => db.DatabaseServerName.Code.Equals(serverCode, StringComparison.InvariantCultureIgnoreCase));
            return list.ToList();
        }

        public List<DatabaseDB> GetDatabasesByLogin(string userLogin)
        {
            var list = 
                from dbs in db.DatabaseDBs
                join dgu in db.DatabaseGroupUsers
                on dbs.Id equals dgu.DbId
                where dgu.UserLogin.Equals(userLogin, StringComparison.InvariantCultureIgnoreCase)
                select dbs;
            return list.ToList();
        }

        public DatabaseDB GetDatabase(int id)
        {
            return db.DatabaseDBs.Find(id);
        }

        public DatabaseDB GetDatabase(int serverId, string nomBD)
        {
            try
            {
                DatabaseDB databaseDB = db.DatabaseDBs.SingleOrDefault(db => db.ServerId == serverId && db.NomBD.Equals(nomBD, StringComparison.InvariantCultureIgnoreCase));
                return databaseDB;
            }
            catch(Exception)
            {

            }

            return null;
        }

        public DatabaseDB AddDatabase(DatabaseModel database)
        {
            // Vérification du nom
            if (! RegularExpression.IsCorrectFileName(database.NomBD))
                throw new DatabaseException("Le nom ne doit pas comporter des caractères spéciaux.");

            // Le nom de la base de données doit être unique sur un serveur donné
            if (GetDatabase(database.ServerId, database.NomBD) != null)
                throw new DatabaseException("Le nom de la base de données existe déjà.");

            string serverName = "Serveur non trouvé";
            DatabaseServerUser databaseServerUser = null;
            try
            {
                // Obtention du serveur
                DatabaseServerName databaseServerName = this.db.DatabaseServerNames.Find(database.ServerId);
                if (databaseServerName == null)
                    return null;

                // Obtention du compte utilisateur du serveur
                ServerAccountService serverAccountService = new ServerAccountService(this.ServiceEpsiContext);
                databaseServerUser = serverAccountService.GetAccountByServerLogin(database.ServerId, database.UserLogin);
                if (databaseServerUser == null)
                    return null;

                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.Code, databaseServerName.IPLocale);
                if (management == null)
                    return null;

                // et céation de la base de données sur le serveur de BD
                management.CreateDatabase(database.NomBD, databaseServerUser.SqlLogin);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Erreur dans l'ajout de la base de données {0} sur le serveur '{1}'", database.ToString(), serverName), ex);
            }

            // Ajout de la base de données dans le référentiel
            DatabaseDB databaseDB = new DatabaseDB
            {
                ServerId = database.ServerId,
                NomBD = database.NomBD,
                DateCreation = DateTime.Now,
                Commentaire = database.Commentaire
            };
            db.DatabaseDBs.Add(databaseDB);

            try
            {
                db.SaveChanges();

                // puis du créateur comme contributeur avec tous les droits
                DatabaseGroupUser databaseGroupUser = new DatabaseGroupUser
                {
                    DbId = databaseDB.Id,
                    UserLogin = databaseServerUser.UserLogin,
                    SqlLogin = databaseServerUser.SqlLogin,
                    GroupType = DatabaseValues.ADMINISTRATEUR,
                    AddedByUserLogin = databaseServerUser.UserLogin
                };
                db.DatabaseGroupUsers.Add(databaseGroupUser);
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!Exists(databaseDB.ServerId))
                {
                    return null;
                }
                else
                {
                    throw new DatabaseException(string.Format("Erreur dans l'ajout de la base de données dans le référentiel", database.ToString()), ex);
                }
            }

            // Enregistrement dans les logs
            WriteLogs("BDD Création - " + string.Format("L'utilisateur '<b>{0}</b>' a créé la bdd '{1}' de type '{2}'", database.UserLogin, database.NomBD, database.ServerId));
            return databaseDB;
        }

        public bool UpdateDatabase(int id, DatabaseModel database)
        {
            DatabaseDB databaseDB = GetDatabase(database.Id);
            if (databaseDB == null)
                return false;

            // Modification des données
            databaseDB.Commentaire = database.Commentaire;

            db.Entry(databaseDB).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception(string.Format("Erreur dans la modification de la base de données '{0}' dans le référentiel", database.ToString()));
            }

            return true;
        }

        /*public bool RemoveDatabase(DatabaseDB database)
        {
            if (database == null)
            {
                return false;
            }
            // L'objet passé en paramètre est de source inconnue, donc on recharge la base de données
            DatabaseDB databaseDB = GetDatabase(database.Id);
            if (databaseDB == null)
                return false;

            return InternalRemoveDatabase(databaseDB);
        }*/

        public DatabaseDB RemoveDatabase(int id)
        {
            DatabaseDB databaseDB = GetDatabase(id);
            if (databaseDB == null)
                return null;

            if (InternalRemoveDatabase(databaseDB))
                return databaseDB;

            return null;
        }
        #endregion

        /*************************************************************************************************************************************/

        #region Gestion des contrbuteurs

        public DatabaseGroupUser GetDatabaseGroupUser(string userLogin, int id)
        {
            try
            {
                return db.DatabaseGroupUsers.SingleOrDefault(gu => gu.DbId == id && gu.UserLogin.Equals(userLogin, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception)
            { }

            return null;
        }

        public DatabaseGroupUser GetDatabaseGroupUserWithSqlLogin(string sqlLogin, int id)
        {
            try
            {
                return db.DatabaseGroupUsers.SingleOrDefault(gu => gu.DbId == id && gu.SqlLogin.Equals(sqlLogin, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception)
            { }

            return null;
        }

        public List<DatabaseGroupUser> GetDatabaseGroupUserWithSqlLogin(string sqlLogin)
        {
            IQueryable<DatabaseGroupUser> list = db.DatabaseGroupUsers.Where(gu => gu.SqlLogin.Equals(sqlLogin, StringComparison.InvariantCultureIgnoreCase));
            return list.ToList();
        }

        public bool IsAdministrateur(string userLogin, int id)
        {
            try
            {
                DatabaseGroupUser databaseGroupUser = GetDatabaseGroupUser(userLogin, id);
                if (databaseGroupUser != null)
                    return databaseGroupUser.GroupType == DatabaseValues.ADMINISTRATEUR;
            }
            catch (Exception)
            { }

            return false;
        }

        public bool IsAdministrateur(string userLogin, List<DatabaseGroupUser> groupUsers)
        {
            foreach (DatabaseGroupUser groupUser in groupUsers)
            {
                if (userLogin.Equals(groupUser.UserLogin, StringComparison.InvariantCultureIgnoreCase) &&
                    groupUser.GroupType == DatabaseValues.ADMINISTRATEUR)
                {
                    return true;
                }
            }

            return false;
        }

        public DatabaseGroupUser AddContributor(string userLogin, GroupUserModel groupUserModel)
        {
            string serverName = null;
            try
            {
                // Obtention de la base de données
                DatabaseDB databaseDB = GetDatabase(groupUserModel.DbId);
                if (databaseDB == null)
                    return null;

                // Obtention du serveur
                DatabaseServerName databaseServerName = this.db.DatabaseServerNames.Find(databaseDB.ServerId);
                if (databaseServerName == null)
                    return null;
                serverName = databaseServerName.Name;

                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.Code, databaseServerName.IPLocale);
                if (management == null)
                    return null;

                management.AddContributor(databaseDB.NomBD, groupUserModel.SqlLogin, groupUserModel.GroupType, groupUserModel.Password);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Erreur dans l'ajout du contributeur {0} sur le serveur '{1}'", groupUserModel.ToString(), serverName), ex);
            }

            DatabaseGroupUser contributor = new DatabaseGroupUser
            {
                DbId = groupUserModel.DbId,
                SqlLogin = groupUserModel.SqlLogin,
                AddedByUserLogin = userLogin,
                GroupType = groupUserModel.GroupType

            };

            try
            {
                // Ajout du contributeur dans le groupe
                this.db.DatabaseGroupUsers.Add(contributor);
                this.db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DatabaseException(string.Format("Erreur dans l'ajout du contributeur {0} dans le référentiel", groupUserModel.ToString()), ex);
            }

            // Envoi du mail

            /*
            if (userLoginSQL != null)
                contributor.UserLoginSQL = userLoginSQL;


                if (userEpsi != null)
                {
                    contributor.UserEpsiFullName = userEpsi.Nom + " " + userEpsi.Prenom;

                    // Envoi du mail
                    if (!string.IsNullOrWhiteSpace(userEpsi.Mail))
                    {
                        StringBuilder body = new StringBuilder();
                        body.AppendFormat("Bonjour, </b><br /><br />Vous avez été ajouté comme contributeur dans la base de données '{0}' par l'utilisateur '{1}'", infos.Name, infos.Createur);
                        if (accountAdded)
                        {
                            body.AppendFormat("<br /><br />Un compte a été créé sur le serveur '{0}' avec un mot de passe aléatoire.<br />", infos.DatabaseServerName.Name);
                            body.Append("Vous devez vous connecter à <a href='https://ingenium.montpellier.epsi.fr/'>Ingénium</a> pour modifier le mot de passe de ce compte.");
                        }
                        body.Append("Connectez-vous à <a href='https://ingenium.montpellier.epsi.fr/'>Ingénium</a> pour voir vos base de données.");
                        body.Append(@"</b><br /><br />L'administrateur réseau<br />EPSI Montpellier");

                        SendMail(userEpsi.Mail, "EPSI - Base de données - Ajout de contributeur", body.ToString());
                    }
                }
            }*/

            return contributor;
            /*
            {
                Message = message,
                Status = status,
                UserEpsiFullName = contributor.UserEpsiFullName,
                UserId = userId,
                LoginSQL = contributor.UserLoginSQL,
                GroupType = groupType,
                GroupTypeDescription = DatabaseService.GetGroupDescription(groupType)
            });*/
        }


        public bool UpdateContributor(GroupUserModel groupUserModel)
        {
            DatabaseGroupUser databaseGroupUser = GetDatabaseGroupUserWithSqlLogin(groupUserModel.SqlLogin, groupUserModel.DbId);
            if (databaseGroupUser == null)
                return false;

            // Modification du mot de passe sur le serveur
            string serverName = null;
            try
            {
                // Obtention de la base de données
                DatabaseDB databaseDB = GetDatabase(groupUserModel.DbId);
                if (databaseDB == null)
                    return false;

                // Obtention du serveur
                DatabaseServerName databaseServerName = this.db.DatabaseServerNames.Find(databaseDB.ServerId);
                if (databaseServerName == null)
                    return false;
                serverName = databaseServerName.Name;

                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.Code, databaseServerName.IPLocale);
                if (management == null)
                    return false;

                management.UpdateContributor(databaseDB.NomBD, groupUserModel.SqlLogin, groupUserModel.GroupType, groupUserModel.Password);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Erreur dans l'ajout du contributeur {0} sur le serveur '{1}'", groupUserModel.ToString(), serverName), ex);
            }

            return true;
        }


        public DatabaseGroupUser RemoveContributor(string userLogin, string sqlLogin, int databaseId)
        {
            DatabaseGroupUser databaseGroupUser = GetDatabaseGroupUserWithSqlLogin(sqlLogin, databaseId);
            if (databaseGroupUser == null)
                return null;

            if (InternalRemoveContributor(databaseGroupUser))
                return databaseGroupUser;

            return null;
        }

        public static string GetGroupDescription(int groupType)
        {
            switch (groupType)
            {
                case DatabaseValues.ADMINISTRATEUR: return "Tous les droits";
                case DatabaseValues.CRUD: return "CRUD enregistrements et tables";
                case DatabaseValues.SELECT:
                default: return "SELECT uniquement";
            }
        }
        #endregion


        /*************************************************************************************************************************************/


        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }

        private bool InternalRemoveDatabase(DatabaseDB database)
        {
            string serverName = "Serveur non trouvé";
            try
            {
                // Obtention du serveur
                DatabaseServerName databaseServerName = this.db.DatabaseServerNames.Find(database.ServerId);
                if (databaseServerName == null)
                    return false;

                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.Code, databaseServerName.IPLocale);
                if (management == null)
                    return false;

                // et suppression de la base de données sur le serveur de BD
                management.RemoveDatabase(database.NomBD);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Erreur dans la suppression de la base de données {0} sur le serveur '{1}'", database.ToString(), serverName), ex);
            }

            // Suppression des contributeurs associés
            db.DatabaseGroupUsers.RemoveRange(database.DatabaseGroupUsers);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception(string.Format("Erreur dans la suppression des contributeurs de la base de données '{0}' dans le référentiel", database.ToString()));
            }

            // Suppression de la base de données
            db.DatabaseDBs.Remove(database);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception(string.Format("Erreur dans la suppression de la base de données '{0}' dans le référentiel", database.ToString()));
            }

            return true;
        }

        private bool InternalRemoveContributor(DatabaseGroupUser databaseGroupUser)
        {
            string serverName = "Serveur non trouvé";
            try
            {
                // Obtention du serveur
                DatabaseServerName databaseServerName = this.db.DatabaseServerNames.Find(databaseGroupUser.DatabaseDB.ServerId);
                if (databaseServerName == null)
                    return false;

                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.Code, databaseServerName.IPLocale);
                if (management == null)
                    return false;

                // et suppression de la base de données sur le serveur de BD
                management.RemoveContributorFromDatabase(databaseGroupUser.DatabaseDB.NomBD, databaseGroupUser.SqlLogin);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Erreur dans la suppression du contributeur '{0}' de la base de données' {1}' sur le serveur '{2}'", databaseGroupUser.SqlLogin, databaseGroupUser.DatabaseDB.NomBD, serverName), ex);
            }

            // Suppression du contributeur
            db.DatabaseGroupUsers.Remove(databaseGroupUser);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception(string.Format("Erreur dans la suppression du contributeur '{0}' de la base de données '{0}' dans le référentiel", databaseGroupUser.SqlLogin, databaseGroupUser.DatabaseDB.NomBD));
            }

            return true;
        }

        private bool Exists(int id)
        {
            return db.DatabaseDBs.Count(e => e.ServerId == id) > 0;
        }

    }
}
