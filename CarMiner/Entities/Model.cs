using System;
using System.Collections.Generic;

namespace CarMiner.Entities
{
    public partial class Model
    {
        public Model()
        {
            Adds = new HashSet<Add>();
        }

        public int Idmodel { get; set; }
        public string Modelname { get; set; } = null!;
        public int Idbrand { get; set; }

        public virtual Brand IdbrandNavigation { get; set; } = null!;
        public virtual ICollection<Add> Adds { get; set; }
    }
}
