using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder
{
    public class Source
    {
        public string registerName { get; set; }
        public int Number { get; set; }
        public List<Grade> Grades { get; set; }
        public bool isQualified { get; set; } // 因應binary搜尋時，回傳的是bool值，必須先判斷TF值後，再依據情況做篩選而設定的變數

    }
}
