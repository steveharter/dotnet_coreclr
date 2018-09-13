// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection.Emit;

namespace System.Reflection
{
    public abstract partial class MethodBase : MemberInfo
    {
        protected private delegate void InvokeDelegate1(TypedReference retVal);
        protected private delegate void InvokeDelegate2(TypedReference retVal, TypedReference arg0);
        protected private delegate void InvokeDelegate3(TypedReference retVal, TypedReference arg0, TypedReference arg1);
        protected private delegate void InvokeDelegate4(TypedReference retVal, TypedReference arg0, TypedReference arg1, TypedReference arg2);

        protected private static Delegate CreateInvokeDelegate(MethodBase method, bool emitNew)
        {
            Debug.Assert(!(emitNew && !(method is ConstructorInfo)));

            MethodInfo methodInfo = method as MethodInfo;
            ConstructorInfo constructorInfo = method as ConstructorInfo;

            if (method.ContainsGenericParameters)
                throw new InvalidOperationException("Method must not contain open generic parameters.");

            Type returnType = emitNew ? method.DeclaringType : (methodInfo == null ? typeof(void) : methodInfo.ReturnType); 
            if (returnType.IsByRef)
                throw new NotSupportedException("Ref returning Methods not supported.");
            bool hasRetVal = returnType != typeof(void);

            ParameterInfo[] parameters = method.GetParametersNoCopy();
            bool hasThis = !(emitNew || method.IsStatic);
            int numDelegateParameters = (hasRetVal ? 1 : 0) + parameters.Length + (hasThis ? 1 : 0);
            Type[] delegateParameters = new Type[numDelegateParameters];
            Array.Fill(delegateParameters, typeof(TypedReference));

            DynamicMethod dm = new DynamicMethod("InvokeStub_" + method.DeclaringType.Name + "." + method.Name, 
                typeof(void), 
                delegateParameters, 
                restrictedSkipVisibility: true);
            ILGenerator ilg = dm.GetILGenerator();

            int typeRefIndex = 0;
            if (hasRetVal)
            {
                ilg.Emit(OpCodes.Ldarg, typeRefIndex++);
                ilg.Emit(OpCodes.Refanyval, returnType);
            }

            if (hasThis)
            {
                ilg.Emit(OpCodes.Ldarg, typeRefIndex++);
                ilg.Emit(OpCodes.Refanyval, method.DeclaringType);
                ilg.Emit(OpCodes.Ldobj, method.DeclaringType);
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                Type parameterType = parameter.ParameterType;
                ilg.Emit(OpCodes.Ldarg, typeRefIndex++);
                if (parameterType.IsByRef)
                {
                    ilg.Emit(OpCodes.Refanyval, parameterType.GetElementType());
                }
                else
                {
                    ilg.Emit(OpCodes.Refanyval, parameterType);
                    ilg.Emit(OpCodes.Ldobj, parameterType);
                }
            }

            if (emitNew)
            {
                if (constructorInfo.IsStatic)
                    throw new NotSupportedException("Cannot call static constructor.");
                ilg.Emit(OpCodes.Newobj, constructorInfo);
            }
            else
            {
                if (methodInfo != null)
                {
                    ilg.Emit(method.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo);
                }
                else
                {
                    ilg.Emit(method.IsStatic ? OpCodes.Call : OpCodes.Callvirt, constructorInfo);
                }
            }

            if (hasRetVal)
            {
                ilg.Emit(OpCodes.Stobj, returnType);
            }

            ilg.Emit(OpCodes.Ret);

            Type delegateType;
            switch (numDelegateParameters)
            {
                case 1:
                    delegateType = typeof(InvokeDelegate1);
                    break;
                case 2:
                    delegateType = typeof(InvokeDelegate2);
                    break;
                case 3:
                    delegateType = typeof(InvokeDelegate3);
                    break;
                case 4:
                    delegateType = typeof(InvokeDelegate4);
                    break;
                default:
                    throw new NotImplementedException("Unsupported number of arguments.");
            }
            return dm.CreateDelegate(delegateType);
        }
    }
}
