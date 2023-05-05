using DocumentsAPI.Enums;
using DocumentsAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DocumentsAPI.Data
{
    public class DocumentsContext : DbContext
    {
        public DocumentsContext(DbContextOptions<DocumentsContext> options) : base(options) { } 


        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentStatus> DocumentStatuses { get; set; }
        public DbSet<Status> Statuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasOne(e => e.DocumentStatus)
                .WithOne()
                .HasForeignKey<DocumentStatus>(e => e.DocumentId)
                .IsRequired();

            modelBuilder.Entity<Status>()
                .HasMany<DocumentStatus>()
                .WithOne()
                .HasForeignKey(e => e.StatusId)
                .IsRequired();

            modelBuilder.Entity<Status>().HasData(ListStatuses());

            base.OnModelCreating(modelBuilder);
        }


        private IEnumerable<Status> ListStatuses() 
        {
            foreach (var status in Enum.GetValues<STATUS>())
            {
                yield return new Status { StatusId = status, Name = status.ToString() };
            }
        }

        public void CreateSQLProcedures()
        {
            var connectionString = this.Database.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sql = @"CREATE PROCEDURE addDocument(@Amount INT, @Description NVARCHAR(1000)) as
                            BEGIN
                                INSERT INTO Documents (Amount, Description)
                                VALUES (@Amount, @Description);
                                             
                                DECLARE @DocumentId INT;
                                SET @DocumentId = SCOPE_IDENTITY();

                                INSERT INTO DocumentStatuses (DocumentId, StatusId, DateTime)
                                VALUES (@DocumentId, 1, GETUTCDATE());

                                SELECT @DocumentId AS DocumentId;
                            END;";
                var addDocumentProcedure = new SqlCommand(sql, connection);


                sql = $@"CREATE PROCEDURE getDocuments(@Skip INT, @Take INT) as
                        BEGIN
                            SELECT d.DocumentId, d.Amount, Description FROM Documents AS d
                            JOIN DocumentStatuses AS ds
                            ON d.DocumentId = ds.DocumentId 
                            WHERE ds.StatusId != {(int)STATUS.DELETED}
                            ORDER BY d.DocumentId
                            OFFSET @Skip ROWS
                            FETCH NEXT @Take ROWS ONLY;
                        END;";
                var getDocumentsProcedure = new SqlCommand(sql, connection);


                sql = $@"CREATE PROCEDURE getDocument(@DocumentId INT) as
                        BEGIN
                            SELECT d.DocumentId, d.Amount, Description FROM Documents AS d
                            JOIN DocumentStatuses AS ds
                            ON d.DocumentId = ds.DocumentId 
                            WHERE d.DocumentId = @DocumentId
                            AND ds.StatusId != {(int)STATUS.DELETED};
                        END;";
                var getDocumentProcedure = new SqlCommand(sql, connection);


                sql = $@"CREATE PROCEDURE deleteDocument(@DocumentId INT) as
                        BEGIN
                            UPDATE DocumentStatuses
                            SET StatusId = {(int)STATUS.DELETED}
                            WHERE DocumentId = @DocumentId;
                        END;";
                var deleteDocumentProcedure = new SqlCommand(sql, connection);


                connection.Open();

                addDocumentProcedure.ExecuteNonQuery();
                
                getDocumentsProcedure.ExecuteNonQuery();
                
                getDocumentProcedure.ExecuteNonQuery();
                
                deleteDocumentProcedure.ExecuteNonQuery();
                
                connection.Close();
            }
        }
    }
}

