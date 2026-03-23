using BitirmeApi.Business.Abstract;
using BitirmeApi.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.Business.Concrete
{
    public class QuestionService:IQuestionService
    {
        private readonly IQuestionDal _questionDal;
        public QuestionService(IQuestionDal questionDal)
        {
            _questionDal = questionDal;
        }
    }
}
