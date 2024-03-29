using FluentValidation;
using System;

namespace Notes.FluentValidation
{
	public static class FluentValidationExtensions
	{
		public static IRuleBuilderOptions<T, string> MaximumLengthInArrayString<T>(this IRuleBuilder<T, string> ruleBuilder, int length, char separator)
		{
			return ruleBuilder
				.Must((rootObject, str, context) =>
				{
					context.MessageFormatter.AppendArgument("MaxLength", length);

					if (str != null)
					{
						var list = str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
						foreach (var item in list)
						{
							if (item.Length > length)
							{
								return false;
							}
						}
					}
					return true;
				})
				.WithMessage("The length of an item of '{PropertyName}' must be {MaxLength} characters or fewer.");
		}

		public static IRuleBuilderOptions<T, string> ValidatePasswordPolicy<T>(this IRuleBuilder<T, string> ruleBuilder, IPasswordPolicy policy)
		{
			return ruleBuilder
				.Must((objectRoot, str, context) =>
				{
					return policy.IsValid(str);
				})
				.WithMessage("The '{PropertyName}' does not meet the password policy requirements.");
		}
	}
}