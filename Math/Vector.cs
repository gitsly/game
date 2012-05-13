// Denna kod använde jag även i CT2890 Datorgrafik, gk 

//---------------------------------------------
// Template class for three dimensional vectors 
// and a a class used for homogenous coordinates.
//---------------------------------------------

#ifndef __Vector_Included__
#define __Vector_Included__

#include <stdlib.h>
#include <math.h>
#include <iostream>
#include <fstream>

#ifndef M_PI // keff math.h!
	#define M_PI 3.141592654
#endif

template <class T> class Vector4;

#define PLANE_FRONT 1
#define PLANE_BACK -1
#define PLANE_COINCIDE 0


// A 2d Vector (x, y)
template <class T>
class Vector2
{
public:
	//--------------
	// Data members.
	//--------------
	union {
		struct {
			T x, y;
		};
		T e[2];
	};


	//--------------
	// Methods.
	//--------------
	Vector2() {}

	Vector2(T k)
	{
		x = k;
		y = k;
	}

	Vector2(T x1, T y1)
	{
		x = x1;
		y = y1;
	}

	bool operator ==(const Vector2& v) const
	{
		return (v.x == x && v.y == y) ? true : false;
	}

	bool operator !=(const Vector2& v) const
	{
		return (v.x == x && v.y == y) ? false : true;
	}


	void set(T x1, T y1)
	{
		x = x1;
		y = y1;
	}

	void setv(T *a)
	{
		x = a[0];
		y = a[1];
	}

	void operator +=(const Vector2& v)
	{
		x += v.x;
		y += v.y;
	}

	Vector2 operator +(const Vector2& v) const
	{
		Vector2 w(*this);
		w += v;
		return w;
	}

	Vector2& operator -=(const Vector2& v)
	{
		x -= v.x;
		y -= v.y;
		return *this;
	}

	Vector2 operator / (const T& a) const
	{
		Vector2 w(* this);
		w /= a;
		return w;
	}

	Vector2 operator - (const Vector2& v) const
	{
		Vector2 w(	x - v.x,
					y - v.y);
		return w;
	}

	void operator = (const T &v)
	{
		x = v;
		y = v;
	}

	Vector2& operator *=(const T &mul)
	{
		x *= mul;
		y *= mul;
		return *this;
	}

	Vector2& operator *=(const Vector2 &mul)
	{
		x *= mul.x;
		y *= mul.y;
		return *this;
	}

	Vector2 operator *(T f) const
	{
		Vector2 w(*this);
		w *= f;
		return w;
	}

	// dot product.
	T operator * (const Vector2 &v) const
	{
		return x * v.x + y * v.y;
	}


	Vector2& operator /=(const T &div)
	{
		x /= div;
		y /= div;
		return *this;
	}

	Vector2& operator /=(const Vector2& v)
	{
		x /= v.x;
		y /= v.y;
		return *this;
	}

	Vector2 operator / (const Vector2& v) const
	{
		Vector2 w(*this);
		w /= v;
		return w;
	}

	Vector2 operator -()
	{
		Vector2 v;
		v.x = -x;
		v.y = -y;
		return v;
	}

	Vector2& interpolate(const Vector2& v1, T interp)
	{
		// Interpolation (the interp can range from 0-1)
		T inv = (1 - interp);
		x = (inv * x) + (v1.x * interp);
		y = (inv * y) + (v1.y * interp);
		return *this;
	}

	void clamp(T min, T max)
	{
		if(min >= max) {
			x = y = max;
		}
		else {
			if(x < min) x = min; else if(x > max) x = max;
			if(y < min) y = min; else if(y > max) y = max;
		}
	}

	T length() const 
	{
		return (T)sqrt(x * x + y * y);
	}

	// returns approx of length. (when the exact length is not needed & performance is a big issue).
	T qlength() const 
	{
		return (T)(fabs(x) + fabs(y));
	}

	void inverse()
	{
		x = -x;
		y = -y;
	}

	Vector2& normalize ()
	{
		T l = length();
		if(l) {
			x /= l;
			y /= l;
		}
		return *this;	
	}

	bool close(const Vector2& v, T mod) 
	{
		if(fabs(x - v.x) < mod
		&& fabs(y - v.y) < mod)
			return true;	
		return false;	
	}

	T operator[] (int i) const
	{
		return e[i];
	}

	bool zero() 
	{ 
		// is vector the zero vector?
		return !(x || y);
	}

	T maximum() const
	{
		T m = x;
		if(y > m) m = y;
		return m;
	}

	T minimum() const
	{
		T m = x;
		if(y < m) m = y;
		return m;
	}

	Vector2& setLength(T l1)
	{
		T l = length();
		if (l != 0.0)
		{
			x = (x / l) * l1;
			y = (y / l) * l1;
		}
		return *this;
	}
};


template <class T>
Vector2<T> closestPointOnLine(const Vector2<T>& a, const Vector2<T>& b, const Vector2<T>& p)
{
	T d, t;
	
	Vector2<T> ba = b - a;
	d = ba.length();
	Vector2<T> c = p - a;
	Vector2<T> v = ba / d; // normalized line vector

	t = v * c;
	// Check to see if ‘t’ is beyond the extents of the line segment
	if (t < 0)
		return a;
	if (t > d)
		return b;
	// Return the point between a and b.
	return a + (v * t);
}

template <class T>
bool pointWithinLineSegment(const Vector2<T>& a, const Vector2<T>& b, const Vector2<T>& p)
{
	T d, t;
	
	Vector2<T> ba = b - a;
	d = ba.length();
	Vector2<T> c = p - a;
	Vector2<T> v = ba / d; // normalized line vector
	t = v * c;
	// Check to see if ‘t’ is beyond the extents of the line segment
	if (t < 0 || t > d)
		return false;
	return true;
}


// Returns distance to line from given point 'p' in direction vec. line is positioned against 'lineOrigin'
// vec & lNormal should be normalized.
template <class T>
T intersect(const Vector2<T>& lineOrigin, const Vector2<T>& lNormal, const Vector2<T>& p, const Vector2<T>& vec)
{
	T d = -(lNormal * lineOrigin);
	T numer = lNormal * p + d;
	T denom = lNormal * vec;
	if(!denom) // normal is orthogonal to vector
		return -1;
	return -(numer / denom);
}

// Returns closest distance to sphere in direction ray_vector from ray_origin.
// Returns -1 if no intersection.
template <class T>
T sphereIntersection(const Vector2<T>& ray_origin, const Vector2<T>& ray_vector, const Vector2<T>& s_origin, T radius)
{
	Vector2<T> q = s_origin - ray_origin;
	// setup quadratic 
	T c = q.length();
	T v = q * ray_vector;
	T d = (radius * radius) - ((c * c) - (v * v));
	if (d < 0)
		return -1;
	return v - (T)sqrt(d);
}

template <class T>
std::ostream& operator << (std::ostream& out, const Vector2<T>& v)
{
//	out << v.x;
//	out << v.y;
	out.write((const char*)&v, sizeof(Vector2<T>));
	return out;
}

template <class T>
std::istream& operator >> (std::istream& in, Vector2<T>& v)
{
//	in >> v.x;
//	in >> v.y;
	in.read((char*)&v, sizeof(Vector2<T>));
	return in;
}


//-------------------
// Vector3 class code
//-------------------

template <class T>
class Vector3
{
public:
	//--------------
	// Data members.
	//--------------
	union {
		struct {
			T x, y, z;
		};
		T e[3];
	};


	//--------------
	// Methods.
	//--------------
	Vector3() {}

	Vector3(T k)
	{
		x = k;
		y = k;
		z = k;
	}

	bool operator ==(const Vector3& v) const
	{
		return (v.x == x && v.y == y && v.z == z) ? true : false;
	}

	bool operator !=(const Vector3& v) const
	{
		return (v.x == x && v.y == y && v.z == z) ? false : true;
	}

	Vector3(T x1, T y1, T z1)
	{
		x = x1;
		y = y1;
		z = z1;
	}

	void set(T x1, T y1, T z1)
	{
		x = x1;
		y = y1;
		z = z1;
	}

	void setv(T *a)
	{
		x = a[0];
		y = a[1];
		z = a[2];
	}

	void operator +=(const Vector3& v)
	{
		x += v.x;
		y += v.y;
		z += v.z;
	}

	Vector3 operator +(const Vector3& v) const
	{
		Vector3 w(*this);
		w += v;
		return w;
	}

	Vector3& operator -=(const Vector3& v)
	{
		x -= v.x;
		y -= v.y;
		z -= v.z;
		return *this;
	}

	Vector3 operator / (const T& a) const
	{
		Vector3 w(* this);
		w /= a;
		return w;
	}

	Vector3 operator - (const Vector3& v) const
	{
		Vector3 w(	x - v.x,
					y - v.y,
					z - v.z);
		return w;
	}

	const Vector3& operator = (const T &v)
	{
		x = v;
		y = v;
		z = v;
		return *this;
	}

	const Vector3& operator *=(const T &mul)
	{
		x *= mul;
		y *= mul;
		z *= mul;
		return *this;
	}

	Vector3& operator *=(const Vector3 &mul)
	{
		x *= mul.x;
		y *= mul.y;
		z *= mul.z;
		return *this;
	}

	Vector3 operator *(T f) const
	{
		Vector3 w(*this);
		w *= f;
		return w;
	}

	// dot product.
	T operator * (const Vector3 &v) const
	{
		return x * v.x + y * v.y + z * v.z;
	}

	// crossproduct.
	Vector3 operator ^ (const Vector3 &v) const
	{
		Vector3<T> w(
			y * v.z - z * v.y,
			z * v.x - x * v.z,
			x * v.y - y * v.x);
		return w;
	}

	Vector3& operator /=(const T &div)
	{
		x /= div;
		y /= div;
		z /= div;
		return *this;
	}

	Vector3& operator /=(const Vector3& v)
	{
		x /= v.x;
		y /= v.y;
		z /= v.z;
		return *this;
	}

	Vector3 operator / (const Vector3& v) const
	{
		Vector3 w(*this);
		w /= v;
		return w;
	}

	operator Vector2<T>() {
		return Vector2<T>(x, y);
	}

	Vector3 operator -()
	{
		Vector3 v;
		v.x = -x;
		v.y = -y;
		v.z = -z;
		return v;
	}

	Vector3& interpolate(const Vector3& v1, T interp)
	{
		// Interpolation (the interp can range from 0-1)
		T inv = (1 - interp);
		x = (inv * x) + (v1.x * interp);
		y = (inv * y) + (v1.y * interp);
		z = (inv * z) + (v1.z * interp);
		return *this;
	}

	void clamp(T min, T max)
	{
		if(min >= max) {
			x = y = z = max;
		}
		else {
			if(x < min) x = min; else if(x > max) x = max;
			if(y < min) y = min; else if(y > max) y = max;
			if(z < min) z = min; else if(z > max) z = max;
		}
	}

	T length() const 
	{
		return (T)sqrt(x * x + y * y + z * z);
	}

	// returns approx of length. (when the exact length is not needed & performance is a big issue).
	T qlength() const 
	{
		return (T)(fabs(x) + fabs(y) + fabs(z));
	}

	void inverse()
	{
		x = -x;
		y = -y;
		z = -z;
	}

	Vector3& normalize ()
	{
		T l = length();
		if(l) {
			x /= l;
			y /= l;
			z /= l;
		}
		return *this;	
	}

	bool close(const Vector3& v, T mod) 
	{
		if(fabs(x - v.x) < mod
		&& fabs(y - v.y) < mod
		&& fabs(z - v.z) < mod)
			return true;	
		return false;	
	}

	T& operator[] (int i)
	{
		return e[i];
	}

	bool zero() 
	{ 
		// is vector the zero vector?
		return !(x || y || z);
	}

	T maximum() const
	{
		T m = x;
		if(y > m) m = y;
		if(z > m) m = z;
		return m;
	}

	T minimum() const
	{
		T m = x;
		if(y < m) m = y;
		if(z < m) m = z;
		return m;
	}

	Vector3& setLength(T l1)
	{
		T l = length();
		if (l != 0.0)
		{
			x = (x / l) * l1;
			y = (y / l) * l1;
			z = (z / l) * l1;
		}
		return *this;
	}

	unsigned int col32() { // useful when vector is representing a 32 bit color value
		unsigned int r, g, b;
		r = (unsigned int)(x * 255);
		g = (unsigned int)(y * 255);
		b = (unsigned int)(z * 255);
		return (r << 16) + (g << 8) + b;
	}
};


//----------------------------------------------------
//  nonmember operator functions 
//----------------------------------------------------
template <class T>
Vector3<T> operator *(T k,const Vector3<T>&v)
{
	Vector3<T> w;
	w = v * k;
	return w;
}

template <class T>
Vector3<T> operator -(T k, const Vector3<T>& v)
{
	Vector3<T> w;
	w.x = k - v.x;
	w.y = k - v.y;
	w.z = k - v.z;
	return w;
}

template <class T>
Vector3<T> operator +(T k, const Vector3<T>& v)
{
	Vector3<T> w;
	w.x = k + v.x;
	w.y = k + v.y;
	w.z = k + v.z;
	return w;
}

//----------------------------------------------------
// nonmember vector functions.
//----------------------------------------------------
template <class T>
T dotProduct(const Vector3<T>& a, const Vector3<T>& b) // Also called scalarProduct
{
	return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
}


// complement to ^ operator.
template <class T>
Vector3<T> crossProduct(const Vector3<T>&a, const Vector3<T>&b)
{
	Vector3<T> w(
		a.y * b.z - a.z * b.y,
		a.z * b.x - a.x * b.z,
		a.x * b.y - a.y * b.x);
	return w;
}


// Without the normalization (assumes normalized vectors). 
template <class T>
T angularDiffOpt(const Vector3<T>&u, const Vector3<T>&v) 
{
	return (T)acos(dotProduct(u, v));	
}

template <class T>
Vector3<T> closestPointOnLine(const Vector3<T>& a, const Vector3<T>& b, const Vector3<T>& p)
{
	T d, t;
	
	Vector3<T> ba = b - a;
	d = ba.length();
	Vector3<T> c = p - a;
	Vector3<T> v = ba / d;

	t = v * c;
	// Check to see if ‘t’ is beyond the extents of the line segment
	if (t < 0)
		return a;
	if (t > d)
		return b;
	// Return the point between a and b.
	v *= t;
	return a + v;
}



// Returns distance to plane from given point 'p' in direction vec. plane is positioned against 'planeOrigin'
// vec & pNormal should be normalized.
template <class T>
T intersect(const Vector3<T>& planeOrigin, const Vector3<T>& pNormal, const Vector3<T>& p, const Vector3<T>& vec)
{
	T d = -(pNormal * planeOrigin);
	T numer = pNormal * p + d;
	T denom = pNormal * vec;

	if(!denom) // normal is orthogonal to vector
		return 0;

	return -(numer / denom);
}

template <class T>
Vector3<T> normalize (const Vector3<T> v)
{
	T len = v.length();
	return Vector3<T>(v.x / len, v.y / len, v.z / len);	
}

// Returns the angle (in radians) between two vectors.
template <class T>
T angularDiff(const Vector3<T>&u, const Vector3<T>&v) 
{
	/*
	Vector3 a, b;
	a = u;
	b = v;
	a.normalize();
	b.normalize();
	return (T)acos(dotProduct(a, b));	
	*/
	return (T)acos(dotProduct(normalize(u), normalize(v)));	
}

// Works as checking if a point is within a polygon made of 'v[]'.
template <class T>
bool pointInsidePolygon(const Vector3<T>& p, Vector3<T> *v, int num, T mod)
{
	Vector3<T> *j = new Vector3<T>[num];
	T a = 0;
	int i;
	
	for(i = 0; i < num; i++)
		j[i] = -(v[i] - p);

	for(i = 0; i < num; i++)
		a += angularDiff(j[i], j[(i + 1) % num]); // OPTIMIZE: get rid of normalization here if possible...

	delete j;
	return (fabs((2 * M_PI) - a) < mod);
}

// Returns closest distance to sphere in direction ray_vector from ray_origin.
// Returns -1 if no intersection.
template <class T>
T sphereIntersection(const Vector3<T>& ray_origin, const Vector3<T>& ray_vector, const Vector3<T>& s_origin, T radius)
{
	Vector3<T> q = s_origin - ray_origin;

	// setup quadratic 
	T c = q.length();
	T v = q * ray_vector;
	T d = (radius * radius) - ((c * c) - (v * v));
	if (d < 0)
		return -1;
	return v - (T)sqrt(d);
}


//---------------------------------
// Vector4 (homogenous coordinates)
//---------------------------------
template <class T>
class Vector4 : public Vector3<T>
{
public:
	Vector4() {}

	Vector4(const T& k) {
		*this = k;
	}

	Vector4(const Vector3<T>& v)
	{
		x = v.x;
		y = v.y;
		z = v.z;
		w = 1;	// assume point.
	}

	Vector4(const Vector3<T>& v, T _w)
	{
		x = v.x;
		y = v.y;
		z = v.z;
		w = _w;	
	}

	void operator * (const T &k) const {
		return vec4(x * k, y * k, z * k, w * k);
	}

	Vector4(T x_, T y_, T z_, T w_)
	{
		x = x_;
		y = y_;
		z = z_;
		w = w_;
	}

	void set(T x_, T y_, T z_, T w_)
	{
		x = x_;
		y = y_;
		z = z_;
		w = w_;
	}
	Vector3<T> homogenize() const
	{
		Vector3<T> v;
		if(!w)
			return Vector3<T>(0, 0, 0);
		v.x = x / w;
		v.y = y / w;
		v.z = z / w;
		return v;
	}

	Vector4& interpolate(const Vector4& v1, T interp)
	{
		// Interpolation (the interp can range from 0-1)
		T inv = (1 - interp);
		x = (inv * x) + (v1.x * interp);
		y = (inv * y) + (v1.y * interp);
		z = (inv * z) + (v1.z * interp);
		w = (inv * w) + (v1.w * interp);
		return *this;
	}

	bool operator ==(const Vector4& v) const
	{
		return (v.x == x && v.y == y && v.z == z && v.w == w) ? true : false;
	}

	bool operator !=(const Vector4& v) const
	{
		return (v.x == x && v.y == y && v.z == z && v.w == w) ? false : true;
	}

	void setv(T *a)
	{
		x = a[0];
		y = a[1];
		z = a[2];
		z = a[4];
	}

	void operator +=(const Vector4& v)
	{
		x += v.x;
		y += v.y;
		z += v.z;
		w += v.w;
	}

	Vector4 operator +(const Vector4& v) const
	{
		Vector3 w(*this);
		w += v;
		return w;
	}

	Vector4& operator -=(const Vector4& v)
	{
		x -= v.x;
		y -= v.y;
		z -= v.z;
		w -= v.w;
		return *this;
	}

	Vector4 operator / (const T& a) const
	{
		Vector4 w(* this);
		w /= a;
		return w;
	}

	Vector4 operator - (const Vector4& v) const
	{
		Vector4 w(	x - v.x,
					y - v.y,
					z - v.z,
					w - v.w);
		return w;
	}

	const Vector4& operator = (const T &v)
	{
		x = v;
		y = v;
		z = v;
		w = v;
		return *this;
	}


	const Vector4& operator *=(const T &mul) // scale
	{
		x *= mul;
		y *= mul;
		z *= mul;
		w *= mul;
		return *this;
	}

	Vector4& operator *=(const Vector4 &mul)
	{
		x *= mul.x;
		y *= mul.y;
		z *= mul.z;
		w *= mul.w;
		return *this;
	}

	// dot product.
	T operator * (const Vector4 &v) const
	{
		return x * v.x + y * v.y + z * v.z + w * v.w;
	}

	Vector4& operator /=(const T &div)
	{
		x /= div;
		y /= div;
		z /= div;
		w /= div;
		return *this;
	}

	Vector4& operator /=(const Vector4& v)
	{
		x /= v.x;
		y /= v.y;
		z /= v.z;
		w /= v.w;
		return *this;
	}

	Vector4 operator / (const Vector4& v) const
	{
		Vector3 w(*this);
		w /= v;
		return w;
	}

	Vector4 operator -()
	{
		Vector4 v;
		v.x = -x;
		v.y = -y;
		v.z = -z;
		v.w = -w;
		return v;
	}

	void clamp(T min, T max)
	{
		if(min >= max) {
			x = y = z = w = max;
		}
		else {
			if(x < min) x = min; else if(x > max) x = max;
			if(y < min) y = min; else if(y > max) y = max;
			if(z < min) z = min; else if(z > max) z = max;
			if(w < min) w = min; else if(w > max) w = max;
		}
	}

	T length() const 
	{
		return (T)sqrt(x * x + y * y + z * z + w * w);
	}

	// returns approx of length. (when the exact length is not needed & performance is a big issue).
	T qlength() const 
	{
		return (T)(fabs(x) + fabs(y) + fabs(z) + fabs(w));
	}

	void inverse()
	{
		x = -x;
		y = -y;
		z = -z;
		w = -w;
	}

	Vector4& normalize ()
	{
		T l = length();
		if(l) {
			x /= l;
			y /= l;
			z /= l;
			w /= l;
		}
		return *this;	
	}

	bool zero() 
	{ 
		// is vector the zero vector?
		return !(x || y || z || w);
	}

	T maximum() const
	{
		T m = x;
		if(y > m) m = y;
		if(z > m) m = z;
		if(w > m) m = w;
		return m;
	}

	T minimum() const
	{
		T m = x;
		if(y < m) m = y;
		if(z < m) m = z;
		if(w < m) m = w;
		return m;
	}

	T w;
};



#endif