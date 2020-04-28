using System.Text.RegularExpressions;
using System;

namespace notes.Core.Internal
{
	/// <summary>
	/// Simple password policy.
	/// </summary>
	public class PasswordPolicy
	{
		/// <summary>
		/// Minimum password length.
		/// </summary>
		public int MinLength { get; set; } = 8;

		/// <summary>
		/// Minimum upper case characters.
		/// </summary>
		public int MinimumUpperCaseCharacters { get; set; } = 1;

		/// <summary>
		/// Minimum lower case characters.
		/// </summary>
		public int MinimumLowerCaseCharacters { get; set; } = 1;

		/// <summary>
		/// Minimum non alpha-numeric characters.
		/// </summary>
		public int MinimumNonAlphaCharacters { get; set; } = 1;

		/// <summary>
		/// Minimum digits characters.
		/// </summary>
		public int MinimumDigitCharacters { get; set; } = 1;

		/// <summary>
		/// Test if the password is valid.
		/// </summary>
		/// <param name="password">The password to validate.</param>
		/// <returns>True on success.</returns>
		public bool IsValid(string password)
		{
			if (String.IsNullOrEmpty(password))
				return false;

			if (password.Length < MinLength)
				return false;

			if (UpperCaseCount(password) < MinimumUpperCaseCharacters)
				return false;

			if (LowerCaseCount(password) < MinimumLowerCaseCharacters)
				return false;

			if (NonAlphaCount(password) < MinimumNonAlphaCharacters)
				return false;

			if (DigitsCount(password) < MinimumDigitCharacters)
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