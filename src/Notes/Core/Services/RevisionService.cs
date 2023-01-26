using DiffPlex.DiffBuilder.Model;
using DiffPlex.DiffBuilder;
using DiffPlex;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Core.Data;
using Notes.Core.Interfaces;
using Notes.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Core.Services
{
	public class RevisionService : IRevisionService
	{
		private readonly ILogger<NoteService> Log;

		private readonly DatabaseContext Context;

		private readonly INoteService NoteService;

		public RevisionService(
			ILogger<NoteService> log,
			DatabaseContext context,
			INoteService note)
		{
			Log = log;
			Context = context;
			NoteService = note;
		}

		/// <summary>
		/// Get all revisions for note.
		/// </summary>
		/// <param name="id">The note id.</param>
		/// <returns>List of revisions.</returns>
		public async Task<IList<Revision>> GetRevisionsAsync(int id)
		{
			Log.LogInformation($"Get revisions for note {id}.");

			var _query = Context.Revision
				.AsNoTracking()
				.Where(f => f.NoteId == id)
				.OrderByDescending(o => o.Dt);

			return await _query.ToListAsync();
		}

		/// <summary>
		/// Get a specific revision.
		/// </summary>
		/// <param name="id">The revision id.</param>
		/// <returns></returns>
		public async Task<Revision> GetRevisionAsync(int id)
		{
			Log.LogInformation($"Get revison {id}.");

			var _query = Context.Revision
				.AsNoTracking()
				.Where(f => f.Id == id);

			return await _query.SingleOrDefaultAsync();
		}

		/// <summary>
		/// Restore revision.
		/// </summary>
		/// <param name="id">The revision id.</param>
		/// <returns></returns>
		public async Task RestoreAsync(int id)
		{
			var _revision = await GetRevisionAsync(id);
			if (_revision == null)
				throw new NotesRevisionNotFoundException();

			var _note = await NoteService.GetByIdAsync(_revision.NoteId);
			if (_note == null)
				throw new NotesNoteNotFoundException();

			_note.Title = _revision.Title;
			_note.Content = _revision.Content;
			_note.Notebook = _revision.Notebook;
			_note.Trash = _revision.Trash;
			_note.Created = _revision.Created;
			_note.Modified = _revision.Modified;

			await Context.SaveChangesAsync();

			Log.LogInformation($"Restore revision {_revision.Id} to note {_revision.NoteId}.");
		}

		/// <summary>
		/// Get diff via DiffPlex
		/// </summary>
		/// <param name="id">The revision id.</param>
		/// <returns>The diff.</returns>
		public async Task<string> GetDiffAsync(int id)
		{
			var _revision = await GetRevisionAsync(id);
			if (_revision == null)
				throw new NotesRevisionNotFoundException();

			var _note = await NoteService.GetByIdAsync(_revision.NoteId);
			if (_revision == null)
				throw new NotesNoteNotFoundException();

			Log.LogInformation($"Create a diff from revision {_revision.Id}.");

			var _diffBuilder = new InlineDiffBuilder(new Differ());
			var _diff = _diffBuilder.BuildDiffModel(_revision.Content, _note.Content);
			var _out = new StringBuilder();

			_out.AppendLine($"--- Revision {_revision.Id}");
			_out.AppendLine($"+++ Current");
			_out.AppendLine();

			foreach (var _line in _diff.Lines)
			{
				switch (_line.Type)
				{
					case ChangeType.Inserted:
						_out.AppendLine($"+  {_line.Text}");
						break;

					case ChangeType.Deleted:
						_out.AppendLine($"-  {_line.Text}");
						break;

					case ChangeType.Unchanged:
						_out.AppendLine($"   {_line.Text}");
						break;
				}
			}

			return _out.ToString();
		}
	}
}