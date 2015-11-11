//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// ===========================================================================
// File: compilestubs.cpp
//
// Add stubs to referenced functions in order to link under Linux when sanitizers are enabled


#include "common.h"
#include "compile.h"

CompilationDomain::CompilationDomain(BOOL,
    BOOL,
    BOOL)
{
    _ASSERTE(false);
}

CompilationDomain::~CompilationDomain()
{
    _ASSERTE(false);
}

HRESULT CompilationDomain::SetContextInfo(LPCWSTR, BOOL)
{
    _ASSERTE(false);
    return E_FAIL;
}

HRESULT CompilationDomain::GetDependencies(CORCOMPILE_DEPENDENCY **,
DWORD *)
{
    _ASSERTE(false);
    return E_FAIL;
}
