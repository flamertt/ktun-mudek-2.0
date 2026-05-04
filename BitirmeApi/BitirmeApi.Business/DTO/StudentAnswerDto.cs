namespace BitirmeApi.Business.DTO
{
    public class StudentAnswerDto
    {
        public Guid Id { get; set; }
        public Guid ExamQuestionId { get; set; }
        public int ExternalStudentId { get; set; }
        public decimal Score { get; set; }
    }

    public class CreateStudentAnswerDto
    {
        public Guid ExamQuestionId { get; set; }
        public int ExternalStudentId { get; set; }
        public decimal Score { get; set; }
    }

    public class UpdateStudentAnswerDto
    {
        public Guid Id { get; set; }
        public decimal Score { get; set; }
    }

    public class BulkStudentAnswerItemDto
    {
        public int ExternalStudentId { get; set; }
        public decimal Score { get; set; }
    }

    public class BulkStudentAnswerRequestDto
    {
        public List<BulkStudentAnswerItemDto> Items { get; set; } = new();
    }

    public class BulkOperationResultDto<TIdentifier>
    {
        public List<TIdentifier> Success { get; set; } = new();
        public List<TIdentifier> Failed { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
