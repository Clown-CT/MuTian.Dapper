using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuTian.Dapper.Test
{
    public class User
    {
        public User()
        {
            UserName =Guid.NewGuid().ToString("N");
            Age = Random.Shared.Next(1, 135);
        }
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
    }
}
