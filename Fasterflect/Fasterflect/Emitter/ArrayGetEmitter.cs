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

namespace Fasterflect.Emitter
{
    internal class ArrayGetEmitter : BaseEmitter
    {
        public ArrayGetEmitter( Type targetType )
        {
            callInfo = new CallInfo( targetType, Flags.AllInstance, MemberTypes.Method,
                                     Constants.ArrayGetterName, new[] { typeof(int) }, null );
        }

        protected internal override DynamicMethod CreateDynamicMethod()
        {
            return CreateDynamicMethod( Constants.ArrayGetterName, callInfo.TargetType,
                                        Constants.ObjectType, new[] { Constants.ObjectType, Constants.IntType } );
        }

        protected internal override Delegate CreateDelegate()
        {
            Type elementType = callInfo.TargetType.GetElementType();
            generator.ldarg_0 // load array
                .castclass( callInfo.TargetType ) // (T[])array
                .ldarg_1 // load index
                .ldelem( elementType ) // load array[index]
                .boxIfValueType( elementType ) // [box] return
                .ret();
            return method.CreateDelegate( typeof(ArrayElementGetter) );
        }
    }
}