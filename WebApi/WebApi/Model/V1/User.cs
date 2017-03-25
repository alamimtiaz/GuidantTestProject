using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WebApi.Model.V1
{
    public class User
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public int Points { get; set; }
        public string Name { get; set; }
    }
}
