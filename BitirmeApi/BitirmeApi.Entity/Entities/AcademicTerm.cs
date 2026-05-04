using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Aktif akademik dönem — üniversite API'sindeki en güncel dönem burada saklanır.
    /// Id, üniversite API'sinin döndürdüğü harici ID'dir (auto-increment değil).
    /// </summary>
    public class AcademicTerm : IEntity
    {
        /// <summary>Üniversite API'sindeki dönem ID'si (harici, otomatik artmaz)</summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>Dönem adı, ör: "2024-25 Güz Dönemi"</summary>
        [Required, MaxLength(100)]
        public string Ad { get; set; } = default!;
    }
}
