using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADsFusion
{
    internal class MergedUser
    {
        public List<User> Users { get; set; }

        public MergedUser()
        {

        }

        public MergedUser(List<User> users)
        {
            Users = users;
        }
    }
}
