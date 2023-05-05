using DocumentsAPI.Data.Repositories;
using DocumentsAPI.Models;
using DocumentsAPI.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADOController : ControllerBase
    {
        private readonly IDocumentRepository _repository;


        public ADOController(ADODocumentRepository repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// Add new document
        /// </summary>
        /// <param name="addDocumentRequest"></param>
        /// <returns>Added document</returns>
        /// <response code="200">Success</response>
        [HttpPost]
        [ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(AddDocumentRequest addDocumentRequest)
        {
            var document = await _repository.AddDocument(new Document { Amount = addDocumentRequest.Amount, Description = addDocumentRequest.Description });

            return Ok(document);
        }


        /// <summary>
        /// Delete document by id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="404">Document not found</response>
        [HttpDelete("{documentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int documentId)
        {
            if (!await _repository.DeleteDocument(documentId))
            {
                return NotFound();
            }

            return Ok();
        }


        /// <summary>
        /// Get existing documents
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>List of documents</returns>
        /// <response code="200">Returns list of documents</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Document>> GetAll(int skip = 0, int take = 10)
        {
            return await _repository.GetDocuments(skip, take);
        }


        /// <summary>
        /// Get document by id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns>Document</returns>
        /// <response code="200">Returns found document</response>
        /// <response code="404">Document not found</response>
        [HttpGet("{documentId}")]
        [ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int documentId)
        {
            var document = await _repository.GetDocument(documentId);

            if (document == null)
            {
                return NotFound();
            }

            return Ok(document);
        }

    }
}
