namespace BoardContr0l.Models
{
    public class User
    {
        public int UserId { get; set; } // LLave primaria con id unico de user 
        public string Username { get; set; } // Propiedad que establece el usuario
        public string PasswordHash { get; set; } // Propiedad que guarda la contraseña en texto plano
        public string Email { get; set; } // Propiedad que establece el ingreso de un email
        public int RoleId { get; set; }  // LLave foranea que se asocia con el id del rol
        public bool Status { get; set; } // Propiedad que representa la actividad  del board
        public string? CreateBy { get; set; } // Propiedad que establece el usuario que crea el ususario, acepta nulos, pero se autimatiza con el controlador
        public string? ModifyBy { get; set; } // Propiedad que establece el usuario que modifica el ususario, acepta nulos, pero se autimatiza con el controlador
        public DateTime CreateDate { get; set; } // propiedad que esatblece la fecha de creación del usuario, se inicializa nula, pero se automatiza en el controlador
        public DateTime ModifiedDate { get; set; } // propiedad que esatblece la fecha de modificación del usuario, se inicializa nula, pero se automatiza en el controlador

        public Role Role { get; set; }  // Relación con las propiedades de la tabla de role
    }
}
