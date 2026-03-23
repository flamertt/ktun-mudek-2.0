using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class SubmissionDal:EfRepository<Submission,ProjectDbContext>,ISubmissionDal
    {
        public SubmissionDal(ProjectDbContext context):base(context)
        {
            
        }
    }
}
