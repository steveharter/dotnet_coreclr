// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using RuntimeTypeCache = System.RuntimeType.RuntimeTypeCache;

namespace System.Reflection
{
    internal sealed partial class RuntimeConstructorInfo : ConstructorInfo, IRuntimeMethodInfo
    {
        private volatile Delegate _lazyInvokeAndCreateDelegate;

        public sealed override void InvokeAndCreate(TypedReference retVal)
        {
            InvokeDelegate1 d = GetOrCreateInvokeAndCreateDelegate<InvokeDelegate1>();
            d(retVal);
        }

        public sealed override void InvokeAndCreate(TypedReference retVal, TypedReference arg0)
        {
            InvokeDelegate2 d = GetOrCreateInvokeAndCreateDelegate<InvokeDelegate2>();
            d(retVal, arg0);
        }

        public sealed override void InvokeAndCreate(TypedReference retVal, TypedReference arg0, TypedReference arg1)
        {
            InvokeDelegate3 d = GetOrCreateInvokeAndCreateDelegate<InvokeDelegate3>();
            d(retVal, arg0, arg1);
        }

        public sealed override void InvokeAndCreate(TypedReference retVal, TypedReference arg0, TypedReference arg1, TypedReference arg2)
        {
            InvokeDelegate4 d = GetOrCreateInvokeAndCreateDelegate<InvokeDelegate4>();
            d(retVal, arg0, arg1, arg2);
        }

        private T GetOrCreateInvokeAndCreateDelegate<T>() where T : Delegate
        {
            Delegate invokeAndCreateDelegate = _lazyInvokeAndCreateDelegate;
            if (invokeAndCreateDelegate == null)
            {
                invokeAndCreateDelegate = _lazyInvokeAndCreateDelegate = CreateInvokeDelegate(this, emitNew: true);
            }

            try
            {
                return (T)invokeAndCreateDelegate;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Wrong number of parameters for invoking " + this);
            }
        }
    }
}
