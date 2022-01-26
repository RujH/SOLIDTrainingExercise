using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    public abstract class FiberglassLineItem : LineItem
    {
        public override decimal AmountPerContainer => MaterialRecord.AmountPerContainer.Value;
        public override decimal ContainersRequired => Quantity / AmountPerContainer;

        public override string ContainerName => "Bags";
        public override decimal MaterialCost => Price * Quantity;
    }
}
