using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Extensions
{
    // Por padrão, as classes de extensão no C# são estáticas
    public static class ModelStateExtension
    {
        // Para tornar este método, um método de extensão, utilizamos o `this`
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            var result = new List<string>();
            foreach (var item in modelState.Values)
            {
                foreach (var error in item.Errors)
                {
                    result.Add(error.ErrorMessage);
                }
            }

            return result;
        }
    }
}
