using System;
using System.Collections.Generic;
using System.Text;

namespace GrassRootDataFetcher.Models
{
    public class Team
    {
        public int team_id { get; set; }
        public string name { get; set; }
        public string nickname { get; set; }
        public DateTime timestamp { get; set; }
        public int club_id { get; set; }
    }
}
