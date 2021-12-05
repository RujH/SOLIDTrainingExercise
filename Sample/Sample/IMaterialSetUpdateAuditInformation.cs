using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    interface IMaterialSetUpdateAuditInformation
    {
        internal void SetUpdateAuditInformation(int updatedByUserId, DateTime updatedDate);
    }
}
