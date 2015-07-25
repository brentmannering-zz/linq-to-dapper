using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnitTest.Model
{
    [Table("DataType")]
    public class DataType
    {
        [Key]
        [Required]
        public int DataTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        [Column("Name")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime? Created { get; set; } 
    }
}
