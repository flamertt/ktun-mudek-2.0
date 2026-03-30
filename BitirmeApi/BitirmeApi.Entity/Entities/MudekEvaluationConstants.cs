namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// CLO / DÖÇ sonuç satırı türü. Unique: (CourseOfferingId, CourseLearningOutcomeId, ResultType).
    /// </summary>
    public static class CloEvaluationResultType
    {
        public const string Midterm = "Midterm";
        public const string Final = "Final";
        public const string Makeup = "Makeup";
        public const string Combined = "Combined";
    }

    /// <summary>
    /// Başarı notunda %60 ağırlıklı kısımda final mi bütünleme mi kullanıldı.
    /// </summary>
    public static class MudekUsedExamType
    {
        public const string None = "None";
        public const string Final = "Final";
        public const string Makeup = "Makeup";
    }
}
