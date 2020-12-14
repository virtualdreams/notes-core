using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Core.Models;

public interface ISearchService
{
	Task<List<Note>> SearchAsync(string term, int next, int limit);
}
