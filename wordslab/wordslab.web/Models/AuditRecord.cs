using System.ComponentModel.DataAnnotations.Schema;

namespace wordslab.web.Models
{
    /// <summary>
    /// Entry in the audit log, used to track a change applied to a single entity.
    /// </summary>
    public class AuditRecord
    {
        public long Id { get; set; }

        /// <summary>
        /// User friendly name of the entity type
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// Primary key of the entity that was changed
        /// </summary>
        public string EntityKey { get; set; }

        /// <summary>
        /// User who made the change
        /// </summary>
        public string UserLogin { get; set; }

        /// <summary>
        /// Date and time when the change was applied
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Create, Update or Delete
        /// </summary>
        public ChangeType ChangeType { get; set; }

        /// <summary>
        /// Key value pairs before the change
        /// </summary>
        [Column(TypeName = "jsonb")]
        public IDictionary<string, string>? ValuesBefore { get; set; }

        /// <summary>
        /// Key value pairs after the change
        /// </summary>
        [Column(TypeName = "jsonb")]
        public IDictionary<string, string>? ValuesAfter { get; set; }
    }

    public enum ChangeType
    {
        Created,
        Updated,
        Deleted
    }
}
