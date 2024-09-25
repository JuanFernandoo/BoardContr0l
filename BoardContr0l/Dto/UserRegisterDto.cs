namespace BoardContr0l.Dto
{
    public class UserRegisterDto // Clase que contiene los datos del registro
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
    }
}
