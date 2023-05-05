using DocumentsAPI.Enums;
using DocumentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentsAPI.Data.Repositories
{
    public class EFDocumentRepository : IDocumentRepository
    {
        private readonly DocumentsContext db;


        public EFDocumentRepository(DocumentsContext appContext)
        {
            db = appContext;
        }


        public async Task<Document> AddDocument(Document document)
        {
            db.Documents.Add(document);

            await db.SaveChangesAsync();

            return document;
        }


        public async Task<bool> DeleteDocument(int documentId)
        {
            var document = await db.DocumentStatuses.FirstOrDefaultAsync(d => d.DocumentId == documentId && d.StatusId != STATUS.DELETED);

            if (document == null)
            {
                return false;
            }

            document.StatusId = STATUS.DELETED;

            await db.SaveChangesAsync();

            return true;
        }


        public async Task<Document?> GetDocument(int documentId)
        {
            return await db.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId && d.DocumentStatus.StatusId != STATUS.DELETED);
        }
        

        public async Task<IEnumerable<Document>> GetDocuments(int skip, int take)
        {
            return await db.Documents.Where(d => d.DocumentStatus.StatusId != STATUS.DELETED).OrderBy(d => d.DocumentId).Skip(skip).Take(take).ToListAsync();
        }

    }
}
