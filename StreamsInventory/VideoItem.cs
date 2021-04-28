using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamsInventory
{
    public class VideoItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string playbackUrl { get; set; }
        public string duration { get; set; }
        public string creatorName { get; set; }
        public string creatorEmail { get; set; }
    }

    public class VideoItemCollection: List<VideoItem>
    {

    }
}
