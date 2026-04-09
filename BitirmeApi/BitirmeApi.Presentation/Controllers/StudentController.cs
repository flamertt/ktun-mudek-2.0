using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BitirmeApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "StudentOnly")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentSurveyService _surveyService;

        public StudentController(IStudentSurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        private Guid GetStudentId()
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var id))
                throw new UnauthorizedAccessException("Token geçersiz.");
            return id;
        }

        // ════════════════════════════════════════════════════════════════════════
        // AKTİF DÖNEM DERSLERİM
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Öğrencinin aktif akademik dönemde kayıtlı olduğu dersleri listeler.
        /// Eski dönem dersleri gösterilmez.
        /// </summary>
        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses()
        {
            try
            {
                return Ok(await _surveyService.GetActiveTermCoursesAsync(GetStudentId()));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // ANKETLER
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Belirtilen derse ait aktif anketleri listeler.
        /// Öğrencinin o derse aktif dönemde kayıtlı olması gerekir.
        /// Her ankette "HasSubmitted" ile daha önce katılım durumu gösterilir.
        /// </summary>
        [HttpGet("my-courses/{offeringId}/surveys")]
        public async Task<IActionResult> GetSurveys(Guid offeringId)
        {
            try
            {
                return Ok(await _surveyService.GetActiveSurveysAsync(offeringId, GetStudentId()));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Forbid(); }
        }

        /// <summary>
        /// Anketi sorularıyla birlikte getirir (doldurmak için).
        /// Öğrencinin ilgili derse aktif dönemde kayıtlı olması gerekir.
        /// Daha önce katılındıysa "HasSubmitted: true" döner.
        /// </summary>
        [HttpGet("surveys/{surveyId}")]
        public async Task<IActionResult> GetSurveyDetail(Guid surveyId)
        {
            try
            {
                return Ok(await _surveyService.GetSurveyDetailAsync(surveyId, GetStudentId()));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Forbid(); }
        }

        /// <summary>
        /// Anketi cevaplar. Her ankete yalnızca bir kez katılınabilir.
        /// Body: { answers: [{ questionId, valueNumeric }] }
        /// valueNumeric: sorunun ScaleMin–ScaleMax aralığında olmalı (0 = cevapsız).
        /// </summary>
        [HttpPost("surveys/{surveyId}/submit")]
        public async Task<IActionResult> SubmitSurvey(Guid surveyId, [FromBody] SubmitSurveyDto dto)
        {
            try
            {
                var result = await _surveyService.SubmitAsync(surveyId, GetStudentId(), dto);
                return CreatedAtAction(nameof(GetSurveyDetail), new { surveyId }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Forbid(); }
        }
    }
}
