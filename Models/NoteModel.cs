using MongoDB.Bson;
using System.Collections.Generic;
using System;

namespace notes.Models
{
	public class NoteModel
	{
		public ObjectId Id { get; set; }

		public string Age { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public IEnumerable<string> Tags { get; set; }

		public string TagsString { get; set; }

		public string Notebook { get; set; }

		public bool Trash { get; set; }

		public int Version { get; set; }

		public string Created { get; set; }

		public string Modified { get; set; }

		public double? Score { get; set; }
	}
}