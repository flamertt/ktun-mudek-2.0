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
    public class AnswerDal:EfRepository<Answer,ProjectDbContext>,IAnswerDal
    {
        public AnswerDal(ProjectDbContext context):base(context)
        {
         
        }
    }
}
