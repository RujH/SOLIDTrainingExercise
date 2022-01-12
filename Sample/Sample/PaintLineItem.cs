using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    class PaintLineItem : MaterialClass
    {
        public override decimal AmountPerContainer => Yield.Value;
        public override decimal ContainersRequired => Quantity / AmountPerContainer;

        public override string ContainerName => "Gallons";
        public override decimal MaterialCost => Price * (Quantity / Yield.Value);
    }
}
