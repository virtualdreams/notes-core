using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using notes.Core.Models;

namespace notes.Core.Services
{
	public class NoteService
	{
		private readonly ILogger<NoteService> Log;
		private readonly MySqlContext Context;

		public NoteService(ILogger<NoteService> log, MySqlContext context)
		{
			Log = log;
			Context = context;
		}

		/// <summary>
		/// Get a list of notes.
		/// </summary>
		/// <param name="next">The next id.</param>
		/// <param name="trashed">Request trashed items or not.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns>List of notes.</returns>
		public async Task<IEnumerable<Note>> GetNotes(int next, bool trashed, int limit)
		{
			Log.LogInformation($"Get all notes.");

			var _query = Context.Note
				.Where(f => f.Trash == trashed);

			if (next > 0)
			{
				_query = _query.Where(f => f.Id < next);
			}

			_query = _query
				.OrderByDescending(o => o.Id)
				.Take(limit);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get a list of notes by notebook name.
		/// </summary>
		/// <param name="notebook">The notebook name.</param>
		/// <param name="next">The next cursor.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns></returns>
		public async Task<IEnumerable<Note>> GetByNotebook(string notebook, int next, int limit)
		{
			notebook = notebook?.Trim();

			Log.LogInformation($"Get notes by notebook '{notebook}'.");

			var _query = Context.Note
				.Where(f => f.Notebook == notebook && f.Trash == false);

			if (next > 0)
			{
				_query = _query.Where(f => f.Id < next);
			}

			_query = _query
				.OrderByDescending(o => o.Id)
				.Take(limit);


			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get a list of notes by tag.
		/// </summary>
		/// <param name="tag">The tag name.</param>
		/// <param name="next">The next cursor.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns></returns>
		public async Task<IEnumerable<Note>> GetByTag(string tag, int next, int limit)
		{
			tag = tag?.Trim();

			Log.LogInformation($"Get notes by tag '{tag}'.");

			var _query = Context.Note.AsQueryable();

			if (!String.IsNullOrEmpty(tag))
			{
				_query = _query.Where(f => f.Trash == false && f.Tags.Any(a => a.Name == tag));
			}
			else
			{
				_query = _query.Where(f => f.Trash == false && !f.Tags.Any());
			}

			if (next > 0)
			{
				_query = _query.Where(f => f.Id < next);
			}

			_query = _query
				.OrderByDescending(o => o.Id)
				.Take(limit);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get a note by id.
		/// </summary>
		/// <param name="id">The note id.</param>
		/// <returns></returns>
		public async Task<Note> GetById(int id)
		{
			Log.LogInformation($"Get note by id {id}.");

			var _query = Context.Note
				.Include(i => i.Tags)
				.Where(f => f.Id == id);

			return await _query.SingleOrDefaultAsync();
		}

		/// <summary>
		/// Get mostly used notebooks.
		/// </summary>
		/// <param name="limit">Limit result to n items.</param>
		/// <returns>A list of notebook.</returns>
		public async Task<IEnumerable<DistinctAndCount>> GetMostUsedNotebooks(int limit)
		{
			Log.LogDebug($"Get the {limit} most used notebooks.");

			var _query = Context.Note
				.Where(f => f.Trash == false && f.Notebook != null)
				.GroupBy(g => g.Notebook)
				.Select(s => new DistinctAndCount
				{
					Name = s.Key,
					Count = s.Count()
				})
				.OrderByDescending(o => o.Count)
				.Take(limit);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get a list of all notebooks.
		/// </summary>
		/// <returns>A list of notebook.</returns>
		public async Task<IEnumerable<DistinctAndCount>> GetNotebooks()
		{
			Log.LogInformation($"Get the full list of notebooks.");

			var _query = Context.Note
				.Where(f => f.Trash == false && f.Notebook != null)
				.GroupBy(g => g.Notebook)
				.Select(s => new DistinctAndCount
				{
					Name = s.Key,
					Count = s.Count()
				})
				.OrderByDescending(o => o.Count);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get mostly used tags.
		/// </summary>
		/// <param name="limit">Limit result to n items.</param>
		/// <returns>A list of tags.</returns>
		public async Task<IEnumerable<DistinctAndCount>> GetMostUsedTags(int limit)
		{
			Log.LogDebug($"Get the {limit} most used tags.");

			var _query = Context.Tag
				.Where(f => f.Note.Trash == false)
				.GroupBy(g => g.Name)
				.Select(s => new DistinctAndCount
				{
					Name = s.Key,
					Count = s.Count()
				})
				.OrderByDescending(o => o.Count)
				.Take(limit);

			return await _query.ToListAsync();
		}

		// <summary>
		/// Get a list of all tags,
		/// </summary>
		/// <returns>A list of tags.</returns>
		public async Task<IEnumerable<DistinctAndCount>> GetTags()
		{
			Log.LogInformation($"Get the full list of tags.");

			var _query = Context.Tag
				.Where(f => f.Note.Trash == false)
				.GroupBy(g => g.Name)
				.Select(s => new DistinctAndCount
				{
					Name = s.Key,
					Count = s.Count()
				})
				.OrderByDescending(o => o.Count);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Create a new note.
		/// </summary>
		/// <param name="title">The note title.</param>
		/// <param name="content">The note content.</param>
		/// <param name="notebook">The notebook name.</param>
		/// <param name="tags">The note tags.</param>
		/// <returns>The created note.</returns>
		public async Task<Note> Create(string title, string content, string notebook, string tags)
		{
			var _tags = tags?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(s => !String.IsNullOrEmpty(s))
				.Distinct()
				.ToArray();

			var _note = new Note
			{
				Title = title?.Trim(),
				Content = content?.Trim(),
				Notebook = notebook?.Trim(),
				Tags = _tags?.Select(t => new Tag { Name = t }).ToList(),
				Trash = false,
				Created = DateTime.UtcNow
			};

			Context.Add(_note);

			await Context.SaveChangesAsync();

			Log.LogInformation($"Create new note with id {_note.Id}.");

			return _note;
		}

		/// <summary>
		/// Update a note.
		/// </summary>
		/// <param name="id">The note id.</param>
		/// <param name="title">The note title.</param>
		/// <param name="content">The note content.</param>
		/// <param name="notebook">The note notebook.</param>
		/// <param name="tags">The note tags.</param>
		/// <returns></returns>
		public async Task Update(int id, string title, string content, string notebook, string tags)
		{
			var _tags = tags?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(s => !String.IsNullOrEmpty(s))
				.Distinct()
				.ToArray();

			var _note = await GetById(id);
			if (_note == null)
				throw new NotesNoteNotFoundException();

			_note.Title = title?.Trim();
			_note.Content = content?.Trim();
			_note.Notebook = notebook?.Trim();

			var _tagsEqual = _tags != null ? _note.Tags.Select(s => s.Name).SequenceEqual(_tags) : false;
			Log.LogInformation($"Tags update needed: {!_tagsEqual}");

			if (!_tagsEqual)
			{
				_note.Tags.Clear();
				_note.Tags = _tags?.Select(t => new Tag { Name = t }).ToList();
			}

			_note.Modified = DateTime.UtcNow;

			await Context.SaveChangesAsync();

			Log.LogInformation($"Update note {id}.");
		}

		/// <summary>
		/// Toggle trash status flag.
		/// </summary>
		/// <param name="id">The note id.</param>
		/// <param name="trash">Set trash flag for the note.</param>
		public async Task Trash(int id, bool trash)
		{
			var _note = await GetById(id);
			if (_note == null)
				throw new NotesNoteNotFoundException();

			_note.Trash = trash;

			Log.LogInformation($"Mark note {id} as deleted (Value: {trash}).");

			await Context.SaveChangesAsync();
		}

		/// <summary>
		/// Remove the note permanently.
		/// </summary>
		/// <param name="id">The note id.</param>
		public async Task Delete(int id)
		{
			var _note = await GetById(id);
			if (_note == null)
				throw new NotesNoteNotFoundException();

			_note.Tags.Clear();

			Context.Note.Remove(_note);

			Log.LogInformation($"Delete note {id} permanently.");

			await Context.SaveChangesAsync();
		}

		/// <summary>
		/// Search for a term.
		/// </summary>
		/// <param name="term">The term to search for.</param>
		/// <param name="next">The next cursor.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns></returns>
		public async Task<IEnumerable<Note>> Search(string term, int next, int limit)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term))
			{
				return Enumerable.Empty<Note>();
			}

			var _query = Context.Note.AsQueryable();

			if (next > 0)
			{
				_query = _query.FromSql($@"
					select
						f.id,
						f.content,
						f.created,
						f.modified,
						f.notebook,
						f.title,
						f.trash
					from
						note as f 
					left join
						tag as a 
					on
						a.noteid = f.id
					where
						f.trash = {false} and
						f.id < {next} and
						(
							match(f.title, f.content, f.notebook) against({term} in natural language mode) or
							match(a.name) against({term} in natural language mode)
						)
					group by
						f.id
					order by
						f.id desc
					limit {limit};
				");
			}
			else
			{
				_query = _query.FromSql($@"
					select
						f.id,
						f.content,
						f.created,
						f.modified,
						f.notebook,
						f.title,
						f.trash
					from
						note as f 
					left join
						tag as a 
					on
						a.noteid = f.id
					where
						f.trash = {false} and
						(
							match(f.title, f.content, f.notebook) against({term} in natural language mode) or
							match(a.name) against({term} in natural language mode)
						)
					group by
						f.id
					order by
						f.id desc
					limit {limit};
				");
			}

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get suggestions for tags.
		/// </summary>
		/// <param name="term">The term to search for.</param>
		/// <returns></returns>
		public async Task<IEnumerable<string>> TagSuggestions(string term)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term) || term.Length < 3)
				return Enumerable.Empty<string>();

			Log.LogDebug($"Get tag suggestions for term '{term}'.");

			var _result = await GetTags();

			return _result.Select(s => s.Name).Where(w => w.IndexOf(term, StringComparison.OrdinalIgnoreCase) != -1);
		}

		/// <summary>
		/// Get suggestions for notebooks.
		/// </summary>
		/// <param name="term">The term to search for.</param>
		/// <returns></returns>
		public async Task<IEnumerable<string>> NotebookSuggestions(string term)
		{
			term = term?.Trim();

			if (String.IsNullOrEmpty(term) || term.Length < 3)
				return Enumerable.Empty<string>();

			Log.LogDebug($"Get notebook suggestions for term '{term}'.");

			var _result = await GetNotebooks();

			return _result.Select(s => s.Name).Where(w => w.IndexOf(term, StringComparison.OrdinalIgnoreCase) != -1);
		}
	}
}