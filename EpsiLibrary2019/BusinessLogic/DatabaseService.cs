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

        public DatabaseService(DatabaseContexte contexte)
            : base(contexte)
        {
        }

        #region Gestion des bases de données
        public List<DatabaseDB> GetDatabases()
        {
            IQueryable<DatabaseDB> list = db.DatabaseDBs.OrderBy(db => db.ServerId).ThenBy(db => db.UserLogin).ThenBy(db => db.Nom);

            return list.ToList();
        }

        public List<DatabaseDB> GetDatabasesByServerId(int serverId)
        {
            IQueryable<DatabaseDB> list = db.DatabaseDBs.Where(db => db.ServerId == serverId);
            list.OrderBy(db => db.UserLogin).ThenBy(db => db.Nom);

            return list.ToList();
        }

        public List<DatabaseDB> GetDatabasesByServerType(int serverType)
        {
            IQueryable<DatabaseDB> list = db.DatabaseDBs.Where(db => db.DatabaseServerName.ServerTypeId == serverType);
            list.OrderBy(db => db.UserLogin).ThenBy(db => db.Nom);

            return list.ToList();
        }

        public List<DatabaseDB> GetDatabasesByLogin(string userLogin)
        {
            IQueryable<DatabaseDB> list = db.DatabaseDBs.Where(db => db.UserLogin.Equals(userLogin, StringComparison.InvariantCultureIgnoreCase));
            list.OrderBy(db => db.ServerId).ThenBy(db => db.Nom);

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
                DatabaseDB databaseDB = db.DatabaseDBs.SingleOrDefault(db => db.ServerId == serverId && db.Nom.Equals(nomBD, StringComparison.InvariantCultureIgnoreCase));
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
            try
            {
                // Obtention du serveur
                DatabaseServerName databaseServerName = this.db.DatabaseServerNames.Find(database.ServerId);
                if (databaseServerName == null)
                    return null;

                // Obtention du compte utilisateur du serveur
                ServerAccountService serverAccountService = new ServerAccountService(this.DatabaseContext);
                DatabaseServerUser databaseServerUser = serverAccountService.GetAccountByServerLogin(database.ServerId, database.UserLogin);
                if (databaseServerUser == null)
                    return null;

                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.ServerTypeId, databaseServerName.IPLocale);
                if (management == null)
                    return null;

                // et céation de la base de données sur le serveur de BD
                management.CreateDatabase(database.NomBD, databaseServerUser.SqlLogin);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Erreur dans l'ajout de la base de données {0} sur le serveur '{1}'", database.ToString(), serverName), ex);
            }

            DatabaseDB databaseDB = new DatabaseDB
            {
                ServerId = database.ServerId,
                NomBD = database.NomBD,
                UserLogin = database.UserLogin,
                Nom = database.UserNom,
                Prenom = database.UserPrenom,
                DateCreation = DateTime.Now,
                Commentaire = database.Commentaire
            };

            // puis dans le référentiel
            db.DatabaseDBs.Add(databaseDB);

            try
            {
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

        public bool UpdateDatabase(DatabaseDB database)
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

        public bool RemoveDatabase(DatabaseDB database)
        {
            if (database == null)
            {
                return false;
            }
            // L'objet passé en paramètre est de source inconnue, donc on le recharche dans la base de données
            DatabaseDB databaseDB = GetDatabase(database.Id);
            if (databaseDB == null)
                return false;

            return InternalRemoveDatabase(databaseDB);
        }

        public DatabaseDB RemoveDatabase(int id)
        {
            DatabaseDB database = GetDatabase(id);
            if (InternalRemoveDatabase(database))
                return database;

            return null;
        }
        #endregion

        /*************************************************************************************************************************************/

        #region Gestion des contrbuteurs
        public DatabaseGroupUser AddContributor(JWTAuthenticationIdentity user, GroupUserModel groupUserModel)
        {
            DatabaseGroupUser contributor = null;
            string serverName = null;
            try
            {
                // Obtention de la base de données
                DatabaseDB databaseDB = GetDatabase(groupUserModel.DbId);
                if (databaseDB == null)
                    return null;

                // L'utilisateur doit êpre propriétaire de la base de données
                if (! user.Name.Equals(databaseDB.UserLogin, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                // Obtention du compte utilisateur du serveur
                ServerAccountService serverAccountService = new ServerAccountService(this.DatabaseContext);
                DatabaseServerUser databaseServerUser = serverAccountService.GetAccountByServerLogin(databaseDB.ServerId, databaseDB.UserLogin);
                if (databaseServerUser == null)
                    return null;

                // Obtention du serveur
                DatabaseServerName databaseServerName = this.db.DatabaseServerNames.Find(databaseDB.ServerId);
                if (databaseServerName == null)
                    return null;
                serverName = databaseServerName.Name;

                // Obtention du serveur réel : MySQL, SQL Server, ... avec son adresse IP
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.ServerTypeId, databaseServerName.IPLocale);
                if (management == null)
                    return null;

                management.AddContributor(databaseDB.NomBD, groupUserModel.SqlLogin, groupUserModel.GroupType, groupUserModel.Password);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Erreur dans l'ajout du contributeur {0} sur le serveur '{1}'", groupUserModel.ToString(), serverName), ex);
            }

            try
            {
                // Ajout du contributeur dans le groupe
                contributor.DbId = groupUserModel.DbId;
                contributor.SqlLogin = groupUserModel.SqlLogin;
                /*contributor.UserId = userId;
                contributor.UserEpsiFullName = userEpsiFullName;*/
                contributor.GroupType = groupUserModel.GroupType;

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
                DatabaseManagement management = DatabaseManagement.CreateDatabaseManagement(databaseServerName.ServerTypeId, databaseServerName.IPLocale);
                if (management == null)
                    return false;

                // et suppression de la base de données sur le serveur de BD
                management.RemoveDatabase(database.NomBD);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Erreur dans l'ajout de la base de données {0} sur le serveur '{1}'", database.ToString(), serverName), ex);
            }

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

        private bool Exists(int id)
        {
            return db.DatabaseDBs.Count(e => e.ServerId == id) > 0;
        }

    }
}
