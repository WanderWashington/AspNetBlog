using AspNetBlog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetBlog.Data.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
             //Tabela
            builder.ToTable("User");

            //Chave Primária
            builder.HasKey(x=> x.Id);

            builder.Property(x=> x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(); //PK IDENTITY(1,1)

            //Propriedades
            builder.Property(x=> x.Name)
                .IsRequired()
                .HasColumnName("Name")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(80);

            builder.Property(x=> x.Bio);
            builder.Property(x=> x.Email);
            builder.Property(x=> x.Image);
            builder.Property(x=> x.PasswordHash);
            builder.Property(x=> x.Github);

            builder.Property(x=> x.Slug)
                .IsRequired()
                .HasColumnName("Slug")
                .HasColumnType("VARCHAR")
                .HasMaxLength(80);
            
            builder.HasIndex(x=> x.Slug, "IX_User_Slug").IsUnique();

            //Muitos para Muitos.
            builder.HasMany(x=> x.Roles)
                .WithMany(x=> x.Users)
                .UsingEntity<Dictionary<string,object>>( // Virtual Table com String e objetos, N x N gera nova table
                    "UserRole",
                    role=> role.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .HasConstraintName("FK_UserRole_RoleId")
                    .OnDelete(DeleteBehavior.Cascade),
                    user => user.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .HasConstraintName("FK_UserRole_UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                );
            
        }
    }
}