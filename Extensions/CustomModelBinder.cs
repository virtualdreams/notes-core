using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using System.Globalization;
using System.Threading.Tasks;
using System;
using notes.Helper;

namespace notes.ModelBinders
{
    public class ObjectIdModelBinder : IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if(result == null)
			{
				bindingContext.Result = ModelBindingResult.Success(ObjectId.Empty);
				return Task.CompletedTask;
			}

			ObjectId _id;
			if(!ObjectId.TryParse(((string)result.ConvertTo(typeof(string))).GetLast(24), out _id))
			{
				bindingContext.Result = ModelBindingResult.Success(ObjectId.Empty);
				return Task.CompletedTask;
			}

			bindingContext.Result = ModelBindingResult.Success(_id);
			return Task.CompletedTask;
        }
    }

    public class DateTimeModelBinder : IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
        	var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if(result == null)
			{
				bindingContext.Result = ModelBindingResult.Success(null);
				return Task.CompletedTask;
			}

			DateTime _dt;
			if(!DateTime.TryParse((string)result, CultureInfo.InvariantCulture, DateTimeStyles.None, out _dt))
			{
				bindingContext.Result = ModelBindingResult.Success(null);
				return Task.CompletedTask;
			}
		
			bindingContext.Result = ModelBindingResult.Success(_dt);
			return Task.CompletedTask;
		}
    }

    public class CustomModelBinderProvider : IModelBinderProvider
    {
        IModelBinder IModelBinderProvider.GetBinder(ModelBinderProviderContext context)
        {
            if(context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			var type = context.Metadata.ModelType;
			if(type == typeof(ObjectId))
			{
				return new ObjectIdModelBinder();
			}

			if(type == typeof(DateTime?))
			{
				return new DateTimeModelBinder();
			}

			return null;
        }
    }
}