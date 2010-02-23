﻿#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
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
using System.Reflection.Emit;
using Fasterflect.Caching;

namespace Fasterflect.Emitter
{
    internal abstract class BaseEmitter
    {
        private static readonly Cache<CallInfo, Delegate> cache = new Cache<CallInfo, Delegate>();
        private static readonly MethodInfo structGetMethod =
            Constants.StructType.GetMethod("get_Value", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo structSetMethod =
            Constants.StructType.GetMethod("set_Value", BindingFlags.Public | BindingFlags.Instance);

        protected CallInfo callInfo;
        protected DynamicMethod method;
        protected EmitHelper generator;

        public Delegate GetDelegate()
        {
            Delegate action = cache.Get( callInfo );
            if( action == null )
            {
                method = CreateDynamicMethod();
                generator = new EmitHelper( method.GetILGenerator() );
                action = CreateDelegate();
                cache.Insert( callInfo, action, CacheStrategy.Temporary );
            }
            return action;
        }

        protected internal abstract DynamicMethod CreateDynamicMethod();
        protected internal abstract Delegate CreateDelegate();

        protected internal static DynamicMethod CreateDynamicMethod( string name, Type targetType, Type returnType,
                                                                     Type[] paramTypes )
        {
            return new DynamicMethod( name, MethodAttributes.Static | MethodAttributes.Public,
                                      CallingConventions.Standard, returnType, paramTypes,
                                      targetType.IsArray ? targetType.GetElementType() : targetType,
                                      true );
        }

        protected void LoadInnerStructToLocal( byte localPosition )
        {
            generator
                .castclass( Constants.StructType ) // (ValueTypeHolder)wrappedStruct
                .callvirt(structGetMethod) // <stack>.get_Value()
                .unbox_any( callInfo.TargetType ) // unbox <stack>
                .stloc( localPosition ) // localStr = <stack>
                .ldloca_s( localPosition ); // load &localStr
        }

        protected void StoreLocalToInnerStruct( byte localPosition )
        {
            generator
                .ldarg_0 // load arg-0 (this)
                .castclass( Constants.StructType ) // wrappedStruct = (ValueTypeHolder)this
                .ldloc( localPosition ) // load localStr
                .boxIfValueType( callInfo.TargetType ) // box <stack>
                .callvirt( structSetMethod ); // wrappedStruct.set_Value(<stack>)
        }
    }
}