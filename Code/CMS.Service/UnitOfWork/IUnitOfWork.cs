using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS.Service.UnitOfWork
{
    public interface IUnitOfWork
    {
        void Save();
        void RollBack();
    }
}
