using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    class FeeLineItem: LineItem
    {
        public override decimal MaterialCost => 0;

        public override decimal Profit => Total;
        public override decimal Total => Price + LaborRate;
    }
}
