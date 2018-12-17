using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpsiLibrary2019.Utilitaires
{
    public interface ILogAppender
    {
        void Write(string level, object message);
    }
}
