using FluentValidation;
using notes.Core.Internal;
using System;
using System.Collections.Generic;

namespace notes.Validators
{
	public static class CustomValidators
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

		public static IRuleBuilderOptions<T, IList<TElement>> ListMustNotEmpty<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder)
		{
			return ruleBuilder
				.Must((objectRoot, list, context) =>
				{
					return list.Count != 0;
				})
				.WithMessage("List must not empty.");
		}

		public static IRuleBuilderOptions<T, string> PasswordPolicy<T>(this IRuleBuilder<T, string> ruleBuilder, PasswordPolicy policy)
		{
			return ruleBuilder
				.Must((objectRoot, str, context) =>
				{
					return policy.IsValid(str);
				})
				.WithMessage("The password does not meet the password policy requirements.");
		}
	}
}