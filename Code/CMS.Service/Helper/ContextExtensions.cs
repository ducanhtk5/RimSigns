using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace CMS.Service.Helper
{
    public static class ContextExtensions
    {
        public static void AttachToOrGet<T>(this ObjectContext context, string entitySetName, ref T entity)
        {
            ObjectStateEntry entry;
            // Track whether we need to perform an attach
            bool attach = false;
            if (context.ObjectStateManager.TryGetObjectStateEntry(context.CreateEntityKey(entitySetName, entity), out entry))
            {
                // Re-attach if necessary
                attach = entry.State == EntityState.Detached;
                // Get the discovered entity to the ref
                entity = (T)entry.Entity;
            }
            else
            {
                // Attach for the first time
                attach = true;
            }
            if (attach)
                context.AttachTo(entitySetName, entity);
        }
    }
}
