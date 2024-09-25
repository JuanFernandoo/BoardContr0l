using BoardContr0l.Models;

namespace BoardContr0l.Dto
{
    public class BoardDto // Transporte de objetos para que no repita un ciclo
    {
        public int BoardId { get; set; } 
        public string Title { get; set; } 
        public bool Status { get; set; }
        public CategoryDto Category { get; set; } // Asocia propiedades de la tabla category
        public string CreatedBy { get; set; } 
        public string ? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ? ModifiedDate { get; set; }

    }
}
