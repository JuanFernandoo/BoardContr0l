using System;
using System.Text.Json.Serialization;

namespace BoardContr0l.Models
{
    public class Slide
    {
        public int SlideId { get; set; } // LLave primaria con id unico de slide 
        public string Description { get; set; } // Propiedad que establece una descripcion de cada slide 
        public string URL { get; set; } // propiedad que establece una URL 
        public int Time { get; set; } // Tiempo en segundos que esatablece la duración de cada slide
        public int BoardId { get; set; } // LLave foranea que se asocia con el id del board
        public bool Status { get; set; } // Propiedad que representa la actividad  del board
        
        [JsonIgnore]
        public string? CreatedBy { get; set; } // Propiedad que establece el usuario que crea el ususario, acepta nulos, pero se autimatiza con el controlador
        
        [JsonIgnore]
        public string? ModifiedBy { get; set; } // Propiedad que establece el usuario que modifica el ususario, acepta nulos, pero se autimatiza con el controlador
        
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }  // Propiedad que establece la fecha de creación del slie, esta se automatiza en el controlado para que se coloque automaticamente
       
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; } // propiedad que esatblece la fecha de modificación del usuario, se inicializa nula, pero se automatiza en el controlador
        
        [JsonIgnore]
        public Board? Board { get; set; }  // Relación con las propiedades de la tabla de board
    }
}
