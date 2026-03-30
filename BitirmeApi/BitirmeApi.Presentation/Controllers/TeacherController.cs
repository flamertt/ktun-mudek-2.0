using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BitirmeApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "TeacherOnly")]
    public class TeacherController : ControllerBase
    {
        private readonly ICourseOfferingService _offeringService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseEvaluationService _evaluationService;
        private readonly IExamService _examService;
        private readonly IExamQuestionService _questionService;
        private readonly IExamQuestionOutcomeMappingService _questionOutcomeService;
        private readonly IAssessmentComponentService _componentService;
        private readonly IAssessmentComponentOutcomeMappingService _componentOutcomeService;
        private readonly IStudentAnswerService _studentAnswerService;
        private readonly IStudentAssessmentComponentScoreService _componentScoreService;
        private readonly ICourseEvaluationLetterGradeRuleService _letterRuleService;
        private readonly ICourseLearningOutcomeService _cloService;
        private readonly IMudekEvaluationCalculatorService _mudekCalculator;

        public TeacherController(
            ICourseOfferingService offeringService,
            IEnrollmentService enrollmentService,
            ICourseEvaluationService evaluationService,
            IExamService examService,
            IExamQuestionService questionService,
            IExamQuestionOutcomeMappingService questionOutcomeService,
            IAssessmentComponentService componentService,
            IAssessmentComponentOutcomeMappingService componentOutcomeService,
            IStudentAnswerService studentAnswerService,
            IStudentAssessmentComponentScoreService componentScoreService,
            ICourseEvaluationLetterGradeRuleService letterRuleService,
            ICourseLearningOutcomeService cloService,
            IMudekEvaluationCalculatorService mudekCalculator)
        {
            _offeringService = offeringService;
            _enrollmentService = enrollmentService;
            _evaluationService = evaluationService;
            _examService = examService;
            _questionService = questionService;
            _questionOutcomeService = questionOutcomeService;
            _componentService = componentService;
            _componentOutcomeService = componentOutcomeService;
            _studentAnswerService = studentAnswerService;
            _componentScoreService = componentScoreService;
            _letterRuleService = letterRuleService;
            _cloService = cloService;
            _mudekCalculator = mudekCalculator;
        }

        private Guid GetTeacherId()
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var id))
                throw new UnauthorizedAccessException("Token geçersiz.");
            return id;
        }

        // ════════════════════════════════════════════════════════════════════════
        // DERSLERİM (CourseOffering üzerinden)
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses([FromQuery] Guid? termId)
            => Ok(await _offeringService.GetByTeacherIdAndTermAsync(GetTeacherId(), termId));

        [HttpGet("my-courses/{offeringId}")]
        public async Task<IActionResult> GetMyCourseDetail(Guid offeringId)
        {
            var offering = await _offeringService.GetByIdForTeacherAsync(offeringId, GetTeacherId());
            return offering == null
                ? NotFound(new { message = "Ders açılışı bulunamadı veya erişim yetkiniz yok." })
                : Ok(offering);
        }

        [HttpGet("my-courses/{offeringId}/students")]
        public async Task<IActionResult> GetCourseStudents(Guid offeringId)
        {
            if (await _offeringService.GetByIdForTeacherAsync(offeringId, GetTeacherId()) == null)
                return NotFound(new { message = "Ders açılışı bulunamadı veya erişim yetkiniz yok." });

            return Ok(await _enrollmentService.GetByOfferingIdAsync(offeringId));
        }

        // ════════════════════════════════════════════════════════════════════════
        // MÜDEK DEĞERLENDİRME
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("my-courses/{offeringId}/evaluation")]
        public async Task<IActionResult> GetEvaluation(Guid offeringId)
        {
            if (await _offeringService.GetByIdForTeacherAsync(offeringId, GetTeacherId()) == null)
                return NotFound(new { message = "Ders açılışı bulunamadı veya erişim yetkiniz yok." });

            var evaluation = await _evaluationService.GetByOfferingIdAsync(offeringId);
            return evaluation == null
                ? NotFound(new { message = "Bu ders açılışı için henüz değerlendirme oluşturulmamış." })
                : Ok(evaluation);
        }

        [HttpPost("my-courses/{offeringId}/evaluation")]
        public async Task<IActionResult> CreateEvaluation(Guid offeringId, [FromBody] CourseEvaluationCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.CourseOfferingId = offeringId;
            try
            {
                var result = await _evaluationService.CreateForTeacherAsync(dto, GetTeacherId());
                return CreatedAtAction(nameof(GetEvaluation), new { offeringId }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("my-courses/{offeringId}/evaluation/{evaluationId}")]
        public async Task<IActionResult> UpdateEvaluation(Guid offeringId, Guid evaluationId, [FromBody] CourseEvaluationUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = evaluationId;
            try { return Ok(await _evaluationService.UpdateForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpDelete("my-courses/{offeringId}/evaluation/{evaluationId}")]
        public async Task<IActionResult> DeleteEvaluation(Guid offeringId, Guid evaluationId)
        {
            try
            {
                await _evaluationService.DeleteForTeacherAsync(evaluationId, GetTeacherId());
                return Ok(new { message = "Değerlendirme silindi." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // SINAV (Exam)
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Bir değerlendirmeye ait sınavları listeler</summary>
        [HttpGet("evaluations/{evaluationId}/exams")]
        public async Task<IActionResult> GetExams(Guid evaluationId)
        {
            try { return Ok(await _examService.GetByEvaluationIdForTeacherAsync(evaluationId, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("exams/{examId}")]
        public async Task<IActionResult> GetExam(Guid examId)
        {
            try
            {
                var item = await _examService.GetByIdForTeacherAsync(examId, GetTeacherId());
                return item == null ? NotFound() : Ok(item);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        /// <summary>
        /// Değerlendirmeye yeni sınav ekler.
        /// Ownership servis katmanında doğrulanır.
        /// </summary>
        [HttpPost("evaluations/{evaluationId}/exams")]
        public async Task<IActionResult> CreateExam(Guid evaluationId, [FromBody] CreateExamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.CourseEvaluationId = evaluationId;
            try
            {
                var result = await _examService.CreateAsync(dto, GetTeacherId());
                return CreatedAtAction(nameof(GetExam), new { examId = result.Id }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("exams/{examId}")]
        public async Task<IActionResult> UpdateExam(Guid examId, [FromBody] UpdateExamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = examId;
            try { return Ok(await _examService.UpdateAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("exams/{examId}")]
        public async Task<IActionResult> DeleteExam(Guid examId)
        {
            try
            {
                await _examService.DeleteAsync(examId, GetTeacherId());
                return Ok(new { message = "Sınav silindi." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // SINAV SORUSU (ExamQuestion)
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("exams/{examId}/questions")]
        public async Task<IActionResult> GetQuestions(Guid examId)
        {
            try { return Ok(await _questionService.GetByExamIdForTeacherAsync(examId, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("exam-questions/{questionId}")]
        public async Task<IActionResult> GetQuestion(Guid questionId)
        {
            try
            {
                var item = await _questionService.GetByIdForTeacherAsync(questionId, GetTeacherId());
                return item == null ? NotFound() : Ok(item);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        /// <summary>
        /// Sınava yeni soru ekler.
        /// Ownership servis katmanında doğrulanır.
        /// </summary>
        [HttpPost("exams/{examId}/questions")]
        public async Task<IActionResult> CreateQuestion(Guid examId, [FromBody] CreateExamQuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.ExamId = examId;
            try
            {
                var result = await _questionService.CreateAsync(dto, GetTeacherId());
                return CreatedAtAction(nameof(GetQuestion), new { questionId = result.Id }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("exam-questions/{questionId}")]
        public async Task<IActionResult> UpdateQuestion(Guid questionId, [FromBody] UpdateExamQuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = questionId;
            try { return Ok(await _questionService.UpdateAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpDelete("exam-questions/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(Guid questionId)
        {
            try
            {
                await _questionService.DeleteAsync(questionId, GetTeacherId());
                return Ok(new { message = "Soru silindi." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpGet("exam-questions/{questionId}/clos")]
        public async Task<IActionResult> GetQuestionClos(Guid questionId)
        {
            try { return Ok(await _questionOutcomeService.GetByQuestionIdForTeacherAsync(questionId, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("exam-questions/{questionId}/clos")]
        public async Task<IActionResult> AddQuestionCloMapping(Guid questionId, [FromBody] CreateExamQuestionOutcomeMappingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.ExamQuestionId = questionId;
            try { return Ok(await _questionOutcomeService.AddForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("exam-question-outcome-mappings/{mappingId}")]
        public async Task<IActionResult> UpdateQuestionCloMapping(Guid mappingId, [FromBody] UpdateExamQuestionOutcomeMappingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = mappingId;
            try { return Ok(await _questionOutcomeService.UpdateForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpDelete("exam-question-outcome-mappings/{mappingId}")]
        public async Task<IActionResult> DeleteQuestionCloMapping(Guid mappingId)
        {
            try { await _questionOutcomeService.DeleteForTeacherAsync(mappingId, GetTeacherId()); return Ok(new { message = "Mapping silindi." }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("exams/{examId}/components")]
        public async Task<IActionResult> GetComponents(Guid examId)
        {
            try { return Ok(await _componentService.GetByExamIdForTeacherAsync(examId, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("assessment-components/{componentId}")]
        public async Task<IActionResult> GetComponent(Guid componentId)
        {
            try
            {
                var item = await _componentService.GetByIdForTeacherAsync(componentId, GetTeacherId());
                return item == null ? NotFound() : Ok(item);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("exams/{examId}/components")]
        public async Task<IActionResult> CreateComponent(Guid examId, [FromBody] CreateAssessmentComponentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.ExamId = examId;
            try { return Ok(await _componentService.AddForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("assessment-components/{componentId}")]
        public async Task<IActionResult> UpdateComponent(Guid componentId, [FromBody] UpdateAssessmentComponentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = componentId;
            try { return Ok(await _componentService.UpdateForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpDelete("assessment-components/{componentId}")]
        public async Task<IActionResult> DeleteComponent(Guid componentId)
        {
            try { await _componentService.DeleteForTeacherAsync(componentId, GetTeacherId()); return Ok(new { message = "Component silindi." }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpGet("assessment-components/{componentId}/clos")]
        public async Task<IActionResult> GetComponentClos(Guid componentId)
        {
            try { return Ok(await _componentOutcomeService.GetByComponentIdForTeacherAsync(componentId, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("assessment-components/{componentId}/clos")]
        public async Task<IActionResult> AddComponentClo(Guid componentId, [FromBody] CreateAssessmentComponentOutcomeMappingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.AssessmentComponentId = componentId;
            try { return Ok(await _componentOutcomeService.AddForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("assessment-component-outcome-mappings/{mappingId}")]
        public async Task<IActionResult> UpdateComponentClo(Guid mappingId, [FromBody] UpdateAssessmentComponentOutcomeMappingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = mappingId;
            try { return Ok(await _componentOutcomeService.UpdateForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpDelete("assessment-component-outcome-mappings/{mappingId}")]
        public async Task<IActionResult> DeleteComponentClo(Guid mappingId)
        {
            try { await _componentOutcomeService.DeleteForTeacherAsync(mappingId, GetTeacherId()); return Ok(new { message = "Mapping silindi." }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("exam-questions/{questionId}/answers")]
        public async Task<IActionResult> GetAnswers(Guid questionId)
        {
            try { return Ok(await _studentAnswerService.GetByQuestionForTeacherAsync(questionId, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("exam-questions/{questionId}/answers")]
        public async Task<IActionResult> AddAnswer(Guid questionId, [FromBody] CreateStudentAnswerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.ExamQuestionId = questionId;
            try { return Ok(await _studentAnswerService.AddForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPost("exam-questions/{questionId}/answers/bulk")]
        public async Task<IActionResult> AddAnswersBulk(Guid questionId, [FromBody] BulkStudentAnswerRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try { return Ok(await _studentAnswerService.AddBulkForTeacherAsync(questionId, dto.Items, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPut("student-answers/{answerId}")]
        public async Task<IActionResult> UpdateAnswer(Guid answerId, [FromBody] UpdateStudentAnswerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = answerId;
            try { return Ok(await _studentAnswerService.UpdateForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("student-answers/{answerId}")]
        public async Task<IActionResult> DeleteAnswer(Guid answerId)
        {
            try { await _studentAnswerService.DeleteForTeacherAsync(answerId, GetTeacherId()); return Ok(new { message = "Answer silindi." }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("assessment-components/{componentId}/scores")]
        public async Task<IActionResult> GetScores(Guid componentId)
        {
            try { return Ok(await _componentScoreService.GetByComponentForTeacherAsync(componentId, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("assessment-components/{componentId}/scores")]
        public async Task<IActionResult> AddScore(Guid componentId, [FromBody] CreateStudentAssessmentComponentScoreDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.AssessmentComponentId = componentId;
            try { return Ok(await _componentScoreService.AddForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPost("assessment-components/{componentId}/scores/bulk")]
        public async Task<IActionResult> AddScoresBulk(Guid componentId, [FromBody] BulkStudentScoreDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try { return Ok(await _componentScoreService.AddBulkForTeacherAsync(componentId, dto.Scores, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPut("student-assessment-component-scores/{scoreId}")]
        public async Task<IActionResult> UpdateScore(Guid scoreId, [FromBody] UpdateStudentAssessmentComponentScoreDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = scoreId;
            try { return Ok(await _componentScoreService.UpdateForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("student-assessment-component-scores/{scoreId}")]
        public async Task<IActionResult> DeleteScore(Guid scoreId)
        {
            try { await _componentScoreService.DeleteForTeacherAsync(scoreId, GetTeacherId()); return Ok(new { message = "Score silindi." }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("evaluations/{evaluationId}/letter-grade-rules")]
        public async Task<IActionResult> GetLetterRules(Guid evaluationId)
        {
            try { return Ok(await _letterRuleService.GetByEvaluationForTeacherAsync(evaluationId, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("evaluations/{evaluationId}/letter-grade-rules")]
        public async Task<IActionResult> AddLetterRule(Guid evaluationId, [FromBody] CreateCourseEvaluationLetterGradeRuleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.CourseEvaluationId = evaluationId;
            try { return Ok(await _letterRuleService.AddForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("letter-grade-rules/{ruleId}")]
        public async Task<IActionResult> UpdateLetterRule(Guid ruleId, [FromBody] UpdateCourseEvaluationLetterGradeRuleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = ruleId;
            try { return Ok(await _letterRuleService.UpdateForTeacherAsync(dto, GetTeacherId())); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpDelete("letter-grade-rules/{ruleId}")]
        public async Task<IActionResult> DeleteLetterRule(Guid ruleId)
        {
            try { await _letterRuleService.DeleteForTeacherAsync(ruleId, GetTeacherId()); return Ok(new { message = "Rule silindi." }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("my-courses/{offeringId}/clos")]
        public async Task<IActionResult> GetClosByOffering(Guid offeringId)
        {
            var offering = await _offeringService.GetByIdForTeacherAsync(offeringId, GetTeacherId());
            if (offering == null) return NotFound(new { message = "Ders açılışı bulunamadı veya erişim yetkiniz yok." });
            return Ok(await _cloService.GetByCourseIdAsync(offering.CourseId));
        }

        // ════════════════════════════════════════════════════════════════════════
        // MÜDEK snapshot hesapları (docs/MUDEK_Rapor.md zinciri — geçme notu → çıktılar → DÖÇ → PÇ)
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Kaydedilmiş MÜDEK sonuç özeti. Ham veri değiştiyse <see cref="MudekEvaluationSnapshotDto.IsCalculationDirty"/> true olabilir.</summary>
        [HttpGet("my-courses/{offeringId}/mudek-evaluation/results")]
        public async Task<IActionResult> GetMudekResults(Guid offeringId)
        {
            if (await _offeringService.GetByIdForTeacherAsync(offeringId, GetTeacherId()) == null)
                return NotFound(new { message = "Ders açılışı bulunamadı veya erişim yetkiniz yok." });

            return Ok(await _mudekCalculator.GetSnapshotForTeacherAsync(offeringId, GetTeacherId()));
        }

        /// <summary>
        /// <b>Hesapla:</b> Vize/final/büt soru ve bileşen notları, harf kuralları, soru–DÖÇ ve DÖÇ–PÇ eşlemeleriyle
        /// rapordaki zinciri çalıştırır; sonuç tablolarına yazar. Önce değerlendirme kaydı ve sınavlar oluşturulmuş olmalıdır.
        /// </summary>
        [HttpPost("my-courses/{offeringId}/mudek-evaluation/calculate")]
        public Task<IActionResult> CalculateMudek(Guid offeringId) => ExecuteMudekRecalculateAsync(offeringId);

        /// <summary>
        /// Aynı işlem <c>calculate</c> ile. Offering için mevcut snapshot satırlarını siler, baştan hesaplar, tek transaction’da kaydeder.
        /// </summary>
        [HttpPost("my-courses/{offeringId}/mudek-evaluation/recalculate")]
        public Task<IActionResult> RecalculateMudek(Guid offeringId) => ExecuteMudekRecalculateAsync(offeringId);

        /// <summary>
        /// Değerlendirme ekranında yalnızca <c>evaluationId</c> varsa: aynı hesaplamayı bu id üzerinden tetikler.
        /// </summary>
        [HttpPost("evaluations/{evaluationId}/mudek-evaluation/calculate")]
        public async Task<IActionResult> CalculateMudekForEvaluation(Guid evaluationId)
        {
            var eval = await _evaluationService.GetByIdAsync(evaluationId);
            if (eval == null)
                return NotFound(new { message = "Değerlendirme bulunamadı." });
            if (await _offeringService.GetByIdForTeacherAsync(eval.CourseOfferingId, GetTeacherId()) == null)
                return NotFound(new { message = "Ders açılışı bulunamadı veya erişim yetkiniz yok." });
            return await ExecuteMudekRecalculateAsync(eval.CourseOfferingId);
        }

        private async Task<IActionResult> ExecuteMudekRecalculateAsync(Guid courseOfferingId)
        {
            try
            {
                return Ok(await _mudekCalculator.RecalculateForTeacherAsync(courseOfferingId, GetTeacherId()));
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("evaluations/{evaluationId}/full-detail")]
        public async Task<IActionResult> GetEvaluationFullDetail(Guid evaluationId)
        {
            try
            {
                var exams = await _examService.GetByEvaluationIdForTeacherAsync(evaluationId, GetTeacherId());
                var componentsByExam = new Dictionary<Guid, object>();
                foreach (var exam in exams)
                {
                    componentsByExam[exam.Id] = await _componentService.GetByExamIdForTeacherAsync(exam.Id, GetTeacherId());
                }
                return Ok(new { evaluationId, exams, componentsByExam });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }
    }
}
