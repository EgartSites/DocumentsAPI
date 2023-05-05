using DocumentsAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DocumentsAPI.Data.Repositories
{
    public class ADODocumentRepository : IDocumentRepository
    {
        private readonly string? _connectionString;


        public ADODocumentRepository(string? connectionString) 
        {
            _connectionString = connectionString;
        }


        public async Task<Document> AddDocument(Document document)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("addDocument", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Amount", document.Amount);
                cmd.Parameters.AddWithValue("@Description", document.Description);

                await connection.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    document.DocumentId = Convert.ToInt32(reader["DocumentId"]);
                }

                await connection.CloseAsync();
            }

            return document;
        }


        public async Task<bool> DeleteDocument(int documentId)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("deleteDocument", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DocumentId", documentId);

                await connection.OpenAsync();

                result = await cmd.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }

            if (result == 0)
            {
                return false;
            }

            return true;
        }


        public async Task<Document?> GetDocument(int documentId)
        {
            Document? document = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("getDocument", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DocumentId", documentId);

                await connection.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    document = new Document
                    {
                        DocumentId = Convert.ToInt32(reader["DocumentId"]),
                        Amount = Convert.ToInt32(reader["Amount"]),
                        Description = reader["Description"].ToString() ?? ""
                    };
                }

                await connection.CloseAsync();
            }

            return document;
        }


        public async Task<IEnumerable<Document>> GetDocuments(int skip, int take)
        {
            List<Document> documents = new();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("getDocuments", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Skip", skip);
                cmd.Parameters.AddWithValue("@Take", take);

                await connection.OpenAsync();

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var document = new Document
                    {
                        DocumentId = Convert.ToInt32(reader["DocumentId"]),
                        Amount = Convert.ToInt32(reader["Amount"]),
                        Description = reader["Description"].ToString() ?? ""
                    };

                    documents.Add(document);
                }

                await connection.CloseAsync();
            }

            return documents;
        }
    }
}
