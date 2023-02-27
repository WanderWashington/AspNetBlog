using AspNetBlog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetBlog.Data.Mappings
{
    public class PostMap : IEntityTypeConfiguration<Post>

    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Post");

            //Chave Primária
            builder.HasKey(x=> x.Id);

            builder.Property(x=> x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(); //PK IDENTITY(1,1)

            //Propriedades
            builder.Property(x=> x.LastUpdateDate)
                .IsRequired()
                .HasColumnName("LastUpdateDate")
                .HasColumnType("SMALLDATETIME")
                .HasDefaultValueSql("GETDATE()"); //SQL Command.
                //.HasDefaultValue(DateTime.Now.ToUniversalTime()); //Code command
            
            builder.HasIndex(x=> x.Slug, "IX_User_Slug").IsUnique();

            //Relacionamentos
            builder.HasOne(x=> x.Author) //tem um pra muitos
                .WithMany(x=> x.Posts) //pra muitos.. ja entende o User
                .HasConstraintName("FK_Post_Author") //Gera uma constraint com o nome definido
                .OnDelete(DeleteBehavior.Cascade); //Delete type
            
            builder.HasOne(x=> x.Category) //tem um pra muitos
                .WithMany(x=> x.Posts) //pra muitos.. ja entende o User
                .HasConstraintName("FK_Post_Category") //Gera uma constraint com o nome definido
                .OnDelete(DeleteBehavior.Cascade); //Delete type

            //Muitos para Muitos.
            builder.HasMany(x=> x.Tags)
                .WithMany(x=> x.Posts)
                .UsingEntity<Dictionary<string,object>>( // Virtual Table com String e objetos, N x N gera nova table
                    "PostTag",
                    post=> post.HasOne<Tag>()
                    .WithMany()
                    .HasForeignKey("PostId")
                    .HasConstraintName("FK_PostTag_PostId")
                    .OnDelete(DeleteBehavior.Cascade),
                    tag => tag.HasOne<Post>()
                    .WithMany()
                    .HasForeignKey("TagId")
                    .HasConstraintName("FK_PostTag_TagId")
                    .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}