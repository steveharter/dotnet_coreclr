// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Reflection.Emit;

namespace System.Reflection
{
    internal sealed partial class RuntimeMethodInfo : MethodInfo, IRuntimeMethodInfo
    {
        private volatile Delegate _lazyInvokeDelegate;

        public sealed override void Invoke2()
        {
            InvokeDelegate1 d = GetOrCreateInvokeDelegate<InvokeDelegate1>();
            d(default(TypedReference));
        }

        public sealed override void Invoke2(TypedReference retVal)
        {
            InvokeDelegate1 d = GetOrCreateInvokeDelegate<InvokeDelegate1>();
            d(retVal);
        }

        public sealed override void Invoke2(TypedReference retVal, TypedReference arg0)
        {
            InvokeDelegate2 d = GetOrCreateInvokeDelegate<InvokeDelegate2>();
            d(retVal, arg0);
        }

        public sealed override void Invoke2(TypedReference retVal, TypedReference arg0, TypedReference arg1)
        {
            InvokeDelegate3 d = GetOrCreateInvokeDelegate<InvokeDelegate3>();
            d(retVal, arg0, arg1);
        }

        public sealed override void Invoke2(TypedReference retVal, TypedReference arg0, TypedReference arg1, TypedReference arg2)
        {
            InvokeDelegate4 d = GetOrCreateInvokeDelegate<InvokeDelegate4>();
            d(retVal, arg0, arg1, arg2);
        }

        private T GetOrCreateInvokeDelegate<T>() where T : Delegate
        {
            Delegate invokeDelegate = _lazyInvokeDelegate;
            if (invokeDelegate == null)
            {
                invokeDelegate = _lazyInvokeDelegate = CreateInvokeDelegate(this, emitNew: false);
            }

            try
            {
                return (T)invokeDelegate;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Wrong number of parameters for invoking " + this);
            }
        }
    }
}
