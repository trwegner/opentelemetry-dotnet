﻿// <copyright file="Tags.cs" company="OpenTelemetry Authors">
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

namespace OpenTelemetry.Tags
{
    using OpenTelemetry.Tags.Propagation;

    public sealed class Tags
    {
        private static readonly object Lock = new object();

        private static Tags tags;

        // The TaggingState shared between the TagsComponent, Tagger, and TagPropagationComponent
        private readonly CurrentTaggingState state;
        private readonly ITagger tagger;
        private readonly ITagPropagationComponent tagPropagationComponent;

        internal Tags()
        {
            this.state = new CurrentTaggingState();
            this.tagger = new Tagger(this.state);
            this.tagPropagationComponent = new TagPropagationComponent(this.state);
        }

        public static ITagger Tagger
        {
            get
            {
                Initialize();
                return tags.tagger;
            }
        }

        public static ITagPropagationComponent TagPropagationComponent
        {
            get
            {
                Initialize();
                return tags.tagPropagationComponent;
            }
        }

        public static TaggingState State
        {
            get
            {
                Initialize();
                return tags.state.Value;
            }
        }

        internal static void Initialize()
        {
            if (tags == null)
            {
                lock (Lock)
                {
                    tags = tags ?? new Tags();
                }
            }
        }
    }
}
