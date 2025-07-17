using WorkflowEngine.Samples.Mvc.Models;

namespace WorkflowEngine.Samples.Mvc.Data
{
    public interface IDocumentRepository
    {
        Task<List<Document>> GetAllAsync();
        Task<Document> GetByIdAsync(int id);
        Task<Document> AddAsync(Document document);
        Task UpdateAsync(Document document);
    }

    public class InMemoryDocumentRepository : IDocumentRepository
    {
        private static readonly List<Document> _documents = new();
        private static int _nextId = 1;

        public Task<List<Document>> GetAllAsync()
        {
            return Task.FromResult(_documents.ToList());
        }

        public Task<Document> GetByIdAsync(int id)
        {
            var document = _documents.FirstOrDefault(d => d.Id == id.ToString());
            return Task.FromResult(document);
        }

        public Task<Document> AddAsync(Document document)
        {
            document.Id = (_nextId++).ToString();
            _documents.Add(document);
            return Task.FromResult(document);
        }

        public Task UpdateAsync(Document document)
        {
            var index = _documents.FindIndex(d => d.Id == document.Id);
            if (index >= 0)
            {
                _documents[index] = document;
            }
            return Task.CompletedTask;
        }
    }
}