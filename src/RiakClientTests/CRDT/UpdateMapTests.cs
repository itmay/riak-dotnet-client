// <copyright file="UpdateMapTests.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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

namespace RiakClientTests.CRDT
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Messages;
    using RiakClient.Models;
    using RiakClient.Util;

    [TestFixture]
    public class UpdateMapTests
    {
        [Test]
        public void UpdateMap_Should_Build_A_DtUpdateReq_Correctly()
        {
            const string bucketType = "maps";
            const string bucket = "myBucket";
            const string key = "map_1";

            byte[] context = Encoding.UTF8.GetBytes("test-context");

            var mapOp = new UpdateMap.MapOperation()
                .IncrementCounter("counter_1", 50)
                .RemoveCounter("counter_2")
                .AddToSet("set_1", "set_value_1");

            /*
                .removeFromSet("set_2", "set_value_2")
                .removeSet("set_3")
                .setRegister("register_1", new Buffer("register_value_1"))
                .removeRegister("register_2")
                .setFlag("flag_1", true)
                .removeFlag("flag_2")
                .removeMap("map_3");

            mapOp.map("map_2").incrementCounter("counter_1", 50)
                .removeCounter("counter_2")
                .addToSet("set_1", new Buffer("set_value_1"))
                .removeFromSet("set_2", new Buffer("set_value_2"))
                .removeSet("set_3")
                .setRegister("register_1", new Buffer("register_value_1"))
                .removeRegister("register_2")
                .setFlag("flag_1", true)
                .removeFlag("flag_2")
                .removeMap("map_3")
                .map("map_2");
             */


            var updateMapCommandBuilder = new UpdateMap.Builder();

            var q1 = new Quorum(1);
            var q2 = new Quorum(2);
            var q3 = new Quorum(3);

            updateMapCommandBuilder
                .WithBucketType(bucketType)
                .WithBucket(bucket)
                .WithKey(key)
                .WithMapOperation(mapOp)
                .WithContext(context)
                .WithW(q3)
                .WithPW(q1)
                .WithDW(q2)
                .WithReturnBody(true)
                .WithIncludeContext(false)
                .WithTimeout(TimeSpan.FromSeconds(20));

            UpdateMap updateMapCommand = updateMapCommandBuilder.Build();

            DtUpdateReq protobuf = updateMapCommand.ConstructPbRequest();
            Assert.AreEqual(Encoding.UTF8.GetBytes(bucketType), protobuf.type);
            Assert.AreEqual(Encoding.UTF8.GetBytes(bucket), protobuf.bucket);
            Assert.AreEqual(Encoding.UTF8.GetBytes(key), protobuf.key);
            Assert.AreEqual(q3, protobuf.w);
            Assert.AreEqual(q1, protobuf.pw);
            Assert.AreEqual(q2, protobuf.dw);
            Assert.True(protobuf.return_body);
            Assert.False(protobuf.include_context);
            Assert.AreEqual(20000, protobuf.timeout);
            Assert.AreEqual(context, protobuf.context);

            MapOp mapOpMsg = protobuf.op.map_op;

            VerifyRemoves(mapOpMsg.removes);
            VerifyUpdates(mapOpMsg.updates);
        }

        private static void VerifyRemoves(ICollection<MapField> mapFields)
        {
            // TODO Assert.AreEqual(5, mapFields.Count);
            Assert.AreEqual(1, mapFields.Count);

            bool counterRemoved = false;
            // bool setRemoved = false;
            // bool registerRemoved = false;
            // bool flagRemoved = false;
            // bool mapRemoved = false;

            foreach (MapField mapField in mapFields)
            {
                switch (mapField.type)
                {
                    case MapField.MapFieldType.COUNTER:
                        Assert.AreEqual((RiakString)mapField.name, (RiakString)"counter_2");
                        counterRemoved = true;
                        break;
                    case MapField.MapFieldType.SET:
                        Assert.AreEqual((RiakString)mapField.name, (RiakString)"set_3");
                        // setRemoved = true;
                        break;
                    case MapField.MapFieldType.MAP:
                        Assert.AreEqual((RiakString)mapField.name, (RiakString)"map_3");
                        // mapRemoved = true;
                        break;
                    case MapField.MapFieldType.REGISTER:
                        Assert.AreEqual((RiakString)mapField.name, (RiakString)"register_2");
                        // registerRemoved = true;
                        break;
                    case MapField.MapFieldType.FLAG:
                        Assert.AreEqual((RiakString)mapField.name, (RiakString)"flag_2");
                        // flagRemoved = true;
                        break;
                    default:
                        break;
                }
            }

            Assert.True(counterRemoved);
            // Assert.True(setRemoved);
            // Assert.True(registerRemoved);
            // Assert.True(flagRemoved);
            // Assert.True(mapRemoved);
        }

        private static MapUpdate VerifyUpdates(IEnumerable<MapUpdate> updates)
        {
            bool counterIncremented = false;
            bool setAddedTo = false;
            /*
            bool setRemovedFrom = false;
            bool registerSet = false;
            bool flagSet = false;
            bool mapAdded = false;
             */
            MapUpdate mapUpdate = null;

            foreach (MapUpdate update in updates)
            {
                switch (update.field.type)
                {
                    case MapField.MapFieldType.COUNTER:
                        Assert.AreEqual(RiakString.FromBytes(update.field.name), (RiakString)"counter_1");
                        Assert.AreEqual(update.counter_op.increment, 50);
                        counterIncremented = true;
                        break;
                    case MapField.MapFieldType.SET:
                        if (!EnumerableUtil.IsNullOrEmpty(update.set_op.adds))
                        {
                            Assert.AreEqual(RiakString.FromBytes(update.field.name), (RiakString)"set_1");
                            Assert.AreEqual(RiakString.FromBytes(update.set_op.adds[0]), (RiakString)"set_value_1");
                            setAddedTo = true;

                        }
                        else
                        {
                            Assert.AreEqual(RiakString.FromBytes(update.field.name), (RiakString)"set_2");
                            Assert.AreEqual(RiakString.FromBytes(update.set_op.removes[0]), (RiakString)"set_value_2");
                            // setRemovedFrom = true;
                        }
                        break;
                    case MapField.MapFieldType.MAP:
                        Assert.AreEqual(RiakString.FromBytes(update.field.name), (RiakString)"map_2");
                        // mapAdded = true;
                        mapUpdate = update;
                        break;
                    case MapField.MapFieldType.REGISTER:
                        Assert.AreEqual(RiakString.FromBytes(update.field.name), (RiakString)"register_1");
                        Assert.AreEqual(RiakString.FromBytes(update.register_op), (RiakString)"register_value_1");
                        // registerSet = true;
                        break;
                    case MapField.MapFieldType.FLAG:
                        Assert.AreEqual(RiakString.FromBytes(update.field.name), (RiakString)"flag_1");
                        Assert.AreEqual(update.flag_op, MapUpdate.FlagOp.ENABLE);
                        // flagSet = true;
                        break;
                    default:
                        break;
                }
            }

            Assert.True(counterIncremented);
            Assert.True(setAddedTo);
            // Assert.True(setRemovedFrom);
            // Assert.True(registerSet);
            // Assert.True(flagSet);
            // Assert.True(mapAdded);

            return mapUpdate;
        }
    }
}