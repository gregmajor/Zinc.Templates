using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Alba;
using Microsoft.AspNetCore.Http;

namespace RedLine.Extensions.Testing.Alba
{
    /// <summary>
    /// Builds a multipart/form-data request for use with Alba.
    /// </summary>
    internal static class MultiPartFormDataRequestBuilder
    {
        internal static void Build(IUrlExpression expression, dynamic payload)
        {
            if (expression is not Scenario scenario)
            {
                throw new InvalidOperationException("expected expression to be a Scenario");
            }

            // set up the boundary and create the multipart form data
            var formDataBoundary = $"--{Guid.NewGuid():N}";
            var contentType = "multipart/form-data; boundary=" + formDataBoundary;

            var multipartContent = new MultipartFormDataContent(formDataBoundary);

            // loop over properties of dynamic payload
            foreach (var property in ((Type)payload.GetType()).GetProperties())
            {
                var key = property.Name;
                var value = property.GetValue(payload);

                if (value is FormFile formFile)
                {
                    var data = new byte[formFile.Length];
                    using var stream = formFile.OpenReadStream();
                    stream.Read(data, 0, data.Length);
                    var fileContent = new ByteArrayContent(data);

                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        FileName = formFile.FileName,
                        Name = key,
                    };

                    multipartContent.Add(fileContent, key, formFile.FileName);
                }
                else
                {
                    multipartContent.Add(new StringContent(value.ToString()), key);
                }
            }

            scenario.ConfigureHttpContext(context =>
            {
                context.Request.ContentType = contentType;
                multipartContent.CopyToAsync(context.Request.Body).Wait();
                context.Request.ContentLength = context.Request.Body.Length;
                multipartContent.Dispose();
            });
        }
    }
}
