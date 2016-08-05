using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.OData.Sample.Models.Abstract
{
    public abstract class ModelBase
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public DateTimeOffset? Created { get; set; }
        public string CreatedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }
        public string ModifiedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}