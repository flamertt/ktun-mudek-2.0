using BitirmeApi.Business.Abstract;
using BitirmeApi.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.Business.Concrete
{
    public class SurveyService:ISurveyService
    {
        private readonly ISurveyDal _surveyDal;
        public SurveyService(ISurveyDal surveyDal)
        {
            _surveyDal = surveyDal;
        }
    }
}
