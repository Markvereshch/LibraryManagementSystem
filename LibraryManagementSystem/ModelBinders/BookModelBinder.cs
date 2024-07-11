using LibraryManagementSystem.DTOs;
using LMS_Shared;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibraryManagementSystem.ModelBinders
{
    //Binder, which creates a new book object from a Url
    public class BookModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var idValue = context.ValueProvider.GetValue("id").FirstValue;
            var titleValue = context.ValueProvider.GetValue("title").FirstValue;
            var authorValue = context.ValueProvider.GetValue("author").FirstValue;
            var genreValue = context.ValueProvider.GetValue("genre").FirstValue;
            var yearValue = context.ValueProvider.GetValue("year").FirstValue;
            var statusValue = context.ValueProvider.GetValue("status").FirstValue;
            var collectionIdValue = context.ValueProvider.GetValue("collectionId").FirstValue;

            if (string.IsNullOrEmpty(idValue) ||
                string.IsNullOrEmpty(titleValue) ||
                string.IsNullOrEmpty(authorValue) ||
                string.IsNullOrEmpty(genreValue) ||
                string.IsNullOrEmpty(yearValue) ||
                string.IsNullOrEmpty(statusValue) ||
                string.IsNullOrEmpty(collectionIdValue))
            {
                context.ModelState.AddModelError("", "Some value is not valid");
                return Task.CompletedTask;
            }
            if (!Int32.TryParse(yearValue, out int year) ||
                !Int32.TryParse(idValue, out int id) ||
                !Enum.TryParse<BookStatus>(statusValue, true, out var status) ||
                !Int32.TryParse(collectionIdValue, out int collectionId))
            {
                context.ModelState.AddModelError("", "Year, id, collectionId or status is not valid");
                return Task.CompletedTask;
            }
            var result = new BookDTO
            {
                Id = id,
                Title = titleValue,
                Author = authorValue,
                Genre = genreValue,
                Year = year,
                Status = status,
                CollectionId = collectionId
            };

            context.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;
        }
    }
}

