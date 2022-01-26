using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    public abstract class FoamLineItem : LineItem
    {
        public override decimal AmountPerContainer => (Quantity * Depth.Value) / Yield.Value;
        public override decimal ContainersRequired => AmountPerContainer;

        public override string ContainerName => "Sets";
        public override decimal MaterialCost => Price * AmountPerContainer;
    }
}

