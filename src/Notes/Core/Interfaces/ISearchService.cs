using Notes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Core.Interfaces
{
	public interface ISearchService
	{
		Task<IList<Note>> SearchAsync(string term, int next, int limit);
		Task<IList<string>> NotebookSuggestionsAsync(string term);
		Task<IList<string>> TagSuggestionsAsync(string term);
	}
}