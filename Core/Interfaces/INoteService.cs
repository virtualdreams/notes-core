using Notes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Core.Interfaces
{
	public interface INoteService
	{
		Task<List<Note>> GetNotesAsync(int next, int limit);
		Task<List<Note>> GetByNotebookAsync(string notebook, int next, int limit);
		Task<List<Note>> GetByTagAsync(string tag, int next, int limit);
		Task<List<Note>> GetDeletedNotes(int next, int limit);
		Task<Note> GetByIdAsync(int id);
		Task<List<DistinctAndCount>> GetMostUsedNotebooksAsync(int limit);
		Task<List<DistinctAndCount>> GetNotebooksAsync();
		Task<List<DistinctAndCount>> GetMostUsedTagsAsync(int limit);
		Task<List<DistinctAndCount>> GetTagsAsync();
		Task<Note> CreateAsync(string title, string content, string notebook, string tags);
		Task UpdateAsync(int id, string title, string content, string notebook, string tags);
		Task TrashAsync(int id, bool trash);
		Task DeleteAsync(int id);
	}
}