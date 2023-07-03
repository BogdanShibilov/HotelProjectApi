using HotelProjectApi.Infrastructure;
using HotelProjectApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace HotelProjectApi.Filters
{
    public class LinkRewritingFilter : IAsyncResultFilter
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public LinkRewritingFilter(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            ObjectResult asObjectResult = context.Result as ObjectResult;
            bool shouldSkip = asObjectResult?.StatusCode >= 400
                || asObjectResult?.Value == null
                || asObjectResult?.Value is not Resource;

            if (shouldSkip)
            {
                return next();
            }

            var rewriter = new LinkRewriter(_urlHelperFactory.GetUrlHelper(context));
            RewriteAllLinks(asObjectResult.Value, rewriter);

            return next();
        }

        private static void RewriteAllLinks(object model, LinkRewriter rewriter)
        {
            if (model == null) return;

            var allProperties = model
                .GetType()
                .GetProperties()
                .Where(p => p.CanRead)
                .ToArray();

            var linkProperties = allProperties
                .Where(p => p.CanWrite && p.PropertyType == typeof(Link));

            foreach ( var linkProperty in linkProperties)
            {
                var rewritten = rewriter.Rewrite(linkProperty.GetValue(model) as Link);
                if (rewritten == null) continue;

                linkProperty.SetValue(model, rewritten);

                if (linkProperty.Name == nameof(Resource.Self))
                {
                    allProperties.SingleOrDefault(p => p.Name == nameof(Resource.Href))
                        ?.SetValue(model, rewritten.Href);

                    allProperties.SingleOrDefault(p => p.Name == nameof(Resource.Method))
                        ?.SetValue(model, rewritten.Method);

                    allProperties.SingleOrDefault(p => p.Name == nameof(Resource.Relations))
                        ?.SetValue(model, rewritten.Relations);
                }
            }

            var arrayProperties = allProperties
                .Where(p => p.PropertyType.IsArray);
            RewriteAllLinksInArrays(arrayProperties, model, rewriter);

            var objectProperties = allProperties
                .Except(linkProperties)
                .Except(arrayProperties);
            RewriteAllLinksInNestedObjects(objectProperties, model, rewriter);


        }

        private static void RewriteAllLinksInNestedObjects(IEnumerable<PropertyInfo> objectProperties, object model, LinkRewriter rewriter)
        {
            foreach (var objectProperty in objectProperties)
            {
                if (objectProperty.PropertyType == typeof(string))
                {
                    continue;
                }

                var typeInfo = objectProperty.PropertyType.GetTypeInfo();
                if (typeInfo.IsClass)
                {
                    RewriteAllLinks(objectProperty.GetValue(model), rewriter);
                }
            }
        }

        private static void RewriteAllLinksInArrays(IEnumerable<PropertyInfo> arrayProperties, object model, LinkRewriter rewriter)
        {
            foreach (var arrayProperty in arrayProperties)
            {
                var array = arrayProperty.GetValue(model) as Array ?? Array.Empty<Array>();

                foreach (var element in array)
                {
                    RewriteAllLinks(element, rewriter);
                }
            }
        }
    }
}
