﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wyam.Common.Configuration;
using Wyam.Common.Execution;
using Wyam.Common.IO;
using Wyam.Common.Modules;
using Wyam.Common.Util;
using Wyam.Core.Execution;
using Wyam.Core.Modules.IO;

namespace Wyam.App
{
    public class PipelineBuilder
    {
        private readonly List<Action<IPipeline>> _actions = new List<Action<IPipeline>>();

        private readonly IPipelineCollection _collection;

        internal PipelineBuilder(IPipelineCollection collection, IReadOnlySettings settings)
        {
            _collection = collection;
            Settings = settings;
        }

        public IReadOnlySettings Settings { get; }

        internal IPipeline Build()
        {
            if (_actions.Count == 0)
            {
                return null;
            }

            IPipeline pipeline = new Pipeline();
            foreach (Action<IPipeline> action in _actions)
            {
                action(pipeline);
            }
            return pipeline;
        }

        public PipelineBuilder WithReadFiles(params string[] patterns)
        {
            _actions.Add(x => x.ReadModules.Add(new ReadFiles((DocumentConfig<IEnumerable<string>>)patterns)));
            return this;
        }

        public PipelineBuilder WithReadFiles(IEnumerable<string> patterns)
        {
            if (patterns != null)
            {
                _actions.Add(x => x.ReadModules.Add(new ReadFiles((DocumentConfig<IEnumerable<string>>)patterns)));
            }
            return this;
        }

        public PipelineBuilder WithWriteFiles()
        {
            _actions.Add(x => x.WriteModules.Add(new WriteFiles()));
            return this;
        }

        public PipelineBuilder WithWriteFiles(string extension)
        {
            _actions.Add(x => x.WriteModules.Add(new WriteFiles(extension)));
            return this;
        }

        public PipelineBuilder WithWriteFiles(DocumentConfig<FilePath> path)
        {
            _actions.Add(x => x.WriteModules.Add(new WriteFiles(path)));
            return this;
        }

        public PipelineBuilder AsSerial()
        {
            // Make sure not to add isolated pipelines as dependencies
            _actions.Add(x => x.Dependencies.AddRange(_collection.Where(p => !p.Value.Isolated).Select(p => p.Key)));
            return this;
        }

        public PipelineBuilder WithReadModules(IEnumerable<IModule> modules)
        {
            if (modules != null)
            {
                _actions.Add(x => x.WithReadModules(modules));
            }
            return this;
        }

        public PipelineBuilder WithReadModules(params IModule[] modules)
        {
            _actions.Add(x => x.WithReadModules(modules));
            return this;
        }

        public PipelineBuilder WithProcessModules(IEnumerable<IModule> modules)
        {
            if (modules != null)
            {
                _actions.Add(x => x.WithProcessModules(modules));
            }
            return this;
        }

        public PipelineBuilder WithProcessModules(params IModule[] modules)
        {
            _actions.Add(x => x.WithProcessModules(modules));
            return this;
        }

        public PipelineBuilder WithRenderModules(IEnumerable<IModule> modules)
        {
            if (modules != null)
            {
                _actions.Add(x => x.WithRenderModules(modules));
            }
            return this;
        }

        public PipelineBuilder WithRenderModules(params IModule[] modules)
        {
            _actions.Add(x => x.WithRenderModules(modules));
            return this;
        }

        public PipelineBuilder WithWriteModules(IEnumerable<IModule> modules)
        {
            if (modules != null)
            {
                _actions.Add(x => x.WithWriteModules(modules));
            }
            return this;
        }

        public PipelineBuilder WithWriteModules(params IModule[] modules)
        {
            _actions.Add(x => x.WithWriteModules(modules));
            return this;
        }

        public PipelineBuilder WithDependencies(params string[] dependencies)
        {
            _actions.Add(x => x.WithDependencies(dependencies));
            return this;
        }

        public PipelineBuilder WithDependencies(IEnumerable<string> dependencies)
        {
            if (dependencies != null)
            {
                _actions.Add(x => x.WithDependencies(dependencies));
            }
            return this;
        }

        public PipelineBuilder AsIsolated(bool isolated = true)
        {
            _actions.Add(x => x.AsIsolated(isolated));
            return this;
        }

        public PipelineBuilder AlwaysProcess(bool alwaysProcess = true)
        {
            _actions.Add(x => x.AlwaysProcess(alwaysProcess));
            return this;
        }
    }
}