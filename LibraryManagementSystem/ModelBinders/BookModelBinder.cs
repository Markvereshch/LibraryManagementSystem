using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibraryManagementSystem.ModelBinders
{
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
            var isAssignedValue = context.ValueProvider.GetValue("isAssigned").FirstValue;

            if (string.IsNullOrEmpty(idValue) ||
                string.IsNullOrEmpty(titleValue) ||
                string.IsNullOrEmpty(authorValue) ||
                string.IsNullOrEmpty(genreValue) ||
                string.IsNullOrEmpty(yearValue) ||
                string.IsNullOrEmpty(statusValue) ||
                string.IsNullOrEmpty(isAssignedValue))
            {
                context.ModelState.AddModelError("", "Some value is not valid");
                return Task.CompletedTask;
            }

            if (!Int32.TryParse(yearValue, out int year) ||
                !Int32.TryParse(idValue, out int id) ||
                !Int32.TryParse(statusValue, out int status) || !Enum.IsDefined(typeof(BookStatus), status) ||
                !Boolean.TryParse(isAssignedValue, out bool isAssigned))
            {
                context.ModelState.AddModelError("", "Year, id, isAssigned or status is not valid");
                return Task.CompletedTask;
            }

            var result = new Book
            {
                Id = id,
                Title = titleValue,
                Author = authorValue,
                Genre = genreValue,
                Year = year,
                Status = status,
                IsAssigned = isAssigned
            };

            context.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;
        }
    }
}

