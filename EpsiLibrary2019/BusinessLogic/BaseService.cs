using System;

using EpsiLibrary2019.Model;
using EpsiLibrary2019.Utilitaires;
using EpsiLibrary2019.Utilitaires.GoogleMail;

namespace EpsiLibrary2019.BusinessLogic
{
    public abstract class BaseService
    {
        protected DatabaseContext db = new DatabaseContext();

        public DatabaseContext DatabaseContext
        {
            get { return db; }
        }

        public BaseService()
        {
            this.db = new DatabaseContext();
        }

        public BaseService(DatabaseContext model)
        {
            this.db = model;
        }

        protected void WriteLogs(string message)
        {
            if (ConfigurationManager.GetConfigurationManager().GetValue("logs.useLogs").Equals("true"))
                LogManager.GetLogger().Info(message);
        }

        protected void SendMail(string to, string subject, string body)
        {
            try
            {
                SmtpGMail client = new SmtpGMail(ConfigurationManager.GetConfigurationManager().GetValue("gmail.user.login"),
                                                 ConfigurationManager.GetConfigurationManager().GetValue("gmail.user.password"));
                client.SendMessage(to, subject, body);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(ex);
            }
        }
    }
}
