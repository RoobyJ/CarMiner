using System;
using System.Collections.Generic;

namespace CarMiner.Entities
{
    public partial class Brand
    {
        public Brand()
        {
            Adds = new HashSet<Add>();
            Models = new HashSet<Model>();
        }

        public int Idbrand { get; set; }
        public string Brandname { get; set; } = null!;

        public virtual ICollection<Add> Adds { get; set; }
        public virtual ICollection<Model> Models { get; set; }
    }
}
