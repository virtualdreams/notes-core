using System;
using System.Security.Cryptography;
using Base62;

namespace notes.Helper
{
	public class ResetToken
	{
		private byte[] _token = null;

		private ResetToken(byte[] bytes)
		{
			if(bytes == null)
				throw new ArgumentNullException(nameof(bytes));

			_token = bytes;
		}

		public ResetToken(string token)
		{
			if(String.IsNullOrEmpty(token))
				throw new ArgumentNullException(nameof(token));

			try
			{
				_token = token.FromBase62();
			}
			catch(FormatException)
			{
				throw new NotesInvalidTokenException();
			}
		}

		/// <summary>
		/// Create a random token.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		static public ResetToken CreateNew(uint length = 60)
		{
			if(length <= 0)
				throw new ArgumentOutOfRangeException("length", "Value must be greater than zero.");

			var buffer = new byte[length];
			var rng = RandomNumberGenerator.Create();
			rng.GetBytes(buffer);

			return new ResetToken(buffer);
		}

		/// <summary>
		/// Return a sha512 hashed token.
		/// </summary>
		/// <returns>Return a sha512 hashed token.</returns>
		public string ToHash()
		{
			var sha = System.Security.Cryptography.SHA512.Create();
			return sha.ComputeHash(_token).ToBase62();
		}

		/// <summary>
		/// Return a base64 token.
		/// </summary>
		/// <returns>Return a base64 token.</returns>
		public string ToBase62String()
		{
			return _token.ToBase62();
		}
	}
}