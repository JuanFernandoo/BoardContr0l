using System.Text.Json.Serialization;

namespace BoardContr0l.Models
{
    public class Role
    {
        public int RoleId { get; set; } // LLave primaria con id unico de role 
        public string RoleName { get; set; } // Propiedad que establece el nombre del rol
        public bool Status { get; set; } // Propiedad que representa la actividad  del board

        [JsonIgnore]
        public string? CreateBy { get; set; } // Propiedad que establece el usuario que crea el ususario, acepta nulos, pero se autimatiza con el controlador
        [JsonIgnore]
        public string? ModifyBy { get; set; } // Propiedad que establece el usuario que modifica el ususario, acepta nulos, pero se autimatiza con el controlador
        [JsonIgnore]
        public DateTime? CreateDate { get; set; } // propiedad que esatblece la fecha de creación del usuario, se inicializa nula, pero se automatiza en el controlador
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }  // propiedad que esatblece la fecha de modificación del usuario, se inicializa nula, pero se automatiza en el controlador
        [JsonIgnore]
        public ICollection<User>? Users { get; set; } // Relación con las propiedades de la tabla de user
    }
}
