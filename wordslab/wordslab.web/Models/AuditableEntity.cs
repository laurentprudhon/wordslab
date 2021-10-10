using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wordslab.web.Models
{
    /// <summary>
    /// Just derive from this class and Entity Framework Core will track all changes applied to this entity 
    /// in the database AuditLog each time SaveChanges is called on the DBContext.
    /// </summary>
    public abstract class AuditableEntity
    {
        /// <summary>
        /// Object type name saved in the audit log is the class name by default.
        /// You can choose any other name of our choice by overriding this property.
        /// </summary>
        [NotMapped]
        public string EntityTypeName
        {
            get {  return GetType().Name; }
        }

        /// <summary>
        /// Primary key auto generated for the entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    }
}
