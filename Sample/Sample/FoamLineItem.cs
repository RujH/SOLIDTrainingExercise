using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    class FoamLineItem : MaterialClass
    {
        public override decimal AmountPerContainer => (Quantity * Depth.Value) / Yield.Value;
        public override decimal ContainersRequired => AmountPerContainer;

        public override string ContainerName => "Sets";
        public override decimal MaterialCost => Price * AmountPerContainer;
    }
}

