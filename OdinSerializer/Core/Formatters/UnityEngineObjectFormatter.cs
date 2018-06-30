//-----------------------------------------------------------------------
// <copyright file="UnityEngineObjectFormatter.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
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
//-----------------------------------------------------------------------

using OdinSerializer;

[assembly: RegisterFormatter(typeof(UnityEngineObjectFormatter<UnityEngine.Object>), 1000)]

namespace OdinSerializer
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Formatter for handling unity objects. Based off of ReflectionFormatter
    /// </summary>
    /// <typeparam name="T">The type which can be serialized and deserialized by the formatter.</typeparam>
    /// <seealso cref="BaseFormatter{T}" />
    public class UnityEngineObjectFormatter<T> : BaseFormatter<T>
    {
        public UnityEngineObjectFormatter()
        {
        }

        public UnityEngineObjectFormatter(ISerializationPolicy overridePolicy)
        {
            this.OverridePolicy = overridePolicy;
        }

        public ISerializationPolicy OverridePolicy { get; private set; }

        /// <summary>
        /// Provides the actual implementation for deserializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">The uninitialized value to serialize into. This value will have been created earlier using <see cref="BaseFormatter{T}.GetUninitializedObject" />.</param>
        /// <param name="reader">The reader to deserialize with.</param>
        protected override void DeserializeImplementation(ref T value, IDataReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides the actual implementation for serializing a value of type <see cref="T" />.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="writer">The writer to serialize with.</param>
        protected override void SerializeImplementation(ref T value, IDataWriter writer)
        {
            var members = FormatterUtilities.GetSerializableMembers(typeof(T), SerializationPolicies.Everything);//this.OverridePolicy ?? writer.Context.Config.SerializationPolicy);

            for (int i = 0; i < members.Length; i++)
            {
                var member = members[i];
                Type type;
                var memberValue = FormatterUtilities.GetMemberValue(member, value);

                if (object.ReferenceEquals(memberValue, null))
                {
                    type = FormatterUtilities.GetContainedType(member);
                }
                else
                {
                    type = memberValue.GetType();
                }

                if (type.Equals(typeof(SerializationData))) continue;

                var serializer = Serializer.Get(type);

                try
                {
                    serializer.WriteValueWeak(member.Name, memberValue, writer);
                }
                catch (Exception ex)
                {
                    writer.Context.Config.DebugContext.LogException(ex);
                }
            }
        }
    }
}
