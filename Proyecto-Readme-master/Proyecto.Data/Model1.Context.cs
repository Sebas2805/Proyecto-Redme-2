using Proyecto.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

public partial class ReadmeDBEntities : DbContext
{
    public ReadmeDBEntities()
      : base("name=ReadmeDBEntities")
    {
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }

    public virtual DbSet<comentario> comentarios { get; set; }
    public virtual DbSet<comunidad> comunidades { get; set; }
    public virtual DbSet<post> posts { get; set; }
    public virtual DbSet<usuario> usuarios { get; set; }
    public virtual DbSet<voto> votoes { get; set; }
}