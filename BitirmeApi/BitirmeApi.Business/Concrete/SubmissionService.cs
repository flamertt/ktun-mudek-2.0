using BitirmeApi.Business.Abstract;
using BitirmeApi.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.Business.Concrete
{
    public class SubmissionService:ISubmissionService
    {
        private readonly ISubmissionDal _submissionDal;
        public SubmissionService(ISubmissionDal submissionDal)
        {
            _submissionDal = submissionDal;
        }
    }
}
