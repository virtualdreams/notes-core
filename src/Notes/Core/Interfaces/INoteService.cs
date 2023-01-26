using Notes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Core.Interfaces
{
	public interface INoteService
	{
		Task<IList<Note>> GetNotesAsync(int next, int limit);
		Task<IList<Note>> GetByNotebookAsync(string notebook, int next, int limit);
		Task<IList<Note>> GetByTagAsync(string tag, int next, int limit);
		Task<IList<Note>> GetDeletedNotes(int next, int limit);
		Task<Note> GetByIdAsync(int id);
		Task<IList<DistinctAndCount>> GetMostUsedNotebooksAsync(int limit);
		Task<IList<DistinctAndCount>> GetNotebooksAsync();
		Task<IList<DistinctAndCount>> GetMostUsedTagsAsync(int limit);
		Task<IList<DistinctAndCount>> GetTagsAsync();
		Task<Note> CreateAsync(string title, string content, string notebook, string tags);
		Task UpdateAsync(int id, string title, string content, string notebook, string tags);
		Task TrashAsync(int id, bool trash);
		Task DeleteAsync(int id);
	}
}