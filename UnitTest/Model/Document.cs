using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Model
{
    [Table("Document")]
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }
        public int FieldId { get; set; }
        public string Name { get; set; }
        public DateTime? Created { get; set; } 
    }
}
