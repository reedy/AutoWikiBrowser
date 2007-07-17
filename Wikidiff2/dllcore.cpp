// Wikidiff2.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "dllcore.h"


BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

wchar_t *wikidiff2_version()
{
	return L"3.9.2.0";
}


