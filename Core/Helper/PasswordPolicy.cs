using System.Text.RegularExpressions;

namespace notes.Helper
{
	/// <summary>
	/// Simple password policy.
	/// </summary>
	public class PasswordPolicy
	{
		/// <summary>
		/// Minimum password length.
		/// </summary>
		public int MinLength = 8;

		/// <summary>
		/// Minimum upper case length.
		/// </summary>
		public int UpperCaseLength = 1;

		/// <summary>
		/// Minimum lower case length.
		/// </summary>
		public int LowerCaseLength = 1;

		/// <summary>
		/// Minimum non alpha-numeric characters length.
		/// </summary>
		public int NonAlphaLength = 1;

		/// <summary>
		/// Minimum digits length.
		/// </summary>
		public int DigitsLength = 1;

		/// <summary>
		/// Test if the password is valid.
		/// </summary>
		/// <param name="password">The password to validate.</param>
		/// <returns>True on success.</returns>
		public bool IsValid(string password)
		{
			if (password.Length < MinLength)
				return false;

			if (UpperCaseCount(password) < UpperCaseLength)
				return false;

			if (LowerCaseCount(password) < LowerCaseLength)
				return false;

			if (NonAlphaCount(password) < NonAlphaLength)
				return false;

			if (DigitsCount(password) < DigitsLength)
				return false;

			return true;
		}

		private int UpperCaseCount(string paswword)
		{
			return Regex.Matches(paswword, "[A-Z]").Count;
		}

		private int LowerCaseCount(string password)
		{
			return Regex.Matches(password, "[a-z]").Count;
		}

		private int DigitsCount(string password)
		{
			return Regex.Matches(password, "[0-9]").Count;
		}

		private int NonAlphaCount(string password)
		{
			return Regex.Matches(password, @"[^0-9a-zA-Z\._]").Count;
		}
	}
}