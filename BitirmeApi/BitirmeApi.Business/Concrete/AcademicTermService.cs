using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.Integration.Abstract;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class AcademicTermService : IAcademicTermService
    {
        private readonly IAcademicTermDal _dal;
        private readonly IUniversityApiService _universityApi;

        public AcademicTermService(IAcademicTermDal dal, IUniversityApiService universityApi)
        {
            _dal = dal;
            _universityApi = universityApi;
        }

        public async Task<AcademicTerm?> GetActiveAsync() =>
            await _dal.GetActiveAsync();

        public async Task<AcademicTerm> SyncActiveAsync(string universityToken)
        {
            var terms = await _universityApi.GetAcademicTermsAsync(universityToken);
            if (terms == null || terms.Count == 0)
                throw new InvalidOperationException("Üniversite API'sinden akademik dönem alınamadı.");

            // En büyük ID = en güncel dönem
            var latest = terms.OrderByDescending(t => t.AcademicTermId).First();

            var existing = await _dal.GetAsync(t => t.Id == latest.AcademicTermId);
            if (existing == null)
            {
                var newTerm = new AcademicTerm { Id = latest.AcademicTermId, Ad = latest.AcademicTermName };
                _dal.Add(newTerm);
                return newTerm;
            }

            existing.Ad = latest.AcademicTermName;
            _dal.Update(existing);
            return existing;
        }
    }
}
