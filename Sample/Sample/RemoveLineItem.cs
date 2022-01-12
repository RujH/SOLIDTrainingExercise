using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    class RemoveLineItem :ProfitClass
    {
        public override decimal Profit => Total - TotalLabor - -(TotalLabor * Settings.Instance.TotalLaborSurchargePercent);
        protected override void UpdateEstimatedQuantityForNewContainersRequired(decimal containersRequired)
        {
            throw new NotImplementedException();
        }
    }

    class RemoveMaterialCost : MaterialClass
    {
        public override decimal MaterialCost => 0;
    }
}
