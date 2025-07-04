using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication4.Models
{
    public class StudentRequest
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CollegeYear { get; set; }
    }
}
