using FluentValidation;
using System;
using System.Collections.Generic;

namespace notes.Validators
{
	public static class CustomValidators
	{
		public static IRuleBuilderOptions<T, string> MaximumLengthInArrayString<T>(this IRuleBuilder<T, string> ruleBuilder, int length, char separator)
		{
			return ruleBuilder.Must(str =>
			{
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
			});
		}

		public static IRuleBuilderOptions<T, IList<TElement>> ListMustNotEmpty<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder)
		{
			return ruleBuilder.Must(list => list.Count != 0);
		}
	}
}