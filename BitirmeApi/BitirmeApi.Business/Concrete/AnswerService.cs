using BitirmeApi.Business.Abstract;
using BitirmeApi.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.Business.Concrete
{
    public class AnswerService:IAnswerService
    {
        private readonly IAnswerDal _answerDal;
        public AnswerService(IAnswerDal answerDal)
        {
            _answerDal = answerDal;
        }
    }
}
