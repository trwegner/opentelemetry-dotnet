﻿// <copyright file="Tracing.cs" company="OpenTelemetry Authors">
// Copyright 2018, OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenTelemetry.Trace
{
    using OpenTelemetry.Context.Propagation;
    using OpenTelemetry.Internal;
    using OpenTelemetry.Trace.Config;
    using OpenTelemetry.Trace.Export;
    using OpenTelemetry.Trace.Internal;

    /// <summary>
    /// Class that manages a global instance of the <see cref="Tracer"/>.
    /// </summary>
    public sealed class Tracing
    {
        private static Tracing tracing = new Tracing();
        private static Tracer tracer;

        internal Tracing()
        {
            IRandomGenerator randomHandler = new RandomGenerator();
            IEventQueue eventQueue = new SimpleEventQueue();

            this.TraceConfig = new Config.TraceConfig();

            // TODO(bdrutu): Add a config/argument for supportInProcessStores.
            if (eventQueue is SimpleEventQueue)
            {
                this.ExportComponent = Export.ExportComponent.CreateWithoutInProcessStores(eventQueue);
            }
            else
            {
                this.ExportComponent = Export.ExportComponent.CreateWithInProcessStores(eventQueue);
            }

            IStartEndHandler startEndHandler =
                new StartEndHandler(
                    this.ExportComponent.SpanExporter,
                    ((ExportComponent)this.ExportComponent).RunningSpanStore,
                    ((ExportComponent)this.ExportComponent).SampledSpanStore,
                    eventQueue);

            tracer = new Tracer(randomHandler, startEndHandler, this.TraceConfig);
        }

        /// <summary>
        /// Gets the tracer to record spans.
        /// </summary>
        public static ITracer Tracer
        {
            get
            {
                return tracer;
            }
        }

        /// <summary>
        /// Gets the exporter to use to upload spans.
        /// </summary>
        public IExportComponent ExportComponent { get; }

        /// <summary>
        /// Gets the trace config.
        /// </summary>
        public ITraceConfig TraceConfig { get; }
    }
}
