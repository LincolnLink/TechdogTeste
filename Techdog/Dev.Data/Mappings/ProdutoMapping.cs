using Dev.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev.Data.Mappings
{
    public class ProdutoMapping : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            // Define a chave primaria
            builder.HasKey(p => p.Id);

            // Define o campo como requirido, define o tipo da coluna            
            builder.Property(p => p.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            // Define o campo como requirido, define o tipo da coluna            
            builder.Property(p => p.Descricao)                
                .HasColumnType("varchar(1000)");

            // Define o campo como requirido, define o tipo da coluna            
            builder.Property(p => p.Imagem)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(p => p.Valor)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            builder.ToTable("Produtos");

        }
    }
}
