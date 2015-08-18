
#pragma warning (disable : 4244)

using namespace System;
using namespace System::Runtime::InteropServices;

void* __clrcall _managed_alloc(size_t n)
{
	return (void*)Marshal::AllocHGlobal((IntPtr)(long long)n);
}
void __clrcall _managed_dealloc(void* mem)
{
	Marshal::FreeHGlobal((IntPtr)mem);
}
void* __clrcall _managed_realloc(void* mem, size_t nsize)
{
	return (void*)Marshal::ReAllocHGlobal((IntPtr)mem, (IntPtr)(long long)nsize);
}

//Standard new
void* __clrcall operator new(size_t n)
{
	return _managed_alloc(n);
}
void __clrcall operator delete(void * p)
{
	if (p)
		_managed_dealloc(p);
}

//Placement new
void *__clrcall operator new(size_t, void *_Where)
{	// construct array with placement at _Where
	return (_Where);
}
void __clrcall operator delete(void *, void *)
{	// delete if placement new fails
}

//Array new
void *__clrcall operator new[](size_t s)
{
	return _managed_alloc(s);
}
void __clrcall operator delete[](void *p)
{
	if (p)
		_managed_dealloc(p);
}

//Placement array new
 void *__clrcall operator new[](size_t, void *_Where)
{	// construct array with placement at _Where
	return (_Where);
}
 void __clrcall operator delete[](void *, void *)
{	// delete if placement array new fails

}

void* __clrcall malloc(size_t s)
{
	return _managed_alloc(s);
}
void __clrcall free(void* mem)
{
	if (mem)
		_managed_dealloc(mem);
}
void* __clrcall realloc(void* p, size_t s)
{
	return _managed_realloc(p, s);
}

int __clrcall memcpy(void* _Dest, void* _Src, size_t _Size)
{
	char* dest = (char*)_Dest;
	char* src = (char*)_Src;

	for (size_t i = 0; i < _Size; i++)
	{
		dest[i] = src[i];
	}

	return 0;
}
int __clrcall memset(void* _dest, int _val, size_t size)
{
#pragma warning (disable:4242)
	char val = _val;
	char* dest = (char*)_dest;
	for (size_t i = 0; i < size; i++)
	{
		dest[i] = val;
	}
	return 0;
#pragma warning(default:4242)
}

/*
These are workarounds so that the binary doesn't depend on a native dll.
*/

#pragma warning(disable:4483)

void __clrcall __identifier(".cctor")()
{
}

extern "C" void __clrcall ___CxxCallUnwindDtor(void(__clrcall* param)(void *), void * arg)
{
	param(arg);
}
extern "C" void __clrcall ___CxxCallUnwindVecDtor(void(__clrcall* param)(void *, unsigned int, int, void(__clrcall*)(void *)), void * arg1, unsigned int arg2, int arg3, void(__clrcall* arg4)(void *))
{
	param(arg1, arg2, arg3, arg4);
}
