using Microsoft.AspNetCore.Identity;

namespace Zumba.Models
{
	public class CzechIdentityErrorDescriber : IdentityErrorDescriber
	{
		public override IdentityError PasswordTooShort(int length)
		{
			return new IdentityError
			{
				Code = nameof(PasswordTooShort),
				Description = $"Hesla musí mít alespoň {length} znaků."
			};
		}

		public override IdentityError PasswordRequiresNonAlphanumeric()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresNonAlphanumeric),
				Description = "Hesla musí obsahovat alespoň jeden speciální znak."
			};
		}

		public override IdentityError PasswordRequiresDigit()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresDigit),
				Description = "Hesla musí obsahovat alespoň jednu číslici ('0'-'9')."
			};
		}

		public override IdentityError PasswordRequiresUpper()
		{
			return new IdentityError
			{
				Code = nameof(PasswordRequiresUpper),
				Description = "Hesla musí obsahovat alespoň jedno velké písmeno ('A'-'Z')."
			};
		}
	}

}
