using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IProgramEntityDal : IRepository<ProgramEntity>
    {
        Task<bool> ExistsAsync(Guid id);
    }
}
