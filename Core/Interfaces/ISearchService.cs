using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Core.Models;

namespace notes.Core.Interfaces
{
	public interface ISearchService
	{
		Task<List<Note>> SearchAsync(string term, int next, int limit);
		Task<List<string>> NotebookSuggestionsAsync(string term);
		Task<List<string>> TagSuggestionsAsync(string term);
	}
}