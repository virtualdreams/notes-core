using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Core.Models;

public interface INoteService
{
	Task<List<Note>> GetNotesAsync(int next, bool trashed, int limit);
	Task<List<Note>> GetByNotebookAsync(string notebook, int next, int limit);
	Task<List<Note>> GetByTagAsync(string tag, int next, int limit);
	Task<Note> GetByIdAsync(int id);
	Task<List<DistinctAndCount>> GetMostUsedNotebooksAsync(int limit);
	Task<List<DistinctAndCount>> GetNotebooksAsync();
	Task<List<DistinctAndCount>> GetMostUsedTagsAsync(int limit);
	Task<List<DistinctAndCount>> GetTagsAsync();
	Task<Note> CreateAsync(string title, string content, string notebook, string tags);
	Task UpdateAsync(int id, string title, string content, string notebook, string tags);
	Task TrashAsync(int id, bool trash);
	Task DeleteAsync(int id);
	Task<List<Note>> SearchAsync(string term, int next, int limit);
	Task<List<string>> TagSuggestionsAsync(string term);
	Task<List<string>> NotebookSuggestionsAsync(string term);
}