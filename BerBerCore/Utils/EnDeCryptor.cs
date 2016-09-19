using System;
using System.Collections.Generic;


namespace BerBerCore
{
	public class EnDeCryptor
	{
		public EnDeCryptor ()
		{
		}

		public const string PARTIAL_KEY = "amjJ3ahGh46daF126g";

		public static string TimeEncrypt_UrlEncoded(string plainText)
		{
			string key = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss") + PARTIAL_KEY;
			string encryptedStr = EncryptAES(plainText, key, PARTIAL_KEY.Remove(16));//IV);

			return System.Web.HttpUtility.UrlEncode(encryptedStr);//encryptedStr;//
		}
		public static string TimeEncrypt(string plainText)
		{
			string key = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss") + PARTIAL_KEY;
			string encryptedStr = EncryptAES(plainText, key, PARTIAL_KEY.Remove(16));//IV);

			return encryptedStr;//
		}
		public static List<string> TimeDecrypt(string cipherText, int codeCount)
		{
			List<string> result = new List<string>();
			DateTime refDate = DateTime.Now.ToUniversalTime();
			string tempStr = null;
			string tempDate = null;
			for (int i = -codeCount; i <= codeCount; i++)
			{
				tempDate = refDate.AddSeconds(i).ToString("yyyyMMddHHmmss") + PARTIAL_KEY;
				tempStr = DecryptAES(cipherText, tempDate, PARTIAL_KEY.Remove(16));//IV);

				if (tempStr != string.Empty)
				{
					result.Add(tempStr);
				}
			}

			return result;
		}
		public static List<DecryptedResult> TimeDecrypt_ForTest(string cipherText, int codeCount)
		{
			List<DecryptedResult> result = new List<DecryptedResult>();
			DecryptedResult tempResult = default(DecryptedResult);
			DateTime refDate = DateTime.Now.ToUniversalTime();
			string tempStr = null;
			string tempDate = null;
			for (int i = -codeCount; i <= codeCount; i++)
			{
				tempDate = refDate.AddSeconds(i).ToString("yyyyMMddHHmmss") + PARTIAL_KEY;
				tempStr = DecryptAES(cipherText, tempDate, PARTIAL_KEY.Remove(16));//IV);

				if (tempStr != string.Empty)
				{
					tempResult = new DecryptedResult(tempStr, i, tempDate);//getMD5(tempDate));
					result.Add(tempResult);
				}
			}

			return result;
		}

		private static string EncryptAES(string msg, string key, string iv)
		{
			string resutlt = string.Empty;

			using (var myAes = System.Security.Cryptography.Aes.Create())
			{
				myAes.Key = System.Text.Encoding.UTF8.GetBytes(key);
				myAes.IV = System.Text.Encoding.UTF8.GetBytes(iv);

				try
				{
					byte[] encrypted = EncryptStringToBytes_Aes(msg, myAes.Key, myAes.IV);
					resutlt = Convert.ToBase64String(encrypted);
				}
				catch
				{

				}

			}

			return resutlt;
		}
		private static string DecryptAES(string msg, string key, string iv)
		{
			string result = string.Empty;
			byte[] arrMsg = Convert.FromBase64String(msg);

			using (var myAes = System.Security.Cryptography.Aes.Create())
			{
				myAes.Key = System.Text.Encoding.UTF8.GetBytes(key);
				myAes.IV = System.Text.Encoding.UTF8.GetBytes(iv);

				try
				{
					result = DecryptStringFromBytes_Aes(arrMsg, myAes.Key, myAes.IV);
				}
				catch
				{

				}

			}


			return result;
		}

		private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
		{
			// Check arguments. 
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("Key");
			byte[] encrypted;
			// Create an AesCryptoServiceProvider object 
			// with the specified key and IV. 
			using (var aesAlg = System.Security.Cryptography.Aes.Create())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				System.Security.Cryptography.ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption. 
				using (System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream())
				{
					using (System.Security.Cryptography.CryptoStream csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
					{
						using (System.IO.StreamWriter swEncrypt = new System.IO.StreamWriter(csEncrypt))
						{

							//Write all data to the stream.
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}


			// Return the encrypted bytes from the memory stream. 
			return encrypted;

		}

		private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
		{
			// Check arguments. 
			if (cipherText == null || cipherText.Length <= 0)
				throw new ArgumentNullException("cipherText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("IV");

			// Declare the string used to hold 
			// the decrypted text. 
			string plaintext = null;

			// Create an AesCryptoServiceProvider object 
			// with the specified key and IV. 
			using (var aesAlg = System.Security.Cryptography.Aes.Create())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				System.Security.Cryptography.ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for decryption. 
				using (System.IO.MemoryStream msDecrypt = new System.IO.MemoryStream(cipherText))
				{
					using (System.Security.Cryptography.CryptoStream csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
					{
						using (System.IO.StreamReader srDecrypt = new System.IO.StreamReader(csDecrypt))
						{

							// Read the decrypted bytes from the decrypting stream 
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
						}
					}
				}

			}

			return plaintext;

		}

		//==
		public static string EncryptMD5(string str)
		{
			byte[] data = System.Text.Encoding.ASCII.GetBytes(str);
			System.Security.Cryptography.MD5CryptoServiceProvider md5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
			return System.Convert.ToBase64String(md5Provider.ComputeHash(data));
		}

	}



	public class DecryptedResult
	{
		public string result = string.Empty;
		public int idx = -1;
		public string key = string.Empty;

		public DecryptedResult(string result, int idx, string key)
		{
			this.result = result;
			this.idx = idx;
			this.key = key;
		}

	}

}

