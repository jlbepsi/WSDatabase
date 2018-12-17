using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EpsiLibrary2019.Model;

namespace EpsiLibrary2019.BusinessLogic
{
    public class DatabaseService
    {
        private ModelDatabase db = new ModelDatabase();


        public List<DatabaseDB> GetDatabases()
        {
            IQueryable<DatabaseDB> list = db.DatabaseDBs.OrderBy(db => db.ServerId).ThenBy(db => db.UserLogin).ThenBy(db => db.Nom);

            return list.ToList();
        }

        public List<DatabaseDB> GetDatabases(int serverId)
        {
            IQueryable<DatabaseDB> list = db.DatabaseDBs.Where(db => db.ServerId == serverId);
            list.OrderBy(db => db.UserLogin).ThenBy(db => db.Nom);

            return list.ToList();
        }

        public List<DatabaseDB> GetDatabases(string userLogin)
        {
            IQueryable<DatabaseDB> list = db.DatabaseDBs.Where(db => db.UserLogin.Equals(userLogin, StringComparison.InvariantCultureIgnoreCase));
            list.OrderBy(db => db.ServerId).ThenBy(db => db.Nom);

            return list.ToList();
        }

        public DatabaseDB GetDatabase(int id)
        {
            return db.DatabaseDBs.Find(id);
        }

        public bool AddDatabase(DatabaseDB databaseServerName)
        {

            db.DatabaseDBs.Add(databaseServerName);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(databaseServerName.ServerId))
                {
                    return false;
                }
                else
                {
                    throw new Exception("Erreur dans l'ajout de la base de données");
                }
            }

            return true;
        }

        public bool UpdateDatabase(DatabaseDB databaseServerName)
        {

            db.Entry(databaseServerName).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(databaseServerName.ServerId))
                {
                    return false;
                }
                else
                {
                    throw new Exception("Erreur dans la modification de la base de données");
                }
            }

            return true;
        }
        public DatabaseDB RemoveDatabase(int id)
        {
            DatabaseDB databaseServerName = db.DatabaseDBs.Find(id);
            if (databaseServerName == null)
            {
                return null;
            }

            db.DatabaseDBs.Remove(databaseServerName);
            db.SaveChanges();

            return databaseServerName;
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
