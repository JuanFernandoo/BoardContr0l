using System;
using System.Text.Json.Serialization;

namespace BoardContr0l.Models
{
    public class Category
    {
        public int CategoryId { get; set; } // LLave primaria con id unico de category
        public string Description { get; set; }  // Propiedad que almacena  una descripción de la categoria
        public bool Status { get; set; }  // Propiedad que representa la actividad  del categoria
        [JsonIgnore]
        public string? CreatedBy { get; set; } // Propiedad que establece el usuario que crea el ususario, acepta nulos, pero se autimatiza con el controlador
        [JsonIgnore]
        public string? ModifiedBy { get; set; } // Propiedad que establece el usuario que modifica el ususario, acepta nulos, pero se autimatiza con el controlador
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }// propiedad que esatblece la fecha de creación del usuario, se inicializa nula, pero se automatiza en el controlador
        [JsonIgnore]
        public DateTime ? ModifiedDate { get; set; } // propiedad que esatblece la fecha de modificación del usuario, se inicializa nula, pero se automatiza en el controlador
    }

}
