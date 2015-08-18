#pragma once

//Evil, but prevents all sorts of compiler errors
#define _INC_STDDEF

#include <gcroot.h>

#undef _INC_STDDEF
//Function predefs
void* __clrcall malloc(size_t s);
void __clrcall free(void* mem);
void* __clrcall realloc(void* p, size_t s);
int __clrcall memcpy(void* _Dest, void* _Src, size_t _Size);
int __clrcall memset(void* dest, int cal, size_t size);

//Placement new
void *__clrcall operator new(size_t, void *_Where);
void __clrcall operator delete(void *, void *);
void* __clrcall operator new(size_t n);
void __clrcall operator delete(void * p);
void *__clrcall operator new[](size_t s);
void __clrcall operator delete[](void *p);
void *__clrcall operator new[](size_t, void *_Where);
void __clrcall operator delete[](void *, void *);

extern "C" void __clrcall ___CxxCallUnwindDtor(void(__clrcall*)(void *), void *);

namespace stdimit
{
#ifdef _M_CEE
	
	/* Internal */
#if 1
	template<class _Ty>
	struct _Get_align
	{	// struct used to determine alignment of _Ty
		_Ty _Elt0;
		char _Elt1;
		_Ty _Elt2;

		_Get_align();	// to quiet diagnostics
		~_Get_align();	// to quiet diagnostics
	};
#endif

	/* Templates */

	template<typename _Ty, size_t SIZE> struct array
	{
		_Ty _Value[SIZE];

		_Ty* operator*()
		{
			return _Value;
		}
		_Ty& operator[](size_t idx)
		{
			return _Value[idx];
		}
	};

	template<typename _Ty>
	_Ty&& forward(_Ty& v)
	{
		return static_cast<_Ty&&>(v);
	}

	//Vector
#if 1
	template<typename T0, typename T1>
	T0 find(T0 start, T0 end, T1 val)
	{
		for (; start < end; start++)
		{
			if (*start == val)
				return start;
		}
		return end;
	}

	template<typename _Ty>
	struct vector
	{
	private:
		_Ty* _val;

		size_t _capacity;
		size_t _size;

		bool _full()
		{
			return _size == _capacity;
		}
		void _resize()
		{
			_capacity *= 2;
			_val = (_Ty*)realloc(_val, _capacity);
		}

	public:
		struct iterator
		{
		public:
			vector& _vec;
			size_t _pos;

			_Ty* operator->()
			{
				return &_vec._val[_pos];
			}
			_Ty& operator*()
			{
				return _vec._val[_pos];
			}

			iterator operator++(int)
			{
				return iterator(_vec, _pos++);
			}
			iterator operator--(int)
			{
				return iterator(_vec, _pos--);
			}

			bool operator ==(const iterator& o)
			{
				return &_vec == &o._vec && _pos == o._pos;
			}
			bool operator !=(const iterator& o)
			{
				return !(*this == o);
			}

			bool operator <(const iterator& o)
			{
				return _pos < o._pos;
			}
			bool operator >(const iterator& o)
			{
				return _pos > o._pos;
			}

			iterator(vector& vec, size_t pos) :
				_vec(vec),
				_pos(pos)
			{

			}
			iterator(const iterator& v) :
				_vec(v._vec),
				_pos(v._pos)
			{

			}

			iterator& operator =(const iterator& v)
			{
				_vec = v._vec;
				_pos = v._pos;
				return *this;
			}
		};

		size_t size()
		{
			return _size;
		}
		size_t capacity()
		{
			return _capacity;
		}
		bool empty()
		{
			return _size == 0;
		}

		iterator begin()
		{
			return iterator(*this, 0);
		}
		iterator end()
		{
			return iterator(*this, _size);
		}

		void erase(const iterator& it)
		{
			_val[it._pos].~_Ty();
			memcpy(&_val[it._pos], &_val[it._pos + 1], _size - it._pos);
			_size--;
		}

		void push_back(const _Ty& val)
		{
			size_t prev = _size;
			_size++;
			if (_full())
				_resize();


			new(&_val[prev]) _Ty(val);
		}
		void pop_front()
		{
			erase(begin());
		}

		vector() :
			_val((_Ty*)malloc(sizeof(_Ty) * 10)),
			_capacity(10),
			_size(0)
		{

		}

		~vector()
		{
			for (size_t i = 0; i < _size; i++)
			{
				_val[i].~_Ty();
			}
			free(_val);
		}

		_Ty& at(size_t idx)
		{
			return _val[idx];
		}
	};

#endif

	//Atomic
#if 1

	using System::Threading::Interlocked;

	template<typename _T1, typename _T2> struct _same
	{
		static const bool value = false;
	};
	template<typename _T1> struct _same<_T1, _T1>
	{
		static const bool value = true;
	};

	template<typename _Nty, typename _Oty>
	struct _Bitwise_cast
	{
		union
		{
			_Oty _1;
			_Nty _2;
		};

		_Bitwise_cast(_Oty v1)
		{
			_1 = v1;
		}

		operator _Nty()
		{
			return _2;
		}
	};

	enum memory_order
	{
		memory_order_relaxed,
		memory_order_acquire,
		memory_order_release,
		memory_order_acq_rel,
		memory_order_seq_cst
	};

	template<typename _Ty, size_t SIZE> struct _Atomic_impl
	{
		typedef _Ty value_type;
		gcroot<System::Object^> _SyncLock = gcnew System::Object();
		_Ty _Value;
		
		struct locker
		{
			_Atomic_impl* p;

			void _Lock()
			{
				System::Threading::Monitor::Enter(p->_SyncLock);
			}
			void _Unlock()
			{
				System::Threading::Monitor::Unlock(p->_SyncLock);
			}

			locker(_Atomic_impl* parent) :
				p(parent)
			{
				_Lock();
			}
			~locker()
			{
				_Unlock();
			}
		};

		_Ty _Load()
		{
			locker l(this);
			return _Value;
		}
		void _Store(const _Ty& nval)
		{
			locker l(this);
			_Value = nval;
		}
	};
	template<typename _Ty>
	struct _Atomic_impl<_Ty, sizeof(System::Int32)>
	{
		typedef _Ty value_type;
		typedef int data_type;

		union
		{
			volatile data_type _Value;
			data_type _NvVal;
		};

		value_type _Load()
		{
			return (value_type)_Bitwise_cast<value_type, volatile data_type>(_Value);
		}
		void _Store(const value_type& nval)
		{
			_Value = (data_type)_Bitwise_cast<data_type, value_type>(nval);
		}

		bool _CompareExchange(const value_type& _nval, value_type% _expected)
		{
			data_type nval = (data_type)_Bitwise_cast<data_type, value_type>(_nval);
			data_type expected = (data_type)_Bitwise_cast<data_type, value_type>(_expected);
			data_type oldval = Interlocked::CompareExchange(_NvVal, nval, expected);
			_expected = (value_type)_Bitwise_cast<value_type, data_type>(oldval);
			return expected == oldval;
		}
		value_type _Exchange(const value_type& _nval)
		{
			return (value_type)_Bitwise_cast<value_type, data_type>(
				Interlocked::Exchange(_NvVal,
				(data_type)_Bitwise_cast<data_type, value_type>(_nval)));
		}

		value_type _Fetch_add(const value_type& _val)
		{
			return (value_type)_Bitwise_cast<value_type, data_type>(
				Interlocked::Add(_NvVal,
				(data_type)_Bitwise_cast<data_type, value_type>(_val)) -
				(data_type)_Bitwise_cast<data_type, value_type>(_val));
		}
		value_type _Fetch_sub(const value_type& _val)
		{
			return (value_type)_Bitwise_cast<value_type, data_type>(
				Interlocked::Add(_NvVal,
				(data_type)_Bitwise_cast<data_type, value_type>(_val)) +
				(data_type)_Bitwise_cast<data_type, value_type>(_val));
		}

		_Atomic_impl()
		{

		}
		_Atomic_impl(const value_type& value) :
			_Value((data_type)_Bitwise_cast<data_type, value_type>(value))
		{

		}
	};
	template<typename _Ty>
	struct _Atomic_impl<_Ty, sizeof(System::Int64)>
	{
		typedef _Ty value_type;
		typedef System::Int64 data_type;

		union
		{
			volatile data_type _Value;
			data_type _NvVal;
		};

		value_type _Load()
		{
			return (value_type)_Bitwise_cast<value_type, data_type>(Interlocked::Read(_NvVal));
		}
		void _Store(const value_type& nval)
		{
			_Value = (data_type)_Bitwise_cast<data_type, value_type>(nval);
		}

		bool _CompareExchange(const value_type& _nval, value_type% _expected)
		{
			data_type nval = (data_type)_Bitwise_cast<data_type, value_type>(_nval);
			data_type expected = (data_type)_Bitwise_cast<data_type, value_type>(_expected);
			data_type oldval = Interlocked::CompareExchange(_NvVal, nval, expected);
			_expected = (value_type)_Bitwise_cast<value_type, data_type>(oldval);
			return expected == oldval;
		}
		value_type _Exchange(const value_type& _nval)
		{
			return (value_type)_Bitwise_cast<value_type, data_type>(
				Interlocked::Exchange(_NvVal,
				(data_type)_Bitwise_cast<data_type, value_type>(_nval)));
		}

		value_type _Fetch_add(const value_type& _val)
		{
			return (value_type)_Bitwise_cast<value_type, data_type>(
				Interlocked::Add(_NvVal,
				(data_type)_Bitwise_cast<data_type, value_type>(_val)) -
				(data_type)_Bitwise_cast<data_type, value_type>(_val));
		}
		value_type _Fetch_sub(const value_type& _val)
		{
			return (value_type)_Bitwise_cast<value_type, data_type>(
				Interlocked::Add(_NvVal,
				(data_type)_Bitwise_cast<data_type, value_type>(_val)) +
				(data_type)_Bitwise_cast<data_type, value_type>(_val));
		}

		_Atomic_impl()
		{

		}
		_Atomic_impl(const value_type& value) :
			_Value((data_type)_Bitwise_cast<data_type, value_type>(value))
		{

		}
	};

	template<typename _Ty> struct _Atomic_impl<_Ty, 1> : public _Atomic_impl<_Ty, 2>
	{
		_Atomic_impl()
		{

		}
		_Atomic_impl(const value_type& value) :
			_Atomic_impl<_Ty, 2>(value)
		{

		}
	};
	template<typename _Ty> struct _Atomic_impl<_Ty, 2> : public _Atomic_impl<_Ty, 4>
	{
		_Atomic_impl()
		{

		}
		_Atomic_impl(const value_type& value) :
			_Atomic_impl<_Ty, 4>(value)
		{

		}
	};

	template<typename _Ity>
	class atomic
	{
	private:
		typedef _Ity _Ty;

		mutable _Atomic_impl<_Ty, sizeof(_Ty)> _Impl;

	public:
		bool _Compare_exchange(_Ty newval, _Ty% expected)
		{
			return _Impl._CompareExchange(newval, expected);
		}
		bool compare_exchange_strong(_Ty% newval, _Ty% expected, memory_order = memory_order_seq_cst)
		{
			return _Compare_exchange(newval, expected);
		}
		bool compare_exchange_strong(_Ty newval, _Ty expected, memory_order, memory_order)
		{
			return _Compare_exchange(newval, expected);
		}
		bool compare_exchange_weak(_Ty% newval, _Ty% expected, memory_order = memory_order_seq_cst)
		{
			return _Compare_exchange(newval, expected);
		}
		bool compare_exchange_weak(_Ty newval, _Ty expected, memory_order, memory_order)
		{
			return _Compare_exchange(newval, expected);
		}

		bool test_and_set(memory_order = memory_order_seq_cst)
		{
			bool prev = false;
			_Compare_exchange(true, prev);
			return prev;
		}

		_Ty load(memory_order = memory_order_seq_cst) const
		{
			return _Impl._Load();
		}
		void store(_Ty v, memory_order = memory_order_seq_cst)
		{
			_Impl._Store(v);
		}

		_Ty fetch_add(const _Ty& i, memory_order = memory_order_seq_cst)
		{
			return _Impl._Fetch_add(i);
		}
		_Ty fetch_sub(const _Ty& i, memory_order = memory_order_seq_cst)
		{
			return _Impl._Fetch_sub(i);
		}

		_Ty operator++(int)
		{
			return _Impl._Fetch_add(1) - 1;
		}
		_Ty operator--(int)
		{
			return _Impl._Fetch_sub(1) + 1;
		}

		_Ty operator ++()
		{
			return ++(*this) - 1;
		}
		_Ty operator --()
		{
			return --(*this) + 1;
		}

		_Ty exchange(const _Ty& v, memory_order = memory_order_seq_cst)
		{
			return _Impl._Exchange(v);
		}
		
		template<typename _Oty>
		bool operator ==(const _Oty& o)
		{
			return _Impl._Load() == o;
		}
		template<typename _Oty>
		bool operator !=(const _Oty& o)
		{
			return val != o;
		}

		void clear(memory_order = memory_order_seq_cst)
		{
			store(false);
		}

		operator _Ty()
		{
			return _Impl._Load();
		}

		atomic() :
			_Impl()
		{

		}
		atomic(_Ty val) :
			_Impl(val)
		{

		}
		template<typename _Oty>
		atomic(_Oty val) :
			_Impl((_Ty)val)
		{

		}
	};
#endif

	template<typename _Ty>
	ref struct shared_ptr_internal
	{
	public:
		_Ty* ptr;

		shared_ptr_internal(_Ty* p) :
			ptr(p)
		{

		}
		!shared_ptr_internal()
		{
			delete ptr;
		}

		~shared_ptr_internal()
		{

		}
	};

	template<typename _Ty>
	struct shared_ptr
	{
		//typedef a _Ty;
	private:


		gcroot<shared_ptr_internal<_Ty>^> _internal;

		_Ty* _Ptr;

	public:

		bool operator ==(_Ty* o)
		{
			return _Ptr == o;
		}
		bool operator !=(_Ty* o)
		{
			return _Ptr != o;
		}

		_Ty* operator->()
		{
			return _Ptr;
		}
		_Ty& operator*()
		{
			return *_Ptr;
		}

		operator _Ty*()
		{
			return _Ptr;
		}

		void operator =(const shared_ptr& ptr)
		{
			_Ptr = ptr._Ptr;
			_internal = ptr._internal;
		}

		_Ty* get()
		{
			return *this;
		}

		shared_ptr(const shared_ptr& ptr)
		{
			_Ptr = ptr._Ptr;
			_internal = ptr._internal;
		}
		shared_ptr(_Ty*const  ptr) :
			_Ptr(ptr),
			_internal(gcnew shared_ptr_internal<_Ty>(ptr))
		{

		}
		shared_ptr() :
			_Ptr(nullptr),
			_internal(nullptr)
		{

		}
	};

	template<typename _Ty>
	void atomic_store(shared_ptr<_Ty>* tgt, shared_ptr<_Ty> src)
	{
		*tgt = src;
	}
	template<typename _Ty>
	shared_ptr<_Ty> atomic_load(shared_ptr<_Ty>* src)
	{
		return *src;
	}

	template<typename _Ty>
	void swap(_Ty& v1, _Ty& v2)
	{
		_Ty cpy = v1;
		v1 = v2;
		v2 = cpy;
	}
	template<typename _Ty>
	_Ty&& move(_Ty& v)
	{
		return static_cast<_Ty&&>(v);
	}

	struct _False_type
	{
		static const bool value = false;
	};
	struct _True_type
	{
		static const bool value = true;
	};

	template<bool val> struct _Cat_base;

	template<> struct _Cat_base<true> : _True_type{};
	template<> struct _Cat_base<false> : _False_type{};

	template<typename _Ty> struct is_trivially_destructible : _Cat_base<__has_trivial_destructor(_Ty)>
	{

	};
	template<typename _Ty> struct alignment_of
	{
		static const size_t value = (sizeof(_Get_align<_Ty>) - 2 * sizeof(_Ty));
	};

	template<typename _Ty> struct make_signed
	{

	};

#if 1

#define TYPE_DEF(_type) template<> struct make_signed<unsigned _type> { typedef _type type; }; \
		template<> struct make_signed<_type> { typedef _type type; }

	TYPE_DEF(char);
	TYPE_DEF(short);
	TYPE_DEF(int);
	TYPE_DEF(long);
	TYPE_DEF(long long);

#undef TYPE_DEF

	template<typename _Ty> struct is_integral : _False_type { };
	template<typename _Ty> struct is_integral<_Ty*> : _True_type { };

#define INTEGRAL_DEF(type) template<> struct is_integral<type> : _True_type { }

	INTEGRAL_DEF(char);
	INTEGRAL_DEF(short);
	INTEGRAL_DEF(int);
	INTEGRAL_DEF(long);
	INTEGRAL_DEF(long long);
	INTEGRAL_DEF(unsigned char);
	INTEGRAL_DEF(unsigned short);
	INTEGRAL_DEF(unsigned int);
	INTEGRAL_DEF(unsigned long);
	INTEGRAL_DEF(unsigned long long);

#undef INTEGRAL_DEF

	template<typename _Ty> struct numeric_limits 
	{ 
		static const bool is_signed = false;
	};

#define NUMERIC_LIMIT_DEF(type) template<> struct numeric_limits<type> { static const bool is_signed = true; }

	NUMERIC_LIMIT_DEF(char);
	NUMERIC_LIMIT_DEF(short);
	NUMERIC_LIMIT_DEF(int);
	NUMERIC_LIMIT_DEF(long);
	NUMERIC_LIMIT_DEF(long long);

#undef NUMERIC_LIMIT_DEF

#define CHAR_BIT 8

#endif

	/* Typedefs */
	typedef unsigned int uint32_t;
	typedef unsigned long long uint64_t;
	typedef size_t size_t;
	typedef atomic<bool> atomic_flag;
	typedef size_t uintptr_t;

	/* Malloc and Free */

	inline void* __clrcall malloc(size_t s)
	{
		return ::malloc(s);
	}
	inline void __clrcall free(void* s)
	{
		::free(s);
	}
#endif
}

namespace moodycamel
{
	using System::Object;
	using System::Threading::Monitor;

	template<typename _Ty>
	struct ConcurrentQueue;

	struct ConsumerToken
	{
		template<typename _Ty>
		ConsumerToken(ConcurrentQueue<_Ty>&)
		{

		}
	};

	template<typename _Ty>
	struct ConcurrentQueue
	{
		//typedef int _Ty;
	private:
		gcroot<Object^> SyncLock = gcnew Object();
		stdimit::vector<_Ty> vector;

		struct locker
		{
			ConcurrentQueue* Queue;

			locker(ConcurrentQueue* q) :
				Queue(q)
			{
				Monitor::Enter(q->SyncLock);
			}
			~locker()
			{
				Monitor::Exit(Queue->SyncLock);
			}
		};

	public:
		void enqueue(const _Ty& val)
		{
			locker l(this);
			vector.push_back(val);
		}
		void enqueue(ConsumerToken&, const _Ty& val)
		{
			enqueue((val));
		}

		bool try_dequeue( _Ty& out)
		{
			if (vector.empty())
				return false;

			locker l{ this };
			out = vector.at(0);
			vector.pop_front();
			return true;
		}
		bool try_dequeue(ConsumerToken&, _Ty& out)
		{
			return try_dequeue(out);
		}
	};
}

namespace std
{
	using namespace stdimit;
}

#define NULL 0

