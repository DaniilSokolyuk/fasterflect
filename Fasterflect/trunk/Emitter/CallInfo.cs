﻿#region License
// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/
#endregion

using System;
using System.Reflection;

namespace Fasterflect.Emitter
{
    /// <summary>
    /// Stores all necessary information to construct a dynamic method and optionally
    /// parameters used to invoke that method.
    /// </summary>
    internal class CallInfo
    {
        private static readonly Type[] EmptyTypeArray = new Type[0];

        public CallInfo(Type targetType, MemberTypes memberTypes, string name)
            : this(targetType, memberTypes, name, EmptyTypeArray)
        {
        }

        public CallInfo(Type targetType, MemberTypes memberTypes, string name, Type[] paramTypes)
        {
            TargetType = targetType;
            MemberTypes = memberTypes;
            Name = name;
            ParamTypes = paramTypes;
        }

        public Type TargetType { get; private set; }
        public MemberTypes MemberTypes { get; set; }
        public Type[] ParamTypes { get; private set; }
        public string Name { get; private set; }
        public bool IsStatic { get; set; }
        public object Target { get; set; }
        public object[] Parameters { get; set; }

        /// <summary>
        /// Two <code>CallInfo</code> instances are considered equaled if the following properties
        /// are equaled: <code>TargetType</code>, <code>MemberTypes</code>, <code>Name</code>,
        /// and <code>ParamTypes</code>.
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = obj as CallInfo;
            if (other == null) return false;
            if (other == this) return true;
            if (other.TargetType != TargetType || other.MemberTypes != MemberTypes || 
                other.Name != Name || other.ParamTypes.Length != ParamTypes.Length)
                return false;
            for (int i = 0; i < ParamTypes.Length; i++)
                if (ParamTypes[i] != other.ParamTypes[i])
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 7;
            hashCode = 31 * hashCode + TargetType.GetHashCode();
            hashCode = 31 * hashCode + Name.GetHashCode();
            for (int i = 0; i < ParamTypes.Length; i++)
                hashCode = 31 * hashCode + ParamTypes[i].GetHashCode();
            return hashCode;
        }
    }
}
