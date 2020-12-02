using System.Collections.Generic;
using System.Threading.Tasks;
using notes.Core.Models;

public interface IRevisionService
{
	Task<List<Revision>> GetRevisionsAsync(int id);
	Task<Revision> GetRevisionAsync(int id);
	Task RestoreAsync(int id);
	Task<string> GetDiffAsync(int id);
}