// <copyright file="RiakEndPoint.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClient
{
    using System;
    using System.Collections.Generic;
    using Comms;

    /// <summary>
    /// Represents a connection to a Riak node, and allows operations to be performed with that connection.
    /// Partial abstract implementation of <see cref="IRiakEndPoint"/>.
    /// </summary>
    public abstract class RiakEndPoint : IRiakEndPoint
    {
        /// <inheritdoc />
        public TimeSpan RetryWaitTime { get; set; }

        /// <summary>
        /// The max number of retry attempts to make when the client encounters 
        /// <see cref="ResultCode"/>.NoConnections or <see cref="ResultCode"/>.CommunicationError errors.
        /// </summary>
        protected abstract int DefaultRetryCount { get; }

        /// <summary>
        /// Disable the exceptions that are thrown when expensive list operations are tried.
        /// </summary>
        protected abstract bool DisableListExceptions { get; }

        /// <summary>
        /// Creates a new instance of <see cref="RiakClient"/>.
        /// </summary>
        /// <returns>
        /// A minty fresh client.
        /// </returns>
        public IRiakClient CreateClient()
        {
            var opts = new RiakClientOptions(DefaultRetryCount, DisableListExceptions);
            return new RiakClient(this, opts);
        }

        /// <inheritdoc />
        public RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts)
        {
            return UseConnection(useFun, RiakResult.FromError, retryAttempts);
        }

        /// <inheritdoc />
        public RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts)
        {
            return UseConnection(useFun, RiakResult<TResult>.FromError, retryAttempts);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="RiakEndPoint"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public abstract RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;

        protected abstract void Dispose(bool disposing);

        protected abstract TRiakResult UseConnection<TRiakResult>(Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, bool, TRiakResult> onError, int retryAttempts)
            where TRiakResult : RiakResult;
    }
}
