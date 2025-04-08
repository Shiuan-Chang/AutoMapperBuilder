using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder
{
    public class Destination
    {
        public string studentName { get; set; }
        public int idNumber { get; set; }
        public List<Grade> Grades { get; set; }
    }
}
