using System;
using System.Text.Json.Serialization;

namespace BoardContr0l.Models
{
    public class Board
    {
        public int BoardId { get; set; } // LLave primaria con id unico de user 
        public string Title { get; set; }  // Propiedad que almacena el titulo del board
        public int CategoryId { get; set; } // Llave foranea que asocia el id de categoria
        public bool Status { get; set; } // Propiedad que representa la actividad  del board

        [JsonIgnore]  
        public string? CreatedBy { get; set; }  // Propiedad que establece el usuario que crea el ususario, acepta nulos, pero se autimatiza con el controlador
        [JsonIgnore]
        public string? ModifiedBy { get; set; } // Propiedad que establece el usuario que modifica el ususario, acepta nulos, pero se autimatiza con el controlador
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } // propiedad que esatblece la fecha de creación del usuario, pero se automatiza en el controlador
        [JsonIgnore]
        public DateTime ? ModifiedDate { get; set; } // propiedad que esatblece la fecha de modificación del usuario, se inicializa nula, pero se automatiza en el controlador
        [JsonIgnore]
        public virtual Category? Category { get; set; } // Relación con las propiedades de la tabla de category
        [JsonIgnore]
        public ICollection<Slide> ? Slides { get; set; } // Relación con las propiedades de la tabla de slide

    }

}
