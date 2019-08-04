﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Statiq.Common;

namespace Statiq.Core
{
    /// <summary>
    /// Adds the result document(s) content to metadata of the input documents.
    /// </summary>
    /// <remarks>
    /// The content of each child result document will be converted to a string.
    /// If more than one result document is produced, the content of each will be added as an array
    /// to the specified metadata key.
    /// </remarks>
    /// <category>Content</category>
    public class AddContentToMetadata : ChildDocumentsModule
    {
        private readonly string _key;

        public AddContentToMetadata(string key)
            : base(Array.Empty<IModule>())
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public AddContentToMetadata(string key, params IModule[] modules)
            : base(modules)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public AddContentToMetadata(string key, params string[] pipelines)
            : base(new ExecuteConfig(Config.FromContext(ctx => ctx.Outputs.FromPipelines(pipelines))))
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteAsync(
            IExecutionContext context,
            IReadOnlyList<IDocument> childOutputs) =>
            childOutputs.Count == 0
                ? (IEnumerable<IDocument>)context.Inputs
                : await context.QueryInputs().SelectAsync(async input => input.Clone(new MetadataItems
                {
                    {
                        _key,
                        childOutputs.Count == 1
                            ? (object)await childOutputs[0].GetStringAsync()
                            : (await childOutputs.SelectAsync(x => x.GetStringAsync())).ToArray()
                    }
                }));
    }
}