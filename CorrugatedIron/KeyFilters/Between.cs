// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

namespace CorrugatedIron.KeyFilters
{
    /// <summary>
    /// Tests that the input is between the first two arguments. 
    /// If the third argument is given, it is whether to treat the range as inclusive. 
    /// If the third argument is omitted, the range is treated as inclusive.
    /// </summary>
    public class Between<T> : RiakKeyFilterToken
    {
        public Between(T first, T second, bool inclusive = true)
            : base("between", first, second, inclusive)
        {
        }
    }
}