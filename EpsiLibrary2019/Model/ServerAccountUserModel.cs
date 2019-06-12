using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpsiLibrary2019.Model
{
    public class ServerAccounUsertModel
    {
        public string SqlLogin { get; set; }
        public string UserLogin { get; set; }

        public DatabaseServerName DatabaseServerName { get; set; }
    }
}
