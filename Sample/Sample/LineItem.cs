using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample
{
    /// <summary>
    /// Applying SRP, OCP and DIP to the LineItem class
    /// </summary>

    #region OCP
    /* public class AmountPerContainerEnumCheckAccessory : MaterialClass
     {
         public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
         {
             return 1M;
         }
     }

     public class AmountPerContainerEnumCheckAtticblow : MaterialClass
     {
         public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
         {
             return Settings.Instance.AtticBlowSqftBase / MaterialRecord.GetRValue((RValueEnum)(RValue ?? 0));
         }
     }

     public class AmountPerContainerEnumCheckFiberglass : MaterialClass
     {
         public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
         {
             return MaterialRecord.AmountPerContainer.Value;
         }
     }

     public class AmountPerContainerEnumCheckFoam : MaterialClass
     {
         public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
         {
             return (Quantity * Depth.Value) / Yield.Value;
         }
     }

     public class AmountPerContainerEnumCheckPaint : MaterialClass
     {
         public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
         {
             return Yield.Value;
         }
     }*/
    #endregion

    #region SRP

    public abstract class MaterialClass : LineItem
    {


        public virtual decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum) { return 0M; }


        private string BaseDescription => $"{WorkArea?.Name} {MaterialRecord?.Material?.Name}";

        public string Name
        {
            get
            {
                if (Category == MaterialCategoryEnum.Atticblow)
                {
                    return $"R{Math.Round(RValue ?? 0, 0)} {BaseDescription}";
                }
                else if (Category == MaterialCategoryEnum.Foam)
                {
                    return $"R{Math.Round(RValue ?? 0, 0)} {MaterialRecord?.Material?.Name} {Math.Round(Depth ?? 0, 2)}in.";
                }

                return BaseDescription;
            }
        }

        public virtual decimal AmountPerContainer
        {
            get
            {
                return 0;
            }
        }

        public virtual decimal ContainersRequired
        {
            get
            {
                return 0;
            }
        }

        public virtual string ContainerName
        {
            get
            {
                return "";
            }
        }



    }

        public abstract class ProfitClass :LineItem
    {

        public void UpdateEstimatedFields(decimal containersRequired, decimal taxRate)
        {
            UpdateEstimatedQuantityForNewContainersRequired(containersRequired);
            TaxRate = taxRate;
        }

        protected abstract void UpdateEstimatedQuantityForNewContainersRequired(decimal containersRequired);

        #region DIP
        IHelpers Helper = new Helpers();
        #endregion

        public virtual  decimal Profit
        {
            get
            { 
                return Total - (TotalLabor + MaterialCost) - (MaterialCost * TaxRate) - (TotalLabor * Settings.Instance.TotalLaborSurchargePercent);
            }
        }
        public decimal UnitPrice => Total / (Quantity == 0 ? 1 : Quantity);
        public decimal TotalLabor => LaborRate * Quantity;



        public virtual decimal Total
        {
            get
            {
                
                        decimal markup = MarkUp != 0 ? MarkUp : 1;
                        decimal adjustedLaborRate = (LaborRate * Quantity) / markup;
                        decimal adjustedMaterialCost = MaterialCost / markup;
                        decimal tax = MaterialCost * TaxRate;
                        decimal laborRateSurcharge = (LaborRate * Quantity * Settings.Instance.TotalLaborSurchargePercent);
                        return Helper.RoundUp(adjustedMaterialCost + adjustedLaborRate + tax + laborRateSurcharge, 2);
                
            }
        }
    }


    

    #endregion

    public abstract class LineItem
    {

        // private MaterialClass materialClass = new MaterialClass();


        public decimal Price { get; set; }
        public int EstimateId { get; internal set; }
        public string Description { get; internal set; }
        public decimal Quantity { get; internal set; } = 0;
        public decimal? Depth { get; internal set; }
        public decimal? RValuePerInch { get; internal set; }
        public virtual decimal? RValue { get; internal set; }
        public decimal? Yield { get; internal set; } = 0;
        public decimal LaborRate { get; internal set; } = 0;
        public decimal Discount { get; internal set; } = 0;
        public decimal MarkUp { get; internal set; } = 0;
        public decimal TaxRate { get; internal set; } = .1M;
        public bool Taxable { get; internal set; } = true;
        public bool SurchargeExempt { get; internal set; } = false;
        public bool DoNotShowOnEstimate { get; internal set; } = false;
        public bool DoNotShowOnWorkOrder { get; internal set; } = false;
        public LineItemStatusEnum Status { get; internal set; } = LineItemStatusEnum.Normal;
        public int? MaterialRecordId { get; internal set; }
        public MaterialRecord MaterialRecord { get; internal set; }
        public int? WorkAreaId { get; internal set; }
        public WorkArea WorkArea { get; internal set; }
        public List<ReplacedLineItem> ReplacedLineItems { get; internal set; }
        public decimal TaxAmount => MaterialCost * TaxRate;
        public LineItemCategoryEnum LineItemCategory { get; internal set; }
        public MaterialCategoryEnum Category { get; internal set; }
        public int? WorkOrderId { get; internal set; }

        public abstract decimal MaterialCost
        {
            get;
        }


        internal void MarkReplaced()
        {
            if (Status == LineItemStatusEnum.Normal && MaterialRecord.Material.Id != (int)MaterialEnum.VentChute)
            {
                Status = LineItemStatusEnum.Replaced;
            }
        }

        internal void MarkNotReplaced()
        {
            if (Status == LineItemStatusEnum.Replaced)
            {
                Status = LineItemStatusEnum.Normal;
            }
        }

        private decimal amountPerContainer;

        public bool IsOptionItem =>
    new Enum[] {
                LineItemStatusEnum.OptionItem,
                LineItemStatusEnum.RejectedOptionItem,
                LineItemStatusEnum.AcceptedOptionItem
    }.Contains(Status);

        public bool IsVentChute => MaterialRecord?.Material?.Id == (int)MaterialEnum.VentChute;
        public bool IsSlopedCeilingLineItem => WorkAreaId == (int)WorkAreaEnum.SlopedCeiling && !IsVentChute;
        public bool AffectsSlopedCeilingLineItems => IsSlopedCeilingLineItem || (ReplacedLineItems?.Where(l => l.LineItem?.IsSlopedCeilingLineItem ?? false).Any() ?? false);


        public bool IsMatch(LineItem lineItem)
        {
            return lineItem.Quantity == Quantity
                   && lineItem.MaterialRecordId == MaterialRecordId
                   && lineItem.WorkAreaId == WorkAreaId
                   && lineItem.Depth == Depth
                   && lineItem.Status == Status
                   && lineItem.Taxable == Taxable
                   && lineItem.RValue == RValue
                   && lineItem.RValuePerInch == RValuePerInch
                   && lineItem.Yield == Yield
                   && Category == Category;
        }


    }
}
