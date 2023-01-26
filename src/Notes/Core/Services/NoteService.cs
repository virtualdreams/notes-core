using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Core.Data;
using Notes.Core.Extensions;
using Notes.Core.Interfaces;
using Notes.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Notes.Core.Services
{
	public class NoteService : INoteService
	{
		private readonly ILogger<NoteService> Log;

		private readonly DataContext Context;

		public NoteService(
			ILogger<NoteService> log,
			DataContext context)
		{
			Log = log;
			Context = context;
		}

		/// <summary>
		/// Get a list of notes.
		/// </summary>
		/// <param name="next">The next id.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns>List of notes.</returns>
		public async Task<List<Note>> GetNotesAsync(int next, int limit)
		{
			Log.LogInformation($"Get all notes.");

			var _query = Context.Note
				.AsNoTracking()
				.Where(f => f.Trash == false)
				.WhereIf(next > 0, f => f.Id < next)
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
		public async Task<List<Note>> GetByNotebookAsync(string notebook, int next, int limit)
		{
			notebook = notebook?.Trim();

			Log.LogInformation($"Get notes by notebook '{notebook}'.");

			var _query = Context.Note
				.AsNoTracking()
				.Where(f => f.Notebook == notebook && f.Trash == false)
				.WhereIf(next > 0, f => f.Id < next)
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
		public async Task<List<Note>> GetByTagAsync(string tag, int next, int limit)
		{
			tag = tag?.Trim();

			Log.LogInformation($"Get notes by tag '{tag}'.");

			var _query = Context.Note
				.AsNoTracking()
				.AsQueryable()
				.WhereIfElse(
					!String.IsNullOrEmpty(tag),
					f => f.Trash == false && f.Tags.Any(a => a.Name == tag),
					f => f.Trash == false && !f.Tags.Any())
				.WhereIf(next > 0, f => f.Id < next)
				.OrderByDescending(o => o.Id)
				.Take(limit);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get a list of deleted notes.
		/// </summary>
		/// <param name="next">The next id.</param>
		/// <param name="limit">Limit the result.</param>
		/// <returns>List of notes.</returns>
		public async Task<List<Note>> GetDeletedNotes(int next, int limit)
		{
			Log.LogInformation($"Get all notes.");

			var _query = Context.Note
				.AsNoTracking()
				.Where(f => f.Trash == true)
				.WhereIf(next > 0, f => f.Id < next)
				.OrderByDescending(o => o.Id)
				.Take(limit);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get a note by id.
		/// </summary>
		/// <param name="id">The note id.</param>
		/// <returns></returns>
		public async Task<Note> GetByIdAsync(int id)
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
		public async Task<List<DistinctAndCount>> GetMostUsedNotebooksAsync(int limit)
		{
			Log.LogDebug($"Get the {limit} most used notebooks.");

			var _query = Context.Note
				.AsNoTracking()
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
		public async Task<List<DistinctAndCount>> GetNotebooksAsync()
		{
			Log.LogInformation($"Get the full list of notebooks.");

			var _query = Context.Note
				.AsNoTracking()
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
		public async Task<List<DistinctAndCount>> GetMostUsedTagsAsync(int limit)
		{
			Log.LogDebug($"Get the {limit} most used tags.");

			var _query = Context.Tag
				.AsNoTracking()
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
		public async Task<List<DistinctAndCount>> GetTagsAsync()
		{
			Log.LogInformation($"Get the full list of tags.");

			var _query = Context.Tag
				.AsNoTracking()
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
		public async Task<Note> CreateAsync(string title, string content, string notebook, string tags)
		{
			var _tags = tags?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(s => !String.IsNullOrEmpty(s))
				.Distinct()
				.ToArray();

			var _dt = DateTime.UtcNow;

			var _note = new Note
			{
				Title = title?.Trim(),
				Content = content?.Trim(),
				Notebook = notebook?.Trim(),
				Tags = _tags?.Select(t => new Tag { Name = t }).ToList(),
				Trash = false,
				Created = _dt,
				Modified = _dt
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
		public async Task UpdateAsync(int id, string title, string content, string notebook, string tags)
		{
			var _tags = tags?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(s => !String.IsNullOrEmpty(s))
				.Distinct()
				.ToArray();

			var _note = await GetByIdAsync(id);
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
		public async Task TrashAsync(int id, bool trash)
		{
			var _note = await GetByIdAsync(id);
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
		public async Task DeleteAsync(int id)
		{
			var _note = await GetByIdAsync(id);
			if (_note == null)
				throw new NotesNoteNotFoundException();

			_note.Tags.Clear();

			Context.Note.Remove(_note);

			Log.LogInformation($"Delete note {id} permanently.");

			await Context.SaveChangesAsync();
		}
	}
}