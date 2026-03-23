using BitirmeApi.Business.Abstract;
using BitirmeApi.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.Business.Concrete
{
    internal class ProgramOutcomeContributionService: IProgramOutcomeContributionService
    { 
        private readonly IProgramOutcomeContributionDal _programOutcomeContributionDal;
        public ProgramOutcomeContributionService(IProgramOutcomeContributionDal programOutcomeContributionDal)
        {
            _programOutcomeContributionDal = programOutcomeContributionDal;
        }
    }
}
