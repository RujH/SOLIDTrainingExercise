using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    class AccessoryLineItem : MaterialClass
    {
        public override decimal AmountPerContainer => 1;
        public override decimal ContainersRequired => 1;
        public override decimal MaterialCost => Price * Quantity;
    }
}

