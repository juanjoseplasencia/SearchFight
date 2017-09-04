
namespace SearchLibrary
{
    public interface ISearchEngine
    {
        string Name { get; }
        long Search(string searchTerm);
    }
}
