using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EpsiLibrary2019.Model;

namespace EpsiLibrary2019.Utilitaires
{
    public class DatabaseGroupUserPermissions
    {
        public static int ADMINISTRATEUR = 1;
        public static int MODIFICATION = 2;
        public static int ECRITURE = 3;
        public static int LECTURE = 4;

        public static int GetGroupType(ICollection<DatabaseGroupUser> databaseGroupUsers, string login)
        {
            foreach(DatabaseGroupUser user in databaseGroupUsers)
            {
                if (! String.IsNullOrWhiteSpace(user.UserLogin) && user.UserLogin.Equals(login, StringComparison.InvariantCultureIgnoreCase))
                    return user.GroupType;
            }

            return -1;
        }
    }
}
