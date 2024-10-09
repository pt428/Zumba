using System.Numerics;

namespace Zumba.StaticClass
{
	public static class IbanGenerator
	{
		public static string GenerateCzechIban(string accountNumber, string bankCode)
		{
			// Pad the account number with leading zeros to make it 16 digits
			string paddedAccountNumber = accountNumber.PadLeft(16, '0');

			// Construct the BBAN (Bank Code + Padded Account Number)
			string bban = bankCode + paddedAccountNumber;

			// Start with the country code and two placeholder check digits
			string countryCode = "CZ";
			string placeholderCheckDigits = "00";

			// Move the country code and placeholder check digits to the end of the BBAN
			string bbanWithCountry = bban + countryCode + placeholderCheckDigits;

			// Convert letters to numbers where A=10, B=11, ..., Z=35
			string numericBbanWithCountry = ConvertLettersToNumbers(bbanWithCountry);

			// Calculate the check digits
			int checkDigits = CalculateCheckDigits(numericBbanWithCountry);

			// Combine to form the IBAN
			string iban = countryCode + checkDigits.ToString("D2") + bban;
			return iban;
		}

		private static string ConvertLettersToNumbers(string input)
		{
			string result = string.Empty;
			foreach (char c in input)
			{
				if (char.IsLetter(c))
				{
					int value = c - 'A' + 10;
					result += value.ToString();
				}
				else
				{
					result += c;
				}
			}
			return result;
		}

		private static int CalculateCheckDigits(string numericInput)
		{
			BigInteger bigInt = BigInteger.Parse(numericInput);
			BigInteger remainder = bigInt % 97;
			int checkDigits = 98 - (int)remainder;
			return checkDigits;
		}


	}
}
