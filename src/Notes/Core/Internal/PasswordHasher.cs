using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System;

namespace Notes.Core.Internal
{
	public class PasswordHasher
	{
		private readonly ILogger<PasswordHasher> Log;

		public PasswordHasher(
			ILogger<PasswordHasher> log)
		{
			Log = log;
		}

		public string HashPassword(string password, int iterations = 600000)
		{
			if (String.IsNullOrEmpty(password))
				throw new ArgumentNullException(nameof(password));

			return Convert.ToBase64String(HashPasswordV2(password, iterations));
		}

		public byte[] HashPasswordV2(string password, int iterations)
		{
			const int _saltSize = 128 / 8; // 128 bits / 16 bytes
			const int _hashSize = 256 / 8; // 256 bits / 32 bytes
			const int _iterSize = 32 / 8;  // 32 bits  / 4 bytes

			byte[] _saltBytes;
			byte[] _hashBytes;
			byte[] _iterBytes = BitConverter.GetBytes(iterations);
			byte[] _hashedPasswordBytes = new byte[_saltSize + _hashSize + _iterSize + 1];

			_saltBytes = RandomNumberGenerator.GetBytes(_saltSize);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(_iterBytes);

			_hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, _saltBytes, iterations, HashAlgorithmName.SHA256, _hashSize);

			_hashedPasswordBytes[0] = 0x01;
			Buffer.BlockCopy(_saltBytes, 0, _hashedPasswordBytes, 1, _saltSize);
			Buffer.BlockCopy(_hashBytes, 0, _hashedPasswordBytes, 1 + _saltSize, _hashSize);
			Buffer.BlockCopy(_iterBytes, 0, _hashedPasswordBytes, 1 + _saltSize + _hashSize, _iterSize);

			Log.LogDebug($"HashV2 Iter {iterations}");
			Log.LogDebug($"HashV2 Salt {ByteArrayToString(_saltBytes)}");
			Log.LogDebug($"HashV2 Hash {ByteArrayToString(_hashBytes)}");

			return _hashedPasswordBytes;
		}

		public bool VerifyPassword(string password, string hashedPassword)
		{
			if (String.IsNullOrEmpty(hashedPassword))
				throw new ArgumentNullException(nameof(password));

			if (String.IsNullOrEmpty(password))
				throw new ArgumentNullException(nameof(hashedPassword));

			byte[] _hashedPassword = Convert.FromBase64String(hashedPassword);

			var _version = _hashedPassword[0];
			Log.LogDebug($"Verify Vers {_version + 1}");

			return _version switch
			{
				0x00 => VerifyPasswordV1(password, _hashedPassword),
				0x01 => VerifyPasswordV2(password, _hashedPassword),
				_ => throw new Exception("Unknown hash version."),
			};
		}

		private bool VerifyPasswordV1(string password, byte[] hashedPassword)
		{
			const int _saltSize = 192 / 8; // 192 bits / 24 bytes
			const int _hashSize = 192 / 8; // 192 bits / 24 bytes
			const int _iterations = 10101;

			byte[] _hashedPasswordBytes;
			byte[] _saltBytes = new byte[_saltSize];
			byte[] _hashBytes = new byte[_hashSize];
			int _arrayLen = _saltSize + _hashSize + 1;

			if (hashedPassword.Length != _arrayLen)
				return false;

			Buffer.BlockCopy(hashedPassword, 1, _saltBytes, 0, _saltSize);
			Buffer.BlockCopy(hashedPassword, _saltSize + 1, _hashBytes, 0, _hashSize);

			Log.LogDebug($"VerifyV1 Iter {_iterations}");
			Log.LogDebug($"VerifyV1 Salt {ByteArrayToString(_saltBytes)}");
			Log.LogDebug($"VerifyV1 Hash {ByteArrayToString(_hashBytes)}");

			_hashedPasswordBytes = Rfc2898DeriveBytes.Pbkdf2(password, _saltBytes, _iterations, HashAlgorithmName.SHA1, _hashSize);

			Log.LogDebug($"VerifyV1 Gen  {ByteArrayToString(_hashedPasswordBytes)}");

			return AreHashesEqual(_hashBytes, _hashedPasswordBytes);
		}

		private bool VerifyPasswordV2(string password, byte[] hashedPassword)
		{
			const int _saltSize = 128 / 8; // 128 bits / 16 bytes
			const int _hashSize = 256 / 8; // 256 bits / 32 bytes
			const int _iterSize = 32 / 8;  // 32 bits  / 4 bytes

			byte[] _hashedPasswordBytes;
			byte[] _saltBytes = new byte[_saltSize];
			byte[] _hashBytes = new byte[_hashSize];
			byte[] _iterBytes = new byte[_iterSize];
			int _arrayLen = _saltSize + _hashSize + _iterSize + 1;

			if (hashedPassword.Length != _arrayLen)
				return false;

			Buffer.BlockCopy(hashedPassword, 1, _saltBytes, 0, _saltSize);
			Buffer.BlockCopy(hashedPassword, 1 + _saltSize, _hashBytes, 0, _hashSize);
			Buffer.BlockCopy(hashedPassword, 1 + _saltSize + _hashSize, _iterBytes, 0, _iterSize);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(_iterBytes);

			int _iterations = BitConverter.ToInt32(_iterBytes);

			Log.LogDebug($"VerifyV2 Iter {_iterations}");
			Log.LogDebug($"VerifyV2 Salt {ByteArrayToString(_saltBytes)}");
			Log.LogDebug($"VerifyV2 Hash {ByteArrayToString(_hashBytes)}");

			_hashedPasswordBytes = Rfc2898DeriveBytes.Pbkdf2(password, _saltBytes, _iterations, HashAlgorithmName.SHA256, _hashSize);

			Log.LogDebug($"VerifyV2 Gen  {ByteArrayToString(_hashedPasswordBytes)}");

			return AreHashesEqual(_hashBytes, _hashedPasswordBytes);
		}

		private bool AreHashesEqual(byte[] firstHash, byte[] secondHash)
		{
			int _minHashLength = firstHash.Length <= secondHash.Length ? firstHash.Length : secondHash.Length;
			var xor = firstHash.Length ^ secondHash.Length;
			for (int i = 0; i < _minHashLength; i++)
				xor |= firstHash[i] ^ secondHash[i];
			return 0 == xor;
		}

		private string ByteArrayToString(byte[] array)
		{
			return BitConverter.ToString(array);
		}
	}
}