using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{

    /// <summary>
    /// Applying ISP
    /// </summary>
    public class Material : IMaterialSetCreateAuditInformation, IMaterialSetUpdateAuditInformation
    {
        private Material()
        {
        }

        public int Id { get; set; }
        public bool Active { get; private set; } = true;
        public string Name { get; private set; }
        public MaterialCategoryEnum Category { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }
        public DateTime? DeletedDate { get; private set; }
        public int CreatedBy { get; private set; }
        public int? UpdatedBy { get; private set; }
        public int? DeletedBy { get; private set; }
        public bool Deleted { get; private set; }

        void IMaterialSetCreateAuditInformation.SetCreateAuditInformation(int createdByUserId)
        {
            CreatedBy = createdByUserId;
            CreatedDate = DateTime.Now;
        }

        void IMaterialSetUpdateAuditInformation.SetUpdateAuditInformation(int updatedByUserId, DateTime updatedDate)
        {
            UpdatedBy = updatedByUserId;
            UpdatedDate = updatedDate;
        }
    }
}
