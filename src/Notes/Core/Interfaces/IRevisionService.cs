using Notes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Core.Interfaces
{
	public interface IRevisionService
	{
		Task<IList<Revision>> GetRevisionsAsync(int id);
		Task<Revision> GetRevisionAsync(int id);
		Task RestoreAsync(int id);
		Task<string> GetDiffAsync(int id);
	}
}