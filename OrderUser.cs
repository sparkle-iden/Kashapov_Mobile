using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashapov
{
    public class OrderUser
    {
        public string UserName { get; set; }
        public int OrderID { get; set; }
        public decimal OrderCost { get; set; }
        public DateTime OrderDate { get; set; }
    }
}

