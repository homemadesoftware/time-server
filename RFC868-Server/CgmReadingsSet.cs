using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFC868_Server
{
    public class CgmReadingsSet
    {
        public ReadingItem[]? items { get; set; }

        public class ReadingItem
        {
            public DateTime dateTime { get; set; }
            public float convertedReading { get; set; }
        }
    }
}
