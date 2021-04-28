using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamsInventory
{
    public class InventoryResult
    {
        public bool Result { get; set; }
        public int ProcessCount { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public Exception Error { get; set; }
    }
}
