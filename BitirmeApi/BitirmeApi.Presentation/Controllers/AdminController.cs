using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BitirmeApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IProgramEntityService _programService;
        private readonly IProgramOutcomeService _programOutcomeService;
        private readonly ICourseService _courseService;
        private readonly ICourseLearningOutcomeService _cloService;
        private readonly ICloPoMapService _cloPoMapService;
        private readonly IAcademicTermService _termService;
        private readonly ICourseOfferingService _offeringService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseEvaluationService _evaluationService;
        private readonly IAppUserService _userService;

        public AdminController(
            IProgramEntityService programService,
            IProgramOutcomeService programOutcomeService,
            ICourseService courseService,
            ICourseLearningOutcomeService cloService,
            ICloPoMapService cloPoMapService,
            IAcademicTermService termService,
            ICourseOfferingService offeringService,
            IEnrollmentService enrollmentService,
            ICourseEvaluationService evaluationService,
            IAppUserService userService)
        {
            _programService = programService;
            _programOutcomeService = programOutcomeService;
            _courseService = courseService;
            _cloService = cloService;
            _cloPoMapService = cloPoMapService;
            _termService = termService;
            _offeringService = offeringService;
            _enrollmentService = enrollmentService;
            _evaluationService = evaluationService;
            _userService = userService;
        }

        // ════════════════════════════════════════════════════════════════════════
        // PROGRAM
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("programs")]
        public async Task<IActionResult> GetPrograms()
        {
            var list = await _programService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("programs/{id}")]
        public async Task<IActionResult> GetProgram(Guid id)
        {
            var item = await _programService.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("programs")]
        public async Task<IActionResult> CreateProgram([FromBody] CreateProgramEntityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _programService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetProgram), new { id = result.Id }, result);
        }

        [HttpPut("programs/{id}")]
        public async Task<IActionResult> UpdateProgram(Guid id, [FromBody] UpdateProgramEntityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = id;
            try { return Ok(await _programService.UpdateAsync(dto)); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpDelete("programs/{id}")]
        public async Task<IActionResult> DeleteProgram(Guid id)
        {
            try
            {
                await _programService.DeleteAsync(id);
                return Ok(new { message = "Program başarıyla silindi." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // ────────────────────────────────────────────────────────────────────────
        // PROGRAM ÇIKTISI
        // ────────────────────────────────────────────────────────────────────────

        [HttpGet("program-outcomes")]
        public async Task<IActionResult> GetProgramOutcomes([FromQuery] Guid? programId)
        {
            var list = programId.HasValue
                ? await _programOutcomeService.GetByProgramIdAsync(programId.Value)
                : await _programOutcomeService.GetAllAsync();
            return Ok(list);
        }

        [HttpPost("program-outcomes")]
        public async Task<IActionResult> CreateProgramOutcome([FromBody] CreateProgramOutcomeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var programExists = await _programService.ExistsAsync(dto.ProgramEntityId);
            if (!programExists) return BadRequest(new { message = "Belirtilen program bulunamadı." });
            var result = await _programOutcomeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetProgramOutcomes), null, result);
        }

        [HttpPut("program-outcomes/{id}")]
        public async Task<IActionResult> UpdateProgramOutcome(Guid id, [FromBody] UpdateProgramOutcomeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = id;
            try { return Ok(await _programOutcomeService.UpdateAsync(dto)); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpDelete("program-outcomes/{id}")]
        public async Task<IActionResult> DeleteProgramOutcome(Guid id)
        {
            try
            {
                await _programOutcomeService.DeleteAsync(id);
                return Ok(new { message = "Program çıktısı başarıyla silindi." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // DERS KATALOĞU
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] Guid? programId)
        {
            var list = programId.HasValue
                ? await _courseService.GetByProgramIdAsync(programId.Value)
                : await _courseService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var item = await _courseService.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var programExists = await _programService.ExistsAsync(dto.ProgramEntityId);
            if (!programExists) return BadRequest(new { message = "Belirtilen program bulunamadı." });
            var result = await _courseService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCourse), new { id = result.Id }, result);
        }

        [HttpPut("courses/{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = id;
            try { return Ok(await _courseService.UpdateAsync(dto)); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                await _courseService.DeleteAsync(id);
                return Ok(new { message = "Ders başarıyla silindi." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // DERS ÖĞRENIM ÇIKTISI (CLO)
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Bir derse ait tüm CLO'ları listeler</summary>
        [HttpGet("courses/{courseId}/clos")]
        public async Task<IActionResult> GetClos(Guid courseId)
            => Ok(await _cloService.GetByCourseIdAsync(courseId));

        /// <summary>Tek CLO detayı (bağlı PO eşlemeleriyle)</summary>
        [HttpGet("courses/{courseId}/clos/{cloId}")]
        public async Task<IActionResult> GetClo(Guid courseId, Guid cloId)
        {
            var item = await _cloService.GetByIdAsync(cloId);
            if (item == null || item.CourseId != courseId) return NotFound();
            return Ok(item);
        }

        /// <summary>Derse yeni CLO ekler</summary>
        [HttpPost("courses/{courseId}/clos")]
        public async Task<IActionResult> CreateClo(Guid courseId, [FromBody] CreateCourseLearningOutcomeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.CourseId = courseId;
            try
            {
                var result = await _cloService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetClo), new { courseId, cloId = result.Id }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        /// <summary>CLO günceller (code, description, orderIndex)</summary>
        [HttpPut("courses/{courseId}/clos/{cloId}")]
        public async Task<IActionResult> UpdateClo(Guid courseId, Guid cloId, [FromBody] UpdateCourseLearningOutcomeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = cloId;
            try { return Ok(await _cloService.UpdateAsync(dto)); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        /// <summary>CLO siler</summary>
        [HttpDelete("courses/{courseId}/clos/{cloId}")]
        public async Task<IActionResult> DeleteClo(Guid courseId, Guid cloId)
        {
            try
            {
                await _cloService.DeleteAsync(cloId);
                return Ok(new { message = "Öğrenim çıktısı silindi." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // CLO → PROGRAM ÇIKTISI EŞLEMESİ
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Bir CLO'nun bağlı olduğu program çıktılarını listeler</summary>
        [HttpGet("courses/{courseId}/clos/{cloId}/program-outcomes")]
        public async Task<IActionResult> GetCloMappings(Guid courseId, Guid cloId)
            => Ok(await _cloPoMapService.GetByCloIdAsync(cloId));

        /// <summary>Bir derse ait tüm CLO → PO eşlemelerini listeler</summary>
        [HttpGet("courses/{courseId}/clo-po-maps")]
        public async Task<IActionResult> GetAllCloPoMaps(Guid courseId)
            => Ok(await _cloPoMapService.GetByCourseIdAsync(courseId));

        /// <summary>CLO'yu bir program çıktısıyla eşler</summary>
        [HttpPost("courses/{courseId}/clos/{cloId}/program-outcomes")]
        public async Task<IActionResult> MapCloToPo(Guid courseId, Guid cloId, [FromBody] CreateCloPoMapDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.CourseLearningOutcomeId = cloId;
            try
            {
                var result = await _cloPoMapService.MapAsync(dto);
                return CreatedAtAction(nameof(GetCloMappings), new { courseId, cloId }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        /// <summary>CLO → PO eşleme ağırlığını günceller</summary>
        [HttpPut("courses/{courseId}/clos/{cloId}/program-outcomes/{programOutcomeId}/weight")]
        public async Task<IActionResult> UpdateCloPoWeight(
            Guid courseId, Guid cloId, Guid programOutcomeId, [FromBody] UpdateCloPoWeightDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try { return Ok(await _cloPoMapService.UpdateWeightAsync(cloId, programOutcomeId, dto.Weight)); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>CLO ile program çıktısı arasındaki eşlemeyi kaldırır</summary>
        [HttpDelete("courses/{courseId}/clos/{cloId}/program-outcomes/{programOutcomeId}")]
        public async Task<IActionResult> UnmapCloFromPo(Guid courseId, Guid cloId, Guid programOutcomeId)
        {
            try
            {
                await _cloPoMapService.UnmapAsync(cloId, programOutcomeId);
                return Ok(new { message = "Eşleme kaldırıldı." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // AKADEMİK DÖNEM
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("academic-terms")]
        public async Task<IActionResult> GetAcademicTerms()
            => Ok(await _termService.GetAllAsync());

        [HttpGet("academic-terms/active")]
        public async Task<IActionResult> GetActiveTerm()
        {
            var term = await _termService.GetActiveAsync();
            return term == null ? NotFound(new { message = "Aktif dönem bulunamadı." }) : Ok(term);
        }

        [HttpGet("academic-terms/{id}")]
        public async Task<IActionResult> GetAcademicTerm(Guid id)
        {
            var item = await _termService.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("academic-terms")]
        public async Task<IActionResult> CreateAcademicTerm([FromBody] AcademicTermCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _termService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAcademicTerm), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("academic-terms/{id}")]
        public async Task<IActionResult> UpdateAcademicTerm(Guid id, [FromBody] AcademicTermUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = id;
            try { return Ok(await _termService.UpdateAsync(dto)); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpPut("academic-terms/{id}/set-active")]
        public async Task<IActionResult> SetActiveTerm(Guid id)
        {
            try { return Ok(await _termService.SetActiveAsync(id)); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpDelete("academic-terms/{id}")]
        public async Task<IActionResult> DeleteAcademicTerm(Guid id)
        {
            try
            {
                await _termService.DeleteAsync(id);
                return Ok(new { message = "Akademik dönem silindi." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // DERS AÇILIŞI (COURSE OFFERING)
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("course-offerings")]
        public async Task<IActionResult> GetCourseOfferings()
            => Ok(await _offeringService.GetAllAsync());

        [HttpGet("course-offerings/active-term")]
        public async Task<IActionResult> GetActiveTermOfferings()
            => Ok(await _offeringService.GetByActiveTermAsync());

        [HttpGet("course-offerings/by-term/{termId}")]
        public async Task<IActionResult> GetOfferingsByTerm(Guid termId)
            => Ok(await _offeringService.GetByTermIdAsync(termId));

        [HttpGet("course-offerings/by-teacher/{teacherId}")]
        public async Task<IActionResult> GetOfferingsByTeacher(Guid teacherId, [FromQuery] Guid? termId)
            => Ok(await _offeringService.GetByTeacherIdAndTermAsync(teacherId, termId));

        [HttpGet("course-offerings/by-course/{courseId}")]
        public async Task<IActionResult> GetOfferingsByCourse(Guid courseId)
            => Ok(await _offeringService.GetByCourseIdAsync(courseId));

        [HttpGet("course-offerings/{id}")]
        public async Task<IActionResult> GetCourseOffering(Guid id)
        {
            var item = await _offeringService.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("course-offerings")]
        public async Task<IActionResult> CreateCourseOffering([FromBody] CourseOfferingCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _offeringService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetCourseOffering), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("course-offerings/{id}")]
        public async Task<IActionResult> UpdateCourseOffering(Guid id, [FromBody] CourseOfferingUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = id;
            try { return Ok(await _offeringService.UpdateAsync(dto)); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpPut("course-offerings/{id}/assign-teacher")]
        public async Task<IActionResult> AssignTeacher(Guid id, [FromBody] AssignTeacherDto dto)
        {
            try { return Ok(await _offeringService.AssignTeacherAsync(id, dto.TeacherId)); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpDelete("course-offerings/{id}/remove-teacher")]
        public async Task<IActionResult> RemoveTeacher(Guid id)
        {
            try { return Ok(await _offeringService.RemoveTeacherAsync(id)); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpDelete("course-offerings/{id}")]
        public async Task<IActionResult> DeleteCourseOffering(Guid id)
        {
            try
            {
                await _offeringService.DeleteAsync(id);
                return Ok(new { message = "Ders açılışı silindi." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // KAYIT (ENROLLMENT)
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("course-offerings/{offeringId}/students")]
        public async Task<IActionResult> GetEnrolledStudents(Guid offeringId)
            => Ok(await _enrollmentService.GetByOfferingIdAsync(offeringId));

        [HttpPost("course-offerings/{offeringId}/students")]
        public async Task<IActionResult> EnrollStudent(Guid offeringId, [FromBody] EnrollmentCreateDto dto)
        {
            try { return Ok(await _enrollmentService.EnrollStudentAsync(offeringId, dto.StudentId)); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPost("course-offerings/{offeringId}/students/bulk")]
        public async Task<IActionResult> BulkEnrollStudents(Guid offeringId, [FromBody] EnrollmentBulkCreateDto dto)
        {
            try { return Ok(await _enrollmentService.BulkEnrollStudentsAsync(offeringId, dto.StudentIds)); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPost("course-offerings/{offeringId}/students/import")]
        public async Task<IActionResult> ImportStudentsFromExcel(Guid offeringId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Excel dosyası boş veya yüklenmedi." });

            var studentNumbers = new List<string>();

            ExcelPackage.License.SetNonCommercialPersonal("BitirmeApi");
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);

            var sheet = package.Workbook.Worksheets.FirstOrDefault();
            if (sheet == null) return BadRequest(new { message = "Excel dosyasında sayfa bulunamadı." });

            for (int row = 2; row <= sheet.Dimension?.End.Row; row++)
            {
                var val = sheet.Cells[row, 1].Text?.Trim();
                if (!string.IsNullOrWhiteSpace(val))
                    studentNumbers.Add(val);
            }

            var result = await _enrollmentService.BulkEnrollByStudentNumbersAsync(offeringId, studentNumbers);
            return Ok(result);
        }

        [HttpPut("course-offerings/{offeringId}/students/{studentId}/status")]
        public async Task<IActionResult> UpdateEnrollmentStatus(Guid offeringId, Guid studentId, [FromBody] EnrollmentStatusUpdateDto dto)
        {
            try { return Ok(await _enrollmentService.UpdateStatusAsync(offeringId, studentId, dto.Status)); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpDelete("course-offerings/{offeringId}/students/{studentId}")]
        public async Task<IActionResult> RemoveEnrollment(Guid offeringId, Guid studentId)
        {
            try
            {
                await _enrollmentService.RemoveEnrollmentAsync(offeringId, studentId);
                return Ok(new { message = "Öğrenci kaydı silindi." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // DERS DEĞERLENDİRME (Sadece okuma — yazma işlemleri TeacherController'da)
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("course-offerings/{offeringId}/evaluation")]
        public async Task<IActionResult> GetEvaluationByOffering(Guid offeringId)
        {
            var item = await _evaluationService.GetByOfferingIdAsync(offeringId);
            return item == null ? NotFound(new { message = "Bu açılış için henüz değerlendirme yok." }) : Ok(item);
        }

        [HttpGet("course-evaluations/{id}")]
        public async Task<IActionResult> GetEvaluation(Guid id)
        {
            var item = await _evaluationService.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("course-evaluations")]
        public async Task<IActionResult> GetAllEvaluations()
            => Ok(await _evaluationService.GetAllAsync());

        // ════════════════════════════════════════════════════════════════════════
        // ÖĞRETMEN YÖNETİMİ
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers([FromQuery] Guid? programId)
        {
            var list = programId.HasValue
                ? await _userService.GetTeachersByProgramIdAsync(programId.Value)
                : await _userService.GetTeachersAsync();
            return Ok(list);
        }

        [HttpGet("teachers/{id}")]
        public async Task<IActionResult> GetTeacher(Guid id)
        {
            var item = await _userService.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("teachers")]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var programExists = await _programService.ExistsAsync(dto.ProgramEntityId);
            if (!programExists) return BadRequest(new { message = "Belirtilen program bulunamadı." });
            try
            {
                var createDto = new CreateAppUserDto
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Password = dto.Password ?? "Bitirme2024!",
                    Role = "Teacher",
                    Title = dto.Title,
                    PhoneNumber = dto.PhoneNumber,
                    ProgramEntityId = dto.ProgramEntityId
                };
                var result = await _userService.AddAsync(createDto);
                return CreatedAtAction(nameof(GetTeacher), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("teachers/{id}")]
        public async Task<IActionResult> UpdateTeacher(Guid id, [FromBody] UpdateTeacherDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updateDto = new UpdateAppUserDto
                {
                    Id = id,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Title = dto.Title,
                    PhoneNumber = dto.PhoneNumber,
                    ProgramEntityId = dto.ProgramEntityId,
                    IsActive = dto.IsActive
                };
                return Ok(await _userService.UpdateAsync(updateDto));
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpDelete("teachers/{id}")]
        public async Task<IActionResult> DeleteTeacher(Guid id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return Ok(new { message = "Öğretmen başarıyla silindi." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // ════════════════════════════════════════════════════════════════════════
        // ÖĞRENCİ YÖNETİMİ
        // ════════════════════════════════════════════════════════════════════════

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents([FromQuery] Guid? programId)
        {
            var list = programId.HasValue
                ? await _userService.GetStudentsByProgramIdAsync(programId.Value)
                : await _userService.GetStudentsAsync();
            return Ok(list);
        }

        [HttpGet("students/{id}")]
        public async Task<IActionResult> GetStudent(Guid id)
        {
            var item = await _userService.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("students")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var programExists = await _programService.ExistsAsync(dto.ProgramEntityId);
            if (!programExists) return BadRequest(new { message = "Belirtilen program bulunamadı." });
            try
            {
                var createDto = new CreateAppUserDto
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Password = dto.Password ?? "Bitirme2024!",
                    Role = "Student",
                    StudentNumber = dto.StudentNumber,
                    PhoneNumber = dto.PhoneNumber,
                    ProgramEntityId = dto.ProgramEntityId
                };
                var result = await _userService.AddAsync(createDto);
                return CreatedAtAction(nameof(GetStudent), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpPut("students/{id}")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updateDto = new UpdateAppUserDto
                {
                    Id = id,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    StudentNumber = dto.StudentNumber,
                    PhoneNumber = dto.PhoneNumber,
                    ProgramEntityId = dto.ProgramEntityId,
                    IsActive = dto.IsActive
                };
                return Ok(await _userService.UpdateAsync(updateDto));
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpDelete("students/{id}")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return Ok(new { message = "Öğrenci başarıyla silindi." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }
    }
}
