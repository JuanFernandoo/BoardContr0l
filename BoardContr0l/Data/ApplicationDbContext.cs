using Microsoft.EntityFrameworkCore;
using BoardContr0l.Models;

namespace BoardContr0l.Data
{
    public class ApplicationDbContext : DbContext // Clase que heresa DbContext que es la se usa para la base de datos
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) // Configuración para  la conexión de la base de datos
            : base(options)
        {
        }
        // Representación de cada tabla de base datos 
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Slide> Slides { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Mapeo de las relaciones entre entidades 
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role) // Relación de uno a muchos con role
                .WithMany(r => r.Users) //  Relación de varios roles por usuario
                .HasForeignKey(u => u.RoleId) // Llave foranéa que hace referencia a Role
                .OnDelete(DeleteBehavior.Cascade); // Especifica que si se elimina un rol, elimina todos los usuarios con este rol
        }
    }
}
