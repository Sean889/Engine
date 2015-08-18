#pragma once

#define LIBLOD3D_API

#include "stdimit.h"
//#include "concurrentqueue.h"

namespace LibPlanet
{
	namespace detail
	{
		template<typename _Ty>
		//This is a cludge because managed types aren't allowed to have native arrays in them.
		value struct ArrayType
		{
			typedef _Ty val_ty;

			value struct Row
			{
				val_ty x;
				val_ty y;
				val_ty z;
				val_ty w;

				val_ty% operator[](size_t idx)
				{
					switch (idx)
					{
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					case 3:
						return w;
					default:
						throw gcnew System::IndexOutOfRangeException();
					}
				}

				Row(val_ty% x, val_ty% y, val_ty% z, val_ty% w) :
					x(x),
					y(y),
					z(z),
					w(w)
				{

				}
			};

			Row v0;
			Row v1;
			Row v2;
			Row v3;

			ArrayType(val_ty xx, val_ty xy, val_ty xz, val_ty xw, val_ty yx, val_ty yy, val_ty yz, val_ty yw, val_ty zx, val_ty zy, val_ty zz, val_ty zw, val_ty wx, val_ty wy, val_ty wz, val_ty ww) :
				v0(xx, xy, xz, xw),
				v1(yx, yy, yz, yw),
				v2(zx, zy, zz, zw),
				v3(wx, wy, wz, ww)
			{

			}

			Row% operator[](size_t idx)
			{
				switch (idx)
				{
				case 0:
					return v0;
				case 1:
					return v1;
				case 2:
					return v2;
				case 3:
					return v3;
				default:
					throw gcnew System::IndexOutOfRangeException();
				}
			}
			val_ty% access(size_t idx)
			{
				switch (idx)
				{
				case 0:
					return (*this)[0][0];
				case 1:
					return (*this)[0][1];
				case 2:
					return (*this)[0][2];
				case 3:
					return (*this)[0][3];
				case 4:
					return (*this)[1][0];
				case 5:
					return (*this)[1][1];
				case 6:
					return (*this)[1][2];
				case 7:
					return (*this)[1][3];
				case 8:
					return (*this)[2][0];
				case 9:
					return (*this)[2][1];
				case 10:
					return (*this)[2][2];
				case 11:
					return (*this)[2][3];
				case 12:
					return (*this)[3][0];
				case 13:
					return (*this)[3][1];
				case 14:
					return (*this)[3][2];
				case 15:
					return (*this)[3][3];
				default:
					throw gcnew System::IndexOutOfRangeException();
				}
			}
		};

		value struct Math
		{
			template<typename _Ty> static inline _Ty sqrt(_Ty val);
			template<typename _Ty> static inline _Ty tan(_Ty val);
			template<typename _Ty> static inline _Ty sin(_Ty val);
			template<typename _Ty> static inline _Ty cos(_Ty val);
		};

#pragma warning (disable : 4244)
#define FUNC_DEF(sqrt, Sqrt)template<> inline double Math::sqrt<double>(double val)	{ return System::Math::Sqrt(val); } template<> inline float Math::sqrt<float>(float val) { return (float)System::Math::Sqrt(val); } 

		FUNC_DEF(sqrt, Sqrt)
		FUNC_DEF(tan, Tan)
		FUNC_DEF(sin, Sin)
		FUNC_DEF(cos, Cos)

#pragma warning (default : 4244)
	}

	/* Declarations */

	template<typename _Ty>
	value struct tvec3
	{
	public:
		typedef _Ty val_ty;

	private:
		static bool fuzzy_zero(val_ty val)
		{
			return val < val_ty(0.001) && val > -val_ty(0.001);
		}

	public:
		val_ty x = 0;
		val_ty y = 0;
		val_ty z = 0;

#pragma warning(disable:4244)

		tvec3(val_ty x, val_ty y, val_ty z) :
			x(x),
			y(y),
			z(z)
		{

		}
		tvec3(OpenTK::Vector3% vec)
		{
			make(vec);
		}
		tvec3(OpenTK::Vector3d% vec)
		{
			make(vec);
		}

		template<typename _Src>
		explicit tvec3(_Src% src) :
			x(src.x),
			y(src.y),
			z(src.z)
		{

		}
		template<typename _Src>
		explicit tvec3(const _Src& src) :
			x(src.x),
			y(src.y),
			z(src.z)
		{

		}

		tvec3 operator +(tvec3% lhs)
		{
			return tvec3(x + lhs.x, y + lhs.y, z + lhs.z);
		}
		tvec3 operator -(tvec3% lhs)
		{
			return tvec3(x - lhs.x, y - lhs.y, z - lhs.z);
		}
		tvec3 operator *(tvec3% lhs)
		{
			return tvec3(x * lhs.x, y * lhs.y, z * lhs.z);
		}
		tvec3 operator /(tvec3% lhs)
		{
			return tvec3(x / lhs.x, y / lhs.y, z / lhs.z);
		}
		tvec3 operator +(val_ty lhs)
		{
			return tvec3(x + lhs, y + lhs, z + lhs);
		}
		tvec3 operator -(val_ty lhs)
		{
			return tvec3(x - lhs, y - lhs, z - lhs);
		}
		tvec3 operator *(val_ty lhs)
		{
			return tvec3(x * lhs, y * lhs, z * lhs);
		}
		tvec3 operator /(val_ty lhs)
		{
			return tvec3(x / lhs, y / lhs, z / lhs);
		}

		void operator +=(tvec3% lhs)
		{
			*this = *this + lhs;
		}
		void operator -=(tvec3% lhs)
		{
			*this = *this - lhs;
		}
		void operator *=(tvec3% lhs)
		{
			*this = *this * lhs;
		}
		void operator /=(tvec3% lhs)
		{
			*this = *this / lhs;
		}
		void operator +=(val_ty lhs)
		{
			*this = *this + lhs;
		}
		void operator -=(val_ty lhs)
		{
			*this = *this - lhs;
		}
		void operator *=(val_ty lhs)
		{
			*this = *this * lhs;
		}
		void operator /=(val_ty lhs)
		{
			*this = *this / lhs;
		}

		bool operator ==(tvec3% o)
		{
			return x == o.x && y == o.y && z == o.z;
		}
		bool operator !=(tvec3% o)
		{
			return !(*this == o);
		}

		template<typename _Src>
		void make(_Src% o)
		{
			x = val_ty(o.x);
			y = val_ty(o.y);
			z = val_ty(o.z);
		}
		void make(OpenTK::Vector3% vec)
		{
			x = vec.X;
			y = vec.Y;
			z = vec.Z;
		}
		void make(OpenTK::Vector3d% vec)
		{
			x = vec.X;
			y = vec.Y;
			z = vec.Z;
		}

		template<typename _Tgt>
		explicit operator _Tgt(void)
		{
			return _Tgt(x, y, z);
		}

		val_ty dot(tvec3% lhs)
		{
			return x * lhs.x + y * lhs.y + z * lhs.z;
		}
		tvec3 cross(tvec3% lhs)
		{
			return tvec3(
				y * lhs.z - z * lhs.y,
				z * lhs.x - x * lhs.z,
				x * lhs.y - y * lhs.x);
		}

		tvec3 lerp(tvec3% b, val_ty f)
		{
			return tvec3(
				(x * (1.0f - f)) + (b.x * f),
				(y * (1.0f - f)) + (b.y * f),
				(z * (1.0f - f)) + (b.z * f));
		}

		tvec3 normalized(void)
		{
			if (fuzzy_zero(x * x + y * y + z * z))
				return tvec3();

			val_ty v = 1 / detail::Math::sqrt(x * x + y * y + z * z);
			return tvec3(x * v, y * v, z * v);
		}
		void normalize(void)
		{
			*this = this->normalized();
		}

		inline val_ty length(void)
		{
			return detail::Math::sqrt(x * x + y * y + z * z);
		}
		inline val_ty length2(void)
		{
			return x * x * y * y * z * z;
		}
		inline val_ty distance(tvec3% o)
		{
			return (this->operator-(o)).length();
		}
		inline val_ty distance2(tvec3% o)
		{
			return (this->operator-(o)).length2();
		}
		inline tvec3 abs(void)
		{
			return tvec3(x < 0 ? -x : x, y < 0 ? -y : y, z < 0 ? -z : z);
		}

		static inline tvec3 map_to_sphere(tvec3% pos, val_ty radius)
		{
			return (pos).normalized() * radius;
		}
#pragma warning(default:4244)
	};
	template<typename _Ty>
	value struct tquat
	{
	public:
		typedef _Ty val_ty;

		val_ty x = val_ty(0);
		val_ty y = val_ty(0);
		val_ty z = val_ty(0);
		val_ty w = val_ty(1);

		tquat(val_ty x, val_ty y, val_ty z, val_ty w) :
			x(x),
			y(y),
			z(z),
			w(w)
		{

		}
		//Angles in radians
		tquat(tvec3<val_ty>% o)
		{
			double c1 = detail::Math::cos(o.y / 2);
			double s1 = detail::Math::sin(o.y / 2);
			double c2 = detail::Math::cos(o.x / 2);
			double s2 = detail::Math::sin(o.x / 2);
			double c3 = detail::Math::cos(o.z / 2);
			double s3 = detail::Math::sin(o.z / 2);
			double c1c2 = c1*c2;
			double s1s2 = s1*s2;
			w = c1c2*c3 - s1s2*s3;
			x = c1c2*s3 + s1s2*c3;
			y = s1*c2*c3 + c1*s2*s3;
			z = c1*s2*c3 - s1*c2*s3;

			this->normalize();
		}
		tquat(OpenTK::Quaternion% val)
		{
			make(val);
		}
		tquat(OpenTK::Quaterniond% val)
		{
			make(val);
		}

		tquat operator +(val_ty o)
		{
			return tquat(x + o, y + o, z + o, w + o);
		}
		tquat operator -(val_ty o)
		{
			return tquat(x - o, y - o, z - o, w - o);
		}
		tquat operator *(val_ty o)
		{
			return tquat(x * o, y * o, z * o, w * o);
		}
		tquat operator /(val_ty o)
		{
			return tquat(x / o, y / o, z / o, w / o);
		}

		tquat conjugate(void)
		{
			return tquat(-x, -y, -z, -w);
		}
		val_ty norm(void)
		{
			using std::sqrt;
			return sqrt(x * x + y * y + z * z + w * w);
		}
		tquat reciprocal(void)
		{
			val_ty n = norm();
			n *= n;
			return *this / n;
		}

		tquat operator -(void)
		{
			return this->conjugate();
		}

		tquat operator +(tquat% o)
		{
			return tquat(x + o.x, y + o.y, z + o.z, w + o.w);
		}
		tquat operator -(tquat% o)
		{
			return tquat(x - o.x, y - o.y, z - o.z, w - o.w);
		}
		tquat operator *(tquat% o)
		{
			return tquat(
				x * o.w + y * o.z - z * o.y + w * o.x,
				-x * o.z + y * o.w + z * o.x + w * o.y,
				x * o.y - y * o.x + z * o.w + w * o.z,
				-x * o.x - y * o.y - z * o.z + w * o.w);
		}
		tquat operator /(tquat% o)
		{
			return *this * o.reciprocal();
		}

		void operator +=(tquat% o)
		{
			*this = *this + o;
		}
		void operator -=(tquat% o)
		{
			*this = *this - o;
		}
		void operator *=(tquat% o)
		{
			*this = *this * o;
		}
		void operator /=(tquat% o)
		{
			*this = *this / o;
		}

		void operator +=(val_ty o)
		{
			*this = *this + o;
		}
		void operator -=(val_ty o)
		{
			*this = *this - o;
		}
		void operator *=(val_ty o)
		{
			*this = *this * o;
		}
		void operator /=(val_ty o)
		{
			*this = *this / o;
		}

		tquat normalized(void)
		{
			using std::sqrt;
			val_ty n = 1 / sqrt(x * x + y * y + z * z + w * w);
			return tquat(x / n, y / n, z / n, w / n);
		}
		void normalize(void)
		{
			*this = normalized();
		}

		template<typename _Src>
		tquat(_Src% src) :
			x(src.x),
			y(src.y),
			z(src.z),
			w(src.w)
		{

		}
		template<typename _Src>
		tquat(const _Src& src) :
			x(src.x),
			y(src.y),
			z(src.z),
			w(src.w)
		{

		}

		template<typename _Src>
		void make(_Src% src)
		{
			x = (src.x),
				y = (src.y),
				z = (src.z),
				w = (src.w)
		}
		template<typename _Src>
		void make(const _Src& src)
		{
			x = (src.x),
				y = (src.y),
				z = (src.z),
				w = (src.w)
		}
		void make(OpenTK::Quaternion% vec)
		{
			x = vec.X;
			y = vec.Y;
			z = vec.Z;
			w = vec.W;
		}
		void make(OpenTK::Quaterniond% vec)
		{
			x = vec.X;
			y = vec.Y;
			z = vec.Z;
			w = vec.W;
		}

		template<typename _Tgt>
		operator _Tgt(void)
		{
			return _Tgt(x, y, z, w);
		}
	};
	template<typename _Ty>
	value struct tmat4
	{
	public:
		typedef _Ty val_ty;

	private:
		detail::ArrayType<val_ty> data;
		property val_ty% m[size_t]
		{
			val_ty% get(size_t idx)
			{
				return data.access(idx);
			}
		}

		typedef typename detail::ArrayType<val_ty>::Row _RowType;

	public:

		tmat4(val_ty xx, val_ty xy, val_ty xz, val_ty xw,
			val_ty yx, val_ty yy, val_ty yz, val_ty yw,
			val_ty zx, val_ty zy, val_ty zz, val_ty zw,
			val_ty wx, val_ty wy, val_ty wz, val_ty ww)
		{
			data[0][0] = xx;
			data[0][1] = xy;
			data[0][2] = xz;
			data[0][3] = xw;
			data[1][0] = yx;
			data[1][1] = yy;
			data[1][2] = yz;
			data[1][3] = yw;
			data[2][0] = zx;
			data[2][1] = zy;
			data[2][2] = zz;
			data[2][3] = zw;
			data[3][0] = wx;
			data[3][1] = wy;
			data[3][2] = wz;
			data[3][3] = ww;
		}

		template<typename _Src>
		void make(_Src% src)
		{
			data[0][0] = val_ty(src[0][0]); data[0][1] = val_ty(src[0][1]); data[0][2] = val_ty(src[0][2]); data[0][3] = val_ty(src[0][3]);
			data[1][0] = val_ty(src[1][0]); data[1][1] = val_ty(src[1][1]); data[1][2] = val_ty(src[1][2]); data[1][3] = val_ty(src[1][3]);
			data[2][0] = val_ty(src[2][0]); data[2][1] = val_ty(src[2][1]); data[2][2] = val_ty(src[2][2]); data[2][3] = val_ty(src[2][3]);
			data[3][0] = val_ty(src[3][0]); data[3][1] = val_ty(src[3][1]); data[3][2] = val_ty(src[3][2]); data[3][3] = val_ty(src[3][3]);
		}
		template<typename _Src>
		void make(const _Src& src)
		{
			data[0][0] = val_ty(src[0][0]); data[0][1] = val_ty(src[0][1]); data[0][2] = val_ty(src[0][2]); data[0][3] = val_ty(src[0][3]);
			data[1][0] = val_ty(src[1][0]); data[1][1] = val_ty(src[1][1]); data[1][2] = val_ty(src[1][2]); data[1][3] = val_ty(src[1][3]);
			data[2][0] = val_ty(src[2][0]); data[2][1] = val_ty(src[2][1]); data[2][2] = val_ty(src[2][2]); data[2][3] = val_ty(src[2][3]);
			data[3][0] = val_ty(src[3][0]); data[3][1] = val_ty(src[3][1]); data[3][2] = val_ty(src[3][2]); data[3][3] = val_ty(src[3][3]);
		}
		void make(OpenTK::Matrix4% src)
		{
			data[0][0] = val_ty(src[0, 0]); data[0][1] = val_ty(src[0, 1]); data[0][2] = val_ty(src[0, 2]); data[0][3] = val_ty(src[0, 3]);
			data[1][0] = val_ty(src[1, 0]); data[1][1] = val_ty(src[1, 1]); data[1][2] = val_ty(src[1, 2]); data[1][3] = val_ty(src[1, 3]);
			data[2][0] = val_ty(src[2, 0]); data[2][1] = val_ty(src[2, 1]); data[2][2] = val_ty(src[2, 2]); data[2][3] = val_ty(src[2, 3]);
			data[3][0] = val_ty(src[3, 0]); data[3][1] = val_ty(src[3, 1]); data[3][2] = val_ty(src[3, 2]); data[3][3] = val_ty(src[3, 3]);
		}
		void make(OpenTK::Matrix4d% src)
		{
			data[0][0] = val_ty(src[0, 0]); data[0][1] = val_ty(src[0, 1]); data[0][2] = val_ty(src[0, 2]); data[0][3] = val_ty(src[0, 3]);
			data[1][0] = val_ty(src[1, 0]); data[1][1] = val_ty(src[1, 1]); data[1][2] = val_ty(src[1, 2]); data[1][3] = val_ty(src[1, 3]);
			data[2][0] = val_ty(src[2, 0]); data[2][1] = val_ty(src[2, 1]); data[2][2] = val_ty(src[2, 2]); data[2][3] = val_ty(src[2, 3]);
			data[3][0] = val_ty(src[3, 0]); data[3][1] = val_ty(src[3, 1]); data[3][2] = val_ty(src[3, 2]); data[3][3] = val_ty(src[3, 3]);
		}

		template<typename _Src>
		explicit tmat4(_Src% src)
		{
			make(src);
		}
		template<typename _Src>
		explicit tmat4(const _Src& src)
		{
			make(src);
		}

		tmat4 inverse(void)
		{
			val_ty inv[16], det;

			inv[0] = m[5] * m[10] * m[15] -
				m[5] * m[11] * m[14] -
				m[9] * m[6] * m[15] +
				m[9] * m[7] * m[14] +
				m[13] * m[6] * m[11] -
				m[13] * m[7] * m[10];

			inv[4] = -m[4] * m[10] * m[15] +
				m[4] * m[11] * m[14] +
				m[8] * m[6] * m[15] -
				m[8] * m[7] * m[14] -
				m[12] * m[6] * m[11] +
				m[12] * m[7] * m[10];

			inv[8] = m[4] * m[9] * m[15] -
				m[4] * m[11] * m[13] -
				m[8] * m[5] * m[15] +
				m[8] * m[7] * m[13] +
				m[12] * m[5] * m[11] -
				m[12] * m[7] * m[9];

			inv[12] = -m[4] * m[9] * m[14] +
				m[4] * m[10] * m[13] +
				m[8] * m[5] * m[14] -
				m[8] * m[6] * m[13] -
				m[12] * m[5] * m[10] +
				m[12] * m[6] * m[9];

			inv[1] = -m[1] * m[10] * m[15] +
				m[1] * m[11] * m[14] +
				m[9] * m[2] * m[15] -
				m[9] * m[3] * m[14] -
				m[13] * m[2] * m[11] +
				m[13] * m[3] * m[10];

			inv[5] = m[0] * m[10] * m[15] -
				m[0] * m[11] * m[14] -
				m[8] * m[2] * m[15] +
				m[8] * m[3] * m[14] +
				m[12] * m[2] * m[11] -
				m[12] * m[3] * m[10];

			inv[9] = -m[0] * m[9] * m[15] +
				m[0] * m[11] * m[13] +
				m[8] * m[1] * m[15] -
				m[8] * m[3] * m[13] -
				m[12] * m[1] * m[11] +
				m[12] * m[3] * m[9];

			inv[13] = m[0] * m[9] * m[14] -
				m[0] * m[10] * m[13] -
				m[8] * m[1] * m[14] +
				m[8] * m[2] * m[13] +
				m[12] * m[1] * m[10] -
				m[12] * m[2] * m[9];

			inv[2] = m[1] * m[6] * m[15] -
				m[1] * m[7] * m[14] -
				m[5] * m[2] * m[15] +
				m[5] * m[3] * m[14] +
				m[13] * m[2] * m[7] -
				m[13] * m[3] * m[6];

			inv[6] = -m[0] * m[6] * m[15] +
				m[0] * m[7] * m[14] +
				m[4] * m[2] * m[15] -
				m[4] * m[3] * m[14] -
				m[12] * m[2] * m[7] +
				m[12] * m[3] * m[6];

			inv[10] = m[0] * m[5] * m[15] -
				m[0] * m[7] * m[13] -
				m[4] * m[1] * m[15] +
				m[4] * m[3] * m[13] +
				m[12] * m[1] * m[7] -
				m[12] * m[3] * m[5];

			inv[14] = -m[0] * m[5] * m[14] +
				m[0] * m[6] * m[13] +
				m[4] * m[1] * m[14] -
				m[4] * m[2] * m[13] -
				m[12] * m[1] * m[6] +
				m[12] * m[2] * m[5];

			inv[3] = -m[1] * m[6] * m[11] +
				m[1] * m[7] * m[10] +
				m[5] * m[2] * m[11] -
				m[5] * m[3] * m[10] -
				m[9] * m[2] * m[7] +
				m[9] * m[3] * m[6];

			inv[7] = m[0] * m[6] * m[11] -
				m[0] * m[7] * m[10] -
				m[4] * m[2] * m[11] +
				m[4] * m[3] * m[10] +
				m[8] * m[2] * m[7] -
				m[8] * m[3] * m[6];

			inv[11] = -m[0] * m[5] * m[11] +
				m[0] * m[7] * m[9] +
				m[4] * m[1] * m[11] -
				m[4] * m[3] * m[9] -
				m[8] * m[1] * m[7] +
				m[8] * m[3] * m[5];

			inv[15] = m[0] * m[5] * m[10] -
				m[0] * m[6] * m[9] -
				m[4] * m[1] * m[10] +
				m[4] * m[2] * m[9] +
				m[8] * m[1] * m[6] -
				m[8] * m[2] * m[5];

			det = m[0] * inv[0] + m[1] * inv[4] + m[2] * inv[8] + m[3] * inv[12];

			if (det == 0)
				return tmat4();

			det = 1.0 / det;

			return tmat4(
				inv[0] * det, inv[1] * det, inv[2] * det, inv[3] * det,
				inv[4] * det, inv[5] * det, inv[6] * det, inv[7] * det,
				inv[8] * det, inv[9] * det, inv[10] * det, inv[11] * det,
				inv[12] * det, inv[13] * det, inv[14] * det, inv[15] * det);

		}
		tmat4 transpose(void)
		{
			return tmat4(
				data[3][3], data[3][2], data[3][1], data[3][0],
				data[2][3], data[2][2], data[2][1], data[2][0],
				data[1][3], data[1][2], data[1][1], data[1][0],
				data[0][3], data[0][2], data[0][1], data[0][0]);
		}
		val_ty determinant(void)
		{
			val_ty inv0 = m[5] * m[10] * m[15] -
				m[5] * m[11] * m[14] -
				m[9] * m[6] * m[15] +
				m[9] * m[7] * m[14] +
				m[13] * m[6] * m[11] -
				m[13] * m[7] * m[10];

			val_ty inv4 = -m[4] * m[10] * m[15] +
				m[4] * m[11] * m[14] +
				m[8] * m[6] * m[15] -
				m[8] * m[7] * m[14] -
				m[12] * m[6] * m[11] +
				m[12] * m[7] * m[10];

			val_ty inv8 = m[4] * m[9] * m[15] -
				m[4] * m[11] * m[13] -
				m[8] * m[5] * m[15] +
				m[8] * m[7] * m[13] +
				m[12] * m[5] * m[11] -
				m[12] * m[7] * m[9];

			val_ty inv12 = -m[4] * m[9] * m[14] +
				m[4] * m[10] * m[13] +
				m[8] * m[5] * m[14] -
				m[8] * m[6] * m[13] -
				m[12] * m[5] * m[10] +
				m[12] * m[6] * m[9];

			return m[0] * inv0 + m[1] * inv4 + m[2] * inv8 + m[3] * inv12;
		}

#define MAT4_APPLY_OP(op) 	data[0][0] op o, data[0][1] op o, data[0][2] op o, data[0][3] op o, \
							data[1][0] op o, data[1][1] op o, data[1][2] op o, data[1][3] op o, \
							data[2][0] op o, data[2][1] op o, data[2][2] op o, data[2][3] op o, \
							data[3][0] op o, data[3][1] op o, data[3][2] op o, data[3][3] op o

		tmat4 operator +(tmat4% o)
		{
			return tmat4(
				data[0][0] + o.data[0][0], data[0][1] + o.data[0][1], data[0][2] + o.data[0][2], data[0][3] + o.data[0][3],
				data[1][0] + o.data[1][0], data[1][1] + o.data[1][1], data[1][2] + o.data[1][2], data[1][3] + o.data[1][3],
				data[2][0] + o.data[2][0], data[2][1] + o.data[2][1], data[2][2] + o.data[2][2], data[2][3] + o.data[2][3],
				data[3][0] + o.data[3][0], data[3][1] + o.data[3][1], data[3][2] + o.data[3][2], data[3][3] + o.data[3][3]);
		}
		tmat4 operator -(tmat4% o)
		{
			return tmat4(
				data[0][0] - o.data[0][0], data[0][1] - o.data[0][1], data[0][2] - o.data[0][2], data[0][3] - o.data[0][3],
				data[1][0] - o.data[1][0], data[1][1] - o.data[1][1], data[1][2] - o.data[1][2], data[1][3] - o.data[1][3],
				data[2][0] - o.data[2][0], data[2][1] - o.data[2][1], data[2][2] - o.data[2][2], data[2][3] - o.data[2][3],
				data[3][0] - o.data[3][0], data[3][1] - o.data[3][1], data[3][2] - o.data[3][2], data[3][3] - o.data[3][3]);
		}
		tmat4 operator *(tmat4% o)
		{
#define MAT4_ELEM_MUL(x, y) data[x][0] * o.data[0][y] + data[x][1] * o.data[1][y] + data[x][2] * o.data[2][y] + data[x][3] * o.data[3][y]
#define MAT4_ROW_MUL(x) MAT4_ELEM_MUL(x, 0), MAT4_ELEM_MUL(x, 1), MAT4_ELEM_MUL(x, 2), MAT4_ELEM_MUL(x, 3)

			return tmat4(
				MAT4_ROW_MUL(0),
				MAT4_ROW_MUL(1),
				MAT4_ROW_MUL(2),
				MAT4_ROW_MUL(3));

#undef MAT4_ELEM_MUL
#undef MAT4_ROW_MUL
		}
		tmat4 operator /(tmat4% o)
		{
			return inverse() * o;
		}

		tmat4 operator +(val_ty o)
		{
			return tmat4(MAT4_APPLY_OP(/ ));
		}
		tmat4 operator -(val_ty o)
		{
			return tmat4(MAT4_APPLY_OP(-));
		}
		tmat4 operator *(val_ty o)
		{
			return tmat4(MAT4_APPLY_OP(*));
		}
		tmat4 operator /(val_ty o)
		{
			return tmat4(MAT4_APPLY_OP(/ ));
		}
		tmat4 operator %(val_ty o)
		{
			return tmat4(MAT4_APPLY_OP(%));
		}

		tvec3<val_ty> operator *(tvec3<val_ty>% o)
		{
			return tvec3<val_ty>(
				data[0][0] * o.x + data[0][1] * o.y + data[0][2] * o.z + data[0][3],
				data[1][0] * o.x + data[1][1] * o.y + data[1][2] * o.z + data[1][3],
				data[2][0] * o.x + data[2][1] * o.y + data[2][2] * o.z + data[2][3]);
		}

		tmat4 operator -(void)
		{
			return *this * -1;
		}
		_RowType& operator[](size_t i)
		{
			return data[i];
		}

		void operator +=(tmat4% o)
		{
			*this = *this + o;
		}
		void operator -=(tmat4% o)
		{
			*this = *this - o;
		}
		void operator *=(tmat4% o)
		{
			*this = *this * o;
		}
		void operator /=(tmat4% o)
		{
			*this = *this / o;
		}

		void operator +=(val_ty o)
		{
			*this = *this + o;
		}
		void operator -=(val_ty o)
		{
			*this = *this - o;
		}
		void operator *=(val_ty o)
		{
			*this = *this * o;
		}
		void operator /=(val_ty o)
		{
			*this = *this / o;
		}
		void operator %=(val_ty o)
		{
			*this = *this % o;
		}

		template<typename _Oty>
		tmat4(tmat4<_Oty>% src)
		{
			data[0][0] = val_ty(src.data[0][0]); data[0][1] = val_ty(src.data[0][1]); data[0][2] = val_ty(src.data[0][2]); data[0][3] = val_ty(src.data[0][3]);
			data[1][0] = val_ty(src.data[1][0]); data[1][1] = val_ty(src.data[1][1]); data[1][2] = val_ty(src.data[1][2]); data[1][3] = val_ty(src.data[1][3]);
			data[2][0] = val_ty(src.data[2][0]); data[2][1] = val_ty(src.data[2][1]); data[2][2] = val_ty(src.data[2][2]); data[2][3] = val_ty(src.data[2][3]);
			data[3][0] = val_ty(src.data[3][0]); data[3][1] = val_ty(src.data[3][1]); data[3][2] = val_ty(src.data[3][2]); data[3][3] = val_ty(src.data[3][3]);
		}

		template<typename _Tgt>
		explicit operator _Tgt(void)
		{
#pragma warning (disable : 4244)
			return _Tgt(
				data[0][0], data[0][1], data[0][2], data[0][3],
				data[1][0], data[1][1], data[1][2], data[1][3],
				data[2][0], data[2][1], data[2][2], data[2][3],
				data[3][0], data[3][1], data[3][2], data[3][3]);
#pragma warning (default : 4244)
		}
#undef MAT4_APPLY_OP
	};
	template<typename _Ty>
	value struct tcoord
	{
		typedef _Ty val_ty;

		tquat<val_ty> rotation;
		tvec3<val_ty> position;
		tvec3<val_ty> scale = tvec3<val_ty>(1, 1, 1);

		tcoord(tvec3<val_ty>% pos, tquat<val_ty>% rot) :
			position(pos),
			rotation(rot)
		{

		}
		tcoord(tvec3<val_ty>% pos, tquat<val_ty>% rot, tvec3<val_ty>% scale) :
			position(pos),
			rotation(rot),
			scale(scale)
		{

		}
		tcoord(EngineSystem::Coord^ transform) :
			position(transform->Position),
			rotation(transform->Rotation),
			scale(1, 1, 1)
		{

		}

		template<typename _Ty>
		tcoord(tcoord<_Ty>% src) :
			position(src.position),
			rotation(src.rotation),
			scale(src.scale)
		{

		}
	};


	/* External Functions */
}
namespace math
{
	template<typename _Ty>
	LibPlanet::tmat4<_Ty> rotate(LibPlanet::tquat<_Ty>% q)
	{
		return LibPlanet::tmat4<_Ty>(
			1.0f - 2.0f*q.y*q.y - 2.0f*q.z*q.z, 2.0f*q.x*q.y - 2.0f*q.z*q.w, 2.0f*q.x*q.z + 2.0f*q.y*q.w, 0.0f,
			2.0f*q.x*q.y + 2.0f*q.z*q.w, 1.0f - 2.0f*q.x*q.x - 2.0f*q.z*q.z, 2.0f*q.y*q.z - 2.0f*q.x*q.w, 0.0f,
			2.0f*q.x*q.z - 2.0f*q.y*q.w, 2.0f*q.y*q.z + 2.0f*q.x*q.w, 1.0f - 2.0f*q.x*q.x - 2.0f*q.y*q.y, 0.0f,
			0.0f, 0.0f, 0.0f, 1.0f);
	}
	template<typename _Ty>
	LibPlanet::tmat4<_Ty> translate(LibPlanet::tvec3<_Ty>% t)
	{
		return LibPlanet::tmat4<_Ty>(
			_Ty(1), _Ty(0), _Ty(0), t.x,
			_Ty(0), _Ty(1), _Ty(0), t.y,
			_Ty(0), _Ty(0), _Ty(1), t.z,
			_Ty(0), _Ty(0), _Ty(0), _Ty(1));
	}
	template<typename _Ty>
	LibPlanet::tmat4<_Ty> scale(LibPlanet::tvec3<_Ty>% s)
	{
		return LibPlanet::tmat4<_Ty>(
			s.x, _Ty(0), _Ty(0), _Ty(0),
			_Ty(0), s.y, _Ty(0), _Ty(0),
			_Ty(0), _Ty(0), s.z, _Ty(0),
			_Ty(0), _Ty(0), _Ty(0), _Ty(1));
	}
	template<typename _Ty>
	LibPlanet::tmat4<_Ty> project(_Ty l, _Ty r, _Ty b, _Ty t, _Ty n, _Ty f)
	{
		return LibPlanet::tmat4<_Ty>(
			_Ty(2) * n / (r - l), _Ty(0), _Ty(0), _Ty(0),
			_Ty(0), _Ty(2) * n / (t - b), _Ty(0), _Ty(0),
			(r + l) / (r - l), (t + b) / (t - b), -(f + n) / (f - n), _Ty(-1),
			_Ty(0), _Ty(0), _Ty(-2) * f * n / (f - n), _Ty(0));
	}
	template<typename _Ty>
	LibPlanet::tmat4<_Ty> project(_Ty near, _Ty far, _Ty fovy, _Ty aspect)
	{
		_Ty ymax = near * LibPlanet::detail::Math::tan(_Ty(0.017453293) * (fovy / _Ty(2)));
		_Ty xmax = ymax * aspect;

		return project(-xmax, xmax, -ymax, ymax, near, far);
	}

	/* Typedefs */

	typedef LibPlanet::tvec3<float> fvec3;
	typedef LibPlanet::tvec3<double> dvec3;

	typedef LibPlanet::tquat<float> fquat;
	typedef LibPlanet::tquat<double> dquat;
	
	typedef LibPlanet::tmat4<float> fmat4;
	typedef LibPlanet::tmat4<double> dmat4;

	typedef LibPlanet::tcoord<float> fcoord;
	typedef LibPlanet::tcoord<double> dcoord;
}
