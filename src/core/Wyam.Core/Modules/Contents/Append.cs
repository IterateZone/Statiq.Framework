using System.Collections.Generic;
using System.Threading.Tasks;
using Wyam.Common.Configuration;
using Wyam.Common.Documents;
using Wyam.Common.Execution;
using Wyam.Common.Modules;

namespace Wyam.Core.Modules.Contents
{
    /// <summary>
    /// Appends the specified content to the existing content of each document.
    /// </summary>
    /// <category>Content</category>
    public class Append : ContentModule
    {
        /// <summary>
        /// Appends the string value of the returned object to to content of each document.
        /// This allows you to specify different content to append for each document depending
        /// on the input document.
        /// </summary>
        /// <param name="content">A delegate that returns the content to append.</param>
        public Append(DocumentConfig<string> content)
            : base(content)
        {
        }

        /// <summary>
        /// The specified modules are executed against an empty initial document and the results
        /// are appended to the content of every input document (possibly creating more
        /// than one output document for each input document).
        /// </summary>
        /// <param name="modules">The modules to execute.</param>
        public Append(params IModule[] modules)
            : base(modules)
        {
        }

        /// <inheritdoc />
        protected override async Task<IDocument> ExecuteAsync(string content, IDocument input, IExecutionContext context)
        {
            if (input == null)
            {
                return await context.GetDocumentAsync(content);
            }
            return content == null ? input : await context.GetDocumentAsync(input, input.Content + content);
        }
    }
}