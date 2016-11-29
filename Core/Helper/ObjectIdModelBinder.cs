using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;

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
			}

			ObjectId _id;
			if(!ObjectId.TryParse((string)result.ConvertTo(typeof(string)), out _id))
			{
				bindingContext.Result = ModelBindingResult.Success(ObjectId.Empty);
			}

			bindingContext.Result = ModelBindingResult.Success(_id);
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

			return null;
        }
    }
}