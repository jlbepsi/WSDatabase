using System;
using System.Collections.Generic;
using System.Text;


namespace EpsiLibrary2019.Utilitaires
{
    public enum Level { None, Debug, Info, Warning, Error, Fatal };

    public class Logger
    {
        private List<ILogAppender> logAppenders = new List<ILogAppender>();

        public bool WriteCurrentDate { get; set; }

        internal List<ILogAppender> Appenders
        {
            get { return logAppenders; }
        }

        internal Logger()
        {
        }

        private void WriteMessage(string level, object message)
        {
            foreach (ILogAppender appender in logAppenders)
            {
                appender.Write(level,string.Format("{0} - {1} : {2}", level, DateTime.Now.ToString("yyyyMMdd HH:mm"), message));
            }
        }
        private void WriteMessage(string level,  Exception exception)
        {
            WriteMessage(level, string.Format("\n\t{0}\n\t{1}", exception.Message, exception.StackTrace));
        }
        private void WriteMessage(string level, object message, Exception exception)
        {
            WriteMessage(level, string.Format("{0}\n\t{1}\n\t{2}", message, exception.Message, exception.StackTrace));
        }


        public void Debug(object message)
        {
            WriteMessage("DEBUG", message);
        }

        public void Info(object message)
        {
            WriteMessage("INFO", message);
        }

        public void Warn(object message)
        {
            WriteMessage("WARNING", message);
        }
        public void Warn(Exception exception)
        {
            WriteMessage("WARNING", exception);
        }
        public void Warn(object message, Exception exception)
        {
            WriteMessage("WARNING", message, exception);
        }

        public void Error(object message)
        {
            WriteMessage("ERROR", message);
        }
        public void Error(Exception exception)
        {
            WriteMessage("ERROR", exception);
        }
        public void Error(object message, Exception exception)
        {
            WriteMessage("ERROR", message, exception);
        }

        public void Fatal(object message)
        {
            WriteMessage("FATAL", message);
        }
        public void Fatal(Exception exception)
        {
            WriteMessage("FATAL", exception);
        }
        public void Fatal(object message, Exception exception)
        {
            WriteMessage("FATAL", message, exception);
        }

        public bool IsEnabledFor(Level level)
        {
            return true;
        }

        public List<string> EnumAppenders()
        {
            return new List<string>(new string[] {"Not Yet Implemented"});
        }
        public List<string> EnumAppenders(Level level)
        {
            return new List<string>(new string[] { "Not Yet Implemented" });
        }
    }
}
