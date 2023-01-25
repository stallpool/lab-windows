// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include <time.h>
#include <string>

static HMODULE instance = 0;
static UINT64 count = 0;
static bool exited = false;


void run_cmd() {}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        DisableThreadLibraryCalls(hModule);
        wchar_t name[MAX_PATH];
        GetModuleFileNameW(hModule, name, MAX_PATH);
        LoadLibraryW(name);
        instance = hModule;
        CreateThread(nullptr, 0, [](void* pp) -> DWORD
            {
                while (!exited)
                {
                    time_t now = time(0);
                    __try {
                        if (count % 5 == 0) run_cmd();
                    } __finally {
                    }
                    Sleep(1000);
                    count = (count + 1) % 10000;
                }
                return 0;
            },
        nullptr, NULL, nullptr);
        break;
    case DLL_PROCESS_DETACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;
    }
    return TRUE;
}

#ifndef _EXPORT_
#define _EXPORT_
#define EXPORT extern "C" __declspec(dllexport)

EXTERN_C __declspec(dllexport) LRESULT CALLBACK msg_hook_proc_ov(int code, WPARAM wparam, LPARAM lparam)
{
    return CallNextHookEx(0, code, wparam, lparam);
}

#endif
