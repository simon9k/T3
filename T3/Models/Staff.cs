using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace T3.Models
{
    public class Staff:Person
    {
        //for staff like coaches/Teachers,managers
        public string JobTitle { set; get; }
        
    }
}
