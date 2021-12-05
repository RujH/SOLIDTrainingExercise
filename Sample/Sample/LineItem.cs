using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample
{
    /// <summary>
    /// Applying SRP, OCP and DIP to the LineItem class
    /// </summary>

    #region OCP
    public class AmountPerContainerEnumCheckAccessory : ContainerClass
    {
        public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
        {
            return 1M;
        }
    }

    public class AmountPerContainerEnumCheckAtticblow : ContainerClass
    {
        public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
        {
            return Settings.Instance.AtticBlowSqftBase / MaterialRecord.GetRValue((RValueEnum)(RValue ?? 0));
        }
    }

    public class AmountPerContainerEnumCheckFiberglass : ContainerClass
    {
        public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
        {
            return MaterialRecord.AmountPerContainer.Value;
        }
    }

    public class AmountPerContainerEnumCheckFoam : ContainerClass
    {
        public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
        {
            return (Quantity * Depth.Value) / Yield.Value;
        }
    }

    public class AmountPerContainerEnumCheckPaint : ContainerClass
    {
        public override decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum)
        {
            return Yield.Value;
        }
    }
    #endregion

    #region SRP
    public class ContainerClass
    {

        private MaterialCategoryEnum Category { get; set; }
        public MaterialRecord MaterialRecord { get; internal set; }
        public decimal? Yield { get; internal set; } = 0;
        public decimal Quantity { get; internal set; } = 0;
        public decimal? Depth { get; internal set; }
        public virtual decimal? RValue { get; internal set; }
        private decimal amountPerContainer;
        public virtual decimal AmountPerContainterEnumCheck(MaterialCategoryEnum categoryEnum) { return 0M; }


        public decimal AmountPerContainer
        {
            get
            {
                return AmountPerContainterEnumCheck(Category);
/*                switch (Category)
                {
                    case MaterialCategoryEnum.Accessory:
                        return 1;
                    case MaterialCategoryEnum.Atticblow:
                        return Settings.Instance.AtticBlowSqftBase / MaterialRecord.GetRValue((RValueEnum)(RValue ?? 0));
                    case MaterialCategoryEnum.Fiberglass:
                        return MaterialRecord.AmountPerContainer.Value;
                    case MaterialCategoryEnum.Foam:
                        return (Quantity * Depth.Value) / Yield.Value;
                    case MaterialCategoryEnum.Paint:
                        return Yield.Value;
                    default:
                        return 0;

                }*/
            }
            internal set
            {
                amountPerContainer = value;
            }
        }

        public decimal ContainersRequired
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Accessory:
                        return 1;
                    case MaterialCategoryEnum.Atticblow:
                        return Quantity / AmountPerContainer;
                    case MaterialCategoryEnum.Fiberglass:
                        return Quantity / AmountPerContainer;
                    case MaterialCategoryEnum.Foam:
                        return AmountPerContainer;
                    case MaterialCategoryEnum.Paint:
                        return Quantity / AmountPerContainer;
                    default:
                        return 0;
                }
            }
        }

        public string ContainerName
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Atticblow:
                        return "Bags";
                    case MaterialCategoryEnum.Fiberglass:
                        return "Bags";
                    case MaterialCategoryEnum.Foam:
                        return "Sets";
                    case MaterialCategoryEnum.Paint:
                        return "Gallons";
                    default:
                        return "";
                };
            }
        }
    }

    public class MaterialClass
    {
        private MaterialCategoryEnum Category { get; set; }
        public decimal Quantity { get; internal set; } = 0;
        public decimal Price { get; set; }
        public decimal? Yield { get; internal set; } = 0;
        private ContainerClass containerClass = new ContainerClass();

        public virtual decimal MaterialCost
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Fee:
                        return 0;
                    case MaterialCategoryEnum.Removal:
                        return 0;
                    case MaterialCategoryEnum.Accessory:
                        return Price * Quantity;
                    case MaterialCategoryEnum.Atticblow:
                        return Price * containerClass.ContainersRequired;
                    case MaterialCategoryEnum.Fiberglass:
                        return Price * Quantity;
                    case MaterialCategoryEnum.Foam:
                        return Price * containerClass.AmountPerContainer;
                    case MaterialCategoryEnum.Paint:
                        return Price * (Quantity / Yield.Value);
                    default:
                        throw new InvalidOperationException("Invalid Material Type");
                }
            }
        }
    }

    public class ProfitClass
    {

        private MaterialClass materialClass = new MaterialClass();

        public decimal Price { get; set; }
        public decimal Quantity { get; internal set; } = 0;
        public decimal LaborRate { get; internal set; } = 0;
        public decimal MarkUp { get; internal set; } = 0;
        public decimal TaxRate { get; internal set; } = .1M;
        public MaterialCategoryEnum Category { get; internal set; }

        #region DIP
        IHelpers Helper = new Helpers();
        #endregion

        public decimal Profit
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Fee:
                        return Total;
                    case MaterialCategoryEnum.Removal:
                        return Total - TotalLabor - -(TotalLabor * Settings.Instance.TotalLaborSurchargePercent);
                    default:
                        return Total - (TotalLabor + materialClass.MaterialCost) - (materialClass.MaterialCost * TaxRate) - (TotalLabor * Settings.Instance.TotalLaborSurchargePercent);
                }
            }
        }
        public decimal UnitPrice => Total / (Quantity == 0 ? 1 : Quantity);
        public decimal TotalLabor => LaborRate * Quantity;



        public decimal Total
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Fee:
                        return Price + LaborRate;
                    default:
                        decimal markup = MarkUp != 0 ? MarkUp : 1;
                        decimal adjustedLaborRate = (LaborRate * Quantity) / markup;
                        decimal adjustedMaterialCost = materialClass.MaterialCost / markup;
                        decimal tax = materialClass.MaterialCost * TaxRate;
                        decimal laborRateSurcharge = (LaborRate * Quantity * Settings.Instance.TotalLaborSurchargePercent);
                        return Helper.RoundUp(adjustedMaterialCost + adjustedLaborRate + tax + laborRateSurcharge, 2);
                }
            }
        }
    }

    public class UpdateMarkClass
    {

        public LineItemStatusEnum Status { get; internal set; } = LineItemStatusEnum.Normal;
        public MaterialRecord MaterialRecord { get; internal set; }

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
    }

    public class BaseDescriptionClass
    {
        public virtual decimal? RValue { get; internal set; }
        public MaterialRecord MaterialRecord { get; internal set; }
        public WorkArea WorkArea { get; internal set; }
        public MaterialCategoryEnum Category { get; internal set; }
        public decimal? Depth { get; internal set; }

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
    
    }
    public abstract class UpdateContainerEstimaterClass
    {

        public decimal TaxRate { get; internal set; } = .1M;

        public void UpdateEstimatedFields(decimal containersRequired, decimal taxRate)
        {
            UpdateEstimatedQuantityForNewContainersRequired(containersRequired);
            TaxRate = taxRate;
        }

        protected abstract void UpdateEstimatedQuantityForNewContainersRequired(decimal containersRequired);
    }
    #endregion

    public abstract class LineItem
    {
        private ContainerClass containerClass = new ContainerClass();
        private MaterialClass materialClass = new MaterialClass();
        private ProfitClass profitClass = new ProfitClass();
        private UpdateMarkClass updateMarkClass = new UpdateMarkClass();
        private BaseDescriptionClass baseDescriptionClass = new BaseDescriptionClass();

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
        public decimal TaxAmount => materialClass.MaterialCost * TaxRate;
        public LineItemCategoryEnum LineItemCategory { get; internal set; }
        public MaterialCategoryEnum Category { get; internal set; }
        public int? WorkOrderId { get; internal set; }

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
