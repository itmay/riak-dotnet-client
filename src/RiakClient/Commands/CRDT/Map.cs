﻿// <copyright file="Map.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace RiakClient.Commands.CRDT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public class Map
    {
        private readonly Counter counters = new Counter();
        private readonly Set sets = new Set();
        private readonly Register registers = new Register();
        private readonly Flag flags = new Flag();
        private readonly MapOf<Map> maps = new MapOf<Map>();

        public Counter Counters
        {
            get { return counters; }
        }

        public Set Sets
        {
            get { return sets; }
        }

        public Register Registers
        {
            get { return registers; }
        }

        public Flag Flags
        {
            get { return flags; }
        }

        public MapOf<Map> Maps
        {
            get { return maps; }
        }

        [Serializable]
        public class MapOf<TValue> : Dictionary<RiakString, TValue>
        {
            public MapOf()
            {
            }

#if !NETCOREAPP1_1
			protected MapOf(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
#endif
		}

        [Serializable]
        public class Counter : MapOf<long>
        {
            public Counter()
            {
            }

#if !NETCOREAPP1_1
			protected Counter(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
#endif

			public long GetValue(RiakString key)
            {
                long value;
                if (TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    return default(long);
                }
            }
        }

        [Serializable]
        public class Set : MapOf<IList<byte[]>>
        {
            public Set()
            {
            }

#if !NETCOREAPP1_1
			protected Set(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
#endif

			public IEnumerable<string> GetValue(RiakString key)
            {
                return GetValueAsRiakStrings(key).Select(v => (string)v);
            }

            public IEnumerable<RiakString> GetValueAsRiakStrings(RiakString key)
            {
                IEnumerable<RiakString> valueAsRiakStrings = null;
                IList<byte[]> value = null;

                if (TryGetValue(key, out value))
                {
                    valueAsRiakStrings = value.Select(v => RiakString.FromBytes(v));
                }

                return valueAsRiakStrings;
            }

            public void Add(RiakString key, byte[] value)
            {
                IList<byte[]> values = null;

                if (!this.TryGetValue(key, out values))
                {
                    values = new List<byte[]>();
                    this[key] = values;
                }

                values.Add(value);
            }
        }

        [Serializable]
        public class Register : MapOf<byte[]>
        {
            public Register()
            {
            }

#if !NETCOREAPP1_1
			protected Register(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
#endif

			public string GetValue(RiakString key)
            {
                return (string)GetValueAsRiakString(key);
            }

            public RiakString GetValueAsRiakString(RiakString key)
            {
                RiakString valueAsRiakString = null;
                byte[] value = null;

                if (TryGetValue(key, out value))
                {
                    valueAsRiakString = new RiakString(value);
                }

                return valueAsRiakString;
            }
        }

        [Serializable]
        public class Flag : MapOf<bool>
        {
            public Flag()
            {
            }

#if !NETCOREAPP1_1
			protected Flag(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
#endif
		}
    }
}