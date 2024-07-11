using LibraryManagementSystem.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace LibraryManagementSystem.ModelBinders
{
    public class BookModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Metadata.ModelType == typeof(BookDTO))
            {
                return new BinderTypeModelBinder(typeof(BookModelBinder));
            }
            return null;
        }
    }
}
