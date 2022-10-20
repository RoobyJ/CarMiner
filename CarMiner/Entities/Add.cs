using System;
using System.Collections.Generic;

namespace CarMiner.Entities
{
    public partial class Add
    {
        public long Idadd { get; set; }
        public int Idbrand { get; set; }
        public int Idmodel { get; set; }
        public long Idotomoto { get; set; }
        public string Mileage { get; set; } = null!;
        public string Fuel { get; set; } = null!;
        public int Prodyear { get; set; }
        public string Power { get; set; } = null!;

        public virtual Brand IdbrandNavigation { get; set; } = null!;
        public virtual Model IdmodelNavigation { get; set; } = null!;
    }
}
