using DocumentsAPI.Models;

namespace DocumentsAPI.Data.Repositories
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetDocuments(int skip, int take);

        Task<Document?> GetDocument(int documentId);
        
        Task<Document> AddDocument(Document document);
        
        Task<bool> DeleteDocument(int documentId);
    }
}
