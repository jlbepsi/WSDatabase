﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EpsiLibrary2019.Model;
using EpsiLibrary2019.Utilitaires;

namespace EpsiLibrary2019.BusinessLogic
{
    public class ServerNameService
    {
        private ServiceEpsiContext db;

        public ServerNameService()
        {
            this.db = new ServiceEpsiContext();
        }
        public ServerNameService(ServiceEpsiContext db)
        {
            this.db = db;
        }


        public List<DatabaseServerName> Get()
        {
            return db.DatabaseServerNames.ToList();
        }

        public DatabaseServerName Get(int id)
        {
            return db.DatabaseServerNames.Find(id);
        }

        public bool Add(DatabaseServerName databaseServerName)
        {

            db.DatabaseServerNames.Add(databaseServerName);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!Exists(databaseServerName.Code))
                {
                    return false;
                }
                else
                {
                    LogManager.GetLogger().Error(ex);
                    throw new Exception("Erreur dans l'ajout du serveur");
                }
            }

            return true;
        }

        public bool Update(DatabaseServerName databaseServerName)
        {

            db.Entry(databaseServerName).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!Exists(databaseServerName.Code))
                {
                    return false;
                }
                else
                {
                    LogManager.GetLogger().Error(ex);
                    throw new Exception("Erreur dans la modification du serveur");
                }
            }

            return true;
        }
        public DatabaseServerName Remove(int id)
        {
            DatabaseServerName databaseServerName = db.DatabaseServerNames.Find(id);
            if (databaseServerName == null)
            {
                return null;
            }

            db.DatabaseServerNames.Remove(databaseServerName);
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

        private bool Exists(string code)
        {
            return db.DatabaseServerNames.Count(e => e.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase)) > 0;
        }

    }
}
