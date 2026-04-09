using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BitirmeApi.Core.Entity;

namespace BitirmeApi.Entity.Entities
{
    public class ProgramEntity : IEntity
    {
        public ProgramEntity()
        {
            ProgramOutcomes = new List<ProgramOutcome>();
            Courses = new List<Course>();
            LetterGradeRules = new List<ProgramLetterGradeRule>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(256)]
        public string Name { get; set; } = default!;

        public int AccreditationCycleYears { get; set; } = 5;

        public ICollection<ProgramOutcome> ProgramOutcomes { get; set; }
        public ICollection<Course> Courses { get; set; }

        public ICollection<ProgramLetterGradeRule> LetterGradeRules { get; set; }
    }
}
