using System;
using System.ComponentModel.DataAnnotations;

namespace Contoso.Unicorn.Entities
{
    /// <summary>
    /// Base entity presentation.
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        [Key]
        [StringLength(36)]
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets created date.
        /// </summary>
        public virtual DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets modified date.
        /// </summary>
        public virtual DateTimeOffset? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets deleted date.
        /// </summary>
        public virtual DateTimeOffset? DeletedDate { get; set; }

        /// <summary>
        /// Gets or sets state.
        /// </summary>
        [StringLength(16)]
        public virtual string State { get; set; }
    }
}