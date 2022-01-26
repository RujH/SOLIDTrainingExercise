using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    public abstract class  RemoveLineItem : LineItem
    {
        public override decimal Profit => Total - TotalLabor - -(TotalLabor * Settings.Instance.TotalLaborSurchargePercent);
        protected override void UpdateEstimatedQuantityForNewContainersRequired(decimal containersRequired)
        {
            throw new NotImplementedException();
        }
    }

    abstract class RemoveMaterialCost : LineItem
    {
        public override decimal MaterialCost => 0;
    }
}
