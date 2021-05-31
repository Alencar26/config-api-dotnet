using curso.api.Business.Entities;
using curso.api.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace curso.api.Infra.Data
{
    public class CursoDbContext : DbContext
    {
        public CursoDbContext(DbContextOptions<CursoDbContext> options) :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CursoMapping());
            modelBuilder.ApplyConfiguration(new UsuarioMapping());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Curso> Cursos { get; set; }
    }
}
