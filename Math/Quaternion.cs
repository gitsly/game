#ifndef _Lib3d_Quaternion_Included__
#define _Lib3d_Quaternion_Included__

#include "matrix.h"
#include "algebra.h"

// Some math.h doesnt define PI.
#ifndef M_PI 
	#define M_PI 3.141592
#endif

template <class T>
class Quaternion {
public:

	Quaternion()
	{
		x = y = z = 0;
		w = 1;
	}

	Quaternion(T _x, T _y, T _z, T _w)
	{
		x = _x;	
		y = _y;	
		z = _z;	
		w = _w;	
	}

	// create from euler angles.
	Quaternion(T a, T b, T c)
	{
		euler(a, b, c);
	} 

	// create from euler angles.
	Quaternion(const Vector3<T>& vec) 
	{
		euler(vec.x, vec.y, vec.z);
	} 

	void euler(T a, T b, T c) // Set quaternion from euler angles.
	{
		T angle;
		double sr, sp, sy, cr, cp, cy;

		angle = c * 0.5f;
		sy = sin(angle);
		cy = cos(angle);
		angle = b * 0.5f;
		sp = sin(angle);
		cp = cos(angle);
		angle = a * 0.5f;
		sr = sin(angle);
		cr = cos(angle);

		double crcp = cr * cp;
		double srsp = sr * sp;

		x = (T)(sr * cp * cy - cr * sp * sy);
		y = (T)(cr * sp * cy + sr * cp * sy);
		z = (T)(crcp * sy - srsp * cy);
		w = (T)(crcp * cy + srsp * sy); 
	}

	// Spherical Linear Interpolation
	// Set values from an interpolation between two other quaternions.
	// This will also modify the second quaternion if it is backwards.
	// q1, q2	The quaternions to interpolate between
	// interp	A value from 0.0-1.0 indicating the linear interpolation parameter.
	void slerp(const Quaternion<T>& q1, Quaternion<T>& q2, T interp)
	{
		// Decide if one of the quaternions is backwards
		int i;
		T a = 0, b = 0;
		
		for ( i = 0; i < 4; i++ )
		{
			a += (q1[i] - q2[i]) * (q1[i] - q2[i]);
			b += (q1[i] + q2[i]) * (q1[i] + q2[i]);
		}

		if (a > b)
			q2 *= -1; // get the inverse.

		T cosom = q1[0] * q2[0] + q1[1] * q2[1] + q1[2] * q2[2] + q1[3] * q2[3];
		double sclq1, sclq2;

		if((1.0 + cosom) > 0.00000001 ) {
			if((1.0 - cosom) > 0.00000001) {
				double omega = acos(cosom);
				double sinom = sin(omega);
				sclq1 = sin((1.0 - interp) * omega) / sinom;
				sclq2 = sin(interp * omega) / sinom;
			}
			else {
				sclq1 = 1.0 - interp;
				sclq2 = interp;
			}
			for( i = 0; i < 4; i++ )
				array[i] = (T)(sclq1 * q1[i] + sclq2 * q2[i]);
		}
		else {
			array[0] = -q1[1];
			array[1] = q1[0];
			array[2] = -q1[3];
			array[3] = q1[2];

			sclq1 = sin((1.0 - interp) * 0.5 * M_PI);
			sclq2 = sin(interp * 0.5 * M_PI);
			for(i = 0; i < 3; i++)
				array[i] = (T)(sclq1 * q1[i] + sclq2 * array[i]);
		}
	}

	// Create from angle rotation around a Vector3<T>. (angle is in radians).
	Quaternion(T angle, const Vector3<T>& vec)
	{
		rotate(angle, vec);
	}

	void rotate(T angle, const Vector3<T>& vec)
	{
		// Here we calculate the sin( theta / 2) once for optimization
		T result = (T)sin(angle / 2.0f);
		// Calculate the x, y and z of the quaternion
		x = T(vec.x * result);
		y = T(vec.y * result);
		z = T(vec.z * result);
		w = (T)cos(angle / 2.0f);
	}

		// quicky for the Vector3<T>class.
	void euler(const Vector3<T>& angle) 
	{
		euler(angle.x, angle.y, angle.z);
	} 

	void identity()
	{
		x = 0;
		y = 0;
		z = 0;
		w = 1;
	}


	//-------------------
	// Operators
	//-------------------
	T operator[](int i) const
	{
		return array[i];
	}

	const Matrix44<T> operator *= (T k) {
		x *= k; y *= k;	z *= k;	w *= k;
		return *this;
	}

	operator Matrix44<T>() const
	{
		Matrix44<T> m;
		m = Matrix44<T>::identity();
		// First row
		m[0] = 1.0f - 2.0f * ( y * y + z * z ); 
		m[1] = 2.0f * (x * y + z * w);
		m[2] = 2.0f * (x * z - y * w);
		// Second row
		m[4] = 2.0f * ( x * y - z * w );  
		m[5] = 1.0f - 2.0f * ( x * x + z * z ); 
		m[6] = 2.0f * (z * y + x * w );  
		// Third row
		m[8] = 2.0f * ( x * z + y * w );
		m[9] = 2.0f * ( y * z - x * w );
		m[10] = 1.0f - 2.0f * ( x * x + y * y );  
		return m;
	}


private:
	union {
		struct {
			T x;
			T y;
			T z;
			T w;
		};
		T array[4];
	};

};


#endif
