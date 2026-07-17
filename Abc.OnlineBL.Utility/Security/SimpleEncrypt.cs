using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Abc.OnlineBL.Utility.Security
{
	/// <summary>
	/// This class will use an encryption/decryption algorythm DES
	/// </summary>

	public class SimpleEncrypt
	{

		public SimpleEncrypt()
		{

		}


		/// <summary>
		/// Encrypts a string
		/// </summary>
		/// <param name="originalString">the string to be encrypted</param>
		/// <param name="key">the password to use as a key</param>
		/// <returns>the encrypted string</returns>
		public static string Encrypt(string original, string key)
		{

			TripleDESCryptoServiceProvider des;
			MD5CryptoServiceProvider hashmd5;

			byte[] keyhash, buff;
			string encrypted = "";

			try
			{
				hashmd5 = new MD5CryptoServiceProvider();
				keyhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
				hashmd5 = null;
				des = new TripleDESCryptoServiceProvider();
				des.Key = keyhash;
				des.Mode = CipherMode.ECB;
				buff = ASCIIEncoding.ASCII.GetBytes(original);

				encrypted = Convert.ToBase64String(

					des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length)

					);
			}
			catch (System.Security.Cryptography.CryptographicException cex)
			{
				throw new SimpleEncryptException("Unknown Encryption Algorithm Error", cex);
			}
			catch (Exception ex)
			{
				throw new SimpleEncryptException("Unknown Encryption Error", ex);
			}

			return encrypted;
		}

		/// <summary>
		/// Decrypts a string
		/// </summary>
		/// <param name="encrypted">the encrypted string</param>
		/// <param name="key">the key used in encryption</param>
		/// <returns>the decrypted string</returns>
		public static string Decrypt(string encrypted, string key)
		{

			TripleDESCryptoServiceProvider des;
			MD5CryptoServiceProvider hashmd5;

			byte[] keyhash, buff;
			string decrypted;

			try
			{
				hashmd5 = new MD5CryptoServiceProvider();
				keyhash = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
				hashmd5 = null;
				des = new TripleDESCryptoServiceProvider();
				des.Key = keyhash;
				des.Mode = CipherMode.ECB;
				buff = Convert.FromBase64String(encrypted);

				decrypted = ASCIIEncoding.ASCII.GetString(

					des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));
			}
			catch (System.Security.Cryptography.CryptographicException cex)
			{
				throw new SimpleEncryptWrongPasswordException("Wrong Password or Bad Data to Decrypt", cex);
			}
			catch (System.FormatException fex)
			{
				throw new SimpleEncryptWrongDataException("Not a Valid BASE64 Encoded Data for Decryption", fex);
			}
			catch (Exception ex)
			{
				throw new SimpleEncryptException("Unknown Decryption Error", ex);
			}
			return decrypted;
		}

		public static string RotEncryptNumber(int number)
		{
			string txt = number.ToString();
			if (txt.Length > 0)
			{
				Char[] cc = new Char[txt.Length];
				for (int i = 0; i < txt.Length; i++)
				{
					int tmp = txt[i];
					Char tmp1 = Convert.ToChar(tmp + 49);
					cc[i] = tmp1;
				}
				Array.Reverse(cc);
				string ss = new string(cc);
				return ss;
			}
			else
			{
				throw new SimpleEncryptWrongDataException("0 Length Input Data");
			}
		}

		public static int RotDecryptNumber(string txt)
		{
			if (txt == null)
			{
				throw new SimpleEncryptWrongDataException("Null Input Data");
			}
			if (txt.Length > 0)
			{
				Char[] cc = txt.ToCharArray();
				Array.Reverse(cc);
				for (int i = 0; i < txt.Length; i++)
				{
					int tmp = cc[i];
					cc[i] = Convert.ToChar(tmp - 49);
					if (!Char.IsDigit(cc[i]))
					{
						throw new SimpleEncryptWrongDataException("Input Data Not a Valid Digit");
					}
				}
				string ss = new string(cc);
				try
				{
					return int.Parse(ss);
				}
				catch (Exception ex)
				{
					throw new SimpleEncryptWrongDataException("Input Data Not a Valid Digit");
				}
			}
			else
			{
				throw new SimpleEncryptWrongDataException("0 Length Input Data");
			}
		}
	}

	public class SimpleEncryptException : System.Exception
	{
		public SimpleEncryptException()
			: base()
		{
		}
		public SimpleEncryptException(string msg)
			: base(msg)
		{
		}
		public SimpleEncryptException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}

	public class SimpleEncryptWrongPasswordException : System.Exception
	{
		public SimpleEncryptWrongPasswordException()
			: base()
		{
		}
		public SimpleEncryptWrongPasswordException(string msg)
			: base(msg)
		{
		}
		public SimpleEncryptWrongPasswordException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}

	public class SimpleEncryptWrongDataException : System.Exception
	{
		public SimpleEncryptWrongDataException()
			: base()
		{
		}
		public SimpleEncryptWrongDataException(string msg)
			: base(msg)
		{
		}
		public SimpleEncryptWrongDataException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}

}

