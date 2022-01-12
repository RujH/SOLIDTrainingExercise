using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    public class AtticblowLineItem : MaterialClass
    {
        public override decimal AmountPerContainer { get { return Settings.Instance.AtticBlowSqftBase / MaterialRecord.GetRValue((RValueEnum)(RValue ?? 0)); } };
        public override decimal ContainersRequired => Quantity / AmountPerContainer;

        public override string ContainerName => "Bags";
        public override decimal MaterialCost => Price * ContainersRequired;
    }

  
}
