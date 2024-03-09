using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuTian.Dapper
{
    public class ConnectionSettings
    {
        public DatabaseType DatabaseType { get; set; }
        public required string ConnectionString { get; set; }
    }
}
