using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace notes.Helper
{
    public class ArrayNotEmptyAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			var _list = value as IList;
			if(_list != null)
			{
				return _list.Count > 0;
			}

			return false;
		}
	}
}