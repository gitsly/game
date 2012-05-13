// Denna kod använde jag även i CT2890 Datorgrafik, gk 

#ifndef __Matrix44_H_Included__
#define __Matrix44_H_Included__

#include "vec.h"
#include <stdlib.h>
#include <stdio.h>
#include <memory.h> 
#include <math.h>
#include <fstream>
#include <string>

#define a_(r, c) (cell[c-1][r-1]) // to be able to use standard mathematic notation of row and columns

// Template maxtrix class.

/* 4x4 Matrix structure.
Matrices are in column major format, like they are in OpenGL

e:
	0| 4|8 |12|
	1| 5|9 |13|
	2| 6|10|14|
	3| 7|11|15|

         c  r
	cell[1][3] == e[7];
*/

template <class T>
class Matrix44 {

public:

	Matrix44 () {}
	Matrix44 (const Matrix44& m) {
		*this = m;
	}

	Matrix44 (const T*a) {
		setv(a);
	}

	Vector3<T> getTrans()
	{
		Vector3 r;
		r.x = e[12];
		r.y = e[13];
		r.z = e[14];
		return r;
	}

	static Matrix44 identity()
	{
		Matrix44 t;
		t.e[0]	= 1; t.e[4]	= 0; t.e[8]	= 0; t.e[12] = 0;
		t.e[1]	= 0; t.e[5]	= 1; t.e[9]	= 0; t.e[13] = 0;
		t.e[2]	= 0; t.e[6]	= 0; t.e[10]= 1; t.e[14] = 0;
		t.e[3]	= 0; t.e[7]	= 0; t.e[11]= 0; t.e[15] = 1; 
		return t;
	}

	static Matrix44 translate(T x, T y, T z)
	{
		Matrix44 t;
		t.e[0]	= 1; t.e[4]	= 0; t.e[8]	= 0; t.e[12] = x;
		t.e[1]	= 0; t.e[5]	= 1; t.e[9]	= 0; t.e[13] = y;
		t.e[2]	= 0; t.e[6]	= 0; t.e[10]= 1; t.e[14] = z;
		t.e[3]	= 0; t.e[7]	= 0; t.e[11]= 0; t.e[15] = 1; 
		return t;
	}

	static Matrix44 translate(const Vector3<T>& v)
	{
		Matrix44 t;
		t.e[0]	= 1; t.e[4]	= 0; t.e[8]	= 0; t.e[12] = v.x;
		t.e[1]	= 0; t.e[5]	= 1; t.e[9]	= 0; t.e[13] = v.y;
		t.e[2]	= 0; t.e[6]	= 0; t.e[10]= 1; t.e[14] = v.z;
		t.e[3]	= 0; t.e[7]	= 0; t.e[11]= 0; t.e[15] = 1; 
		return t;
	}

	Matrix44& clearTranslation()
	{
		cell[3][0] = 0;
		cell[3][1] = 0;
		cell[3][2] = 0;
		return *this;
	}

	

	void setrc(int row, int col, T val)
	{
		cell[col][row] = val;
	}

	T getrc(int row, int col) const
	{
		return cell[col][row];
	}

	T get(int index) const
	{
		return e[index];
	}

	void setv(const T a[16])
	{
		memcpy(e, a, sizeof(T) * 16);
	}

	//index operator
	T& operator [](int i) {
		return e[i];
	}

	Matrix44<T> operator *(const T s) const
	{
		Matrix44<T> m;
		for(int i = 0; i < 16; i++) 
			m.e[i] = e[i] * s;
		return m;
	}

	Vector4<T> operator *(const Vector4<T>& v) const
	{
		Vector4<T> t;
		t.x = v.x * cell[0][0] + v.y * cell[1][0] + v.z * cell[2][0] + v.w * cell[3][0];
		t.y = v.x * cell[0][1] + v.y * cell[1][1] + v.z * cell[2][1] + v.w * cell[3][1];
		t.z = v.x * cell[0][2] + v.y * cell[1][2] + v.z * cell[2][2] + v.w * cell[3][2];
		t.w = v.x * cell[0][3] + v.y * cell[1][3] + v.z * cell[2][3] + v.w * cell[3][3];
		return t;
	}

	// Assume vector is a point (not a "vector") when multiplying with 3-dim vector.
	Vector3<T> operator *(const Vector3<T>& v) const
	{
		Vector4<T> t;
		t.x = v.x * cell[0][0] + v.y * cell[1][0] + v.z * cell[2][0] + cell[3][0];
		t.y = v.x * cell[0][1] + v.y * cell[1][1] + v.z * cell[2][1] + cell[3][1];
		t.z = v.x * cell[0][2] + v.y * cell[1][2] + v.z * cell[2][2] + cell[3][2];
		t.w = v.x * cell[0][3] + v.y * cell[1][3] + v.z * cell[2][3] + cell[3][3];
		return t.homogenize();
	}

	// Multiply two matrices.
	Matrix44 operator * (const Matrix44& m) const
	{
		Matrix44 t;

		//         c  r				
		//	t.cell[0][0] = cell[0][0] * m.cell[0][0] + cell[1][0] * m.cell[0][1]
		int col, row, k;


		for (col = 0; col < 4; col++) {
			for (row = 0; row < 4; row++) {
				t.cell[col][row] = 0;
				for (k = 0; k < 4; k++)
					t.cell[col][row] += cell[k][row] * m.cell[col][k];
			}
		}
		return t;
	}

	Matrix44& operator *= (const T k) // multiply by a scalar.
	{
		for(int i = 0; i < 16; i++)
			e[i] *= k;
		return *this;
	}

	Matrix44& operator += (const Matrix44<T>& m) // multiply by a scalar.
	{
		for(int i = 0; i < 16; i++)
			e[i] += m.e[i];
		return *this;
	}

	Matrix44& operator /= (const T k) // division by a scalar.
	{
		for(int i = 0; i < 16; i++)
			e[i] /= k;
		return *this;
	}

	Matrix44& operator = (const Matrix44& m)
	{
		memcpy(e, m.e, sizeof(T) * 16);
		return *this;
	}


	static Matrix44 scale(T x, T y, T z)
	{
		Matrix44 t;
		t.e[0]	= x; t.e[4]	= 0; t.e[8]	= 0; t.e[12] = 0;
		t.e[1]	= 0; t.e[5]	= y; t.e[9]	= 0; t.e[13] = 0;
		t.e[2]	= 0; t.e[6]	= 0; t.e[10]= z; t.e[14] = 0;
		t.e[3]	= 0; t.e[7]	= 0; t.e[11]= 0; t.e[15] = 1; 
		return t;
	}

	static Matrix44 scale(const Vector3<T>& vec)
	{
		Matrix44 t;
		t.e[0]	= vec.x; t.e[4]	= 0; t.e[8]	= 0; t.e[12] = 0;
		t.e[1]	= 0; t.e[5]	= vec.y; t.e[9]	= 0; t.e[13] = 0;
		t.e[2]	= 0; t.e[6]	= 0; t.e[10]= vec.z; t.e[14] = 0;
		t.e[3]	= 0; t.e[7]	= 0; t.e[11]= 0; t.e[15] = 1; 
		return t;
	}

	// angle is in radians. glStylish rotation around given vector.
	static Matrix44 rotate(T angle, const Vector3<T>& vec)
	{
		return rotate(angle, vec.x, vec.y, vec.z);
	}

	// angle is in radians. glStylish rotation around given vector (AxisAngle).
	static Matrix44 rotate(T angle, T x, T y, T z)
	{
		T c = (T)cos(angle); // Optimization.
		T s = (T)sin(angle);
		T ic = 1.0f - c;
		Matrix44 t;

		// build rotational Matrix44 around vector. GL stylish
		t.cell[0][0] = (x * x * ic) + c;
		t.cell[0][1] = (y * x * ic) + (z * s);
		t.cell[0][2] = (x * z * ic) - (y * s);
		t.cell[0][3] = 0;

		t.cell[1][0] = (x * y * ic) - (z * s);
		t.cell[1][1] = (y * y * ic) + c;
		t.cell[1][2] = (y * z * ic) + (x * s);
		t.cell[1][3] = 0;

		t.cell[2][0] = (x * z * ic) + (y * s);
		t.cell[2][1] = (y * z * ic) - (x * s);
		t.cell[2][2] = (z * z * ic) + c;
		t.cell[2][3] = 0;

		t.cell[3][0] = 0;
		t.cell[3][1] = 0;
		t.cell[3][2] = 0;
		t.cell[3][3] = 1;

		return t;
	}

	// Rotations around single axes (angle is in radians).
	static Matrix44 rotateX(T angle)
	{
		T c = cos(angle);
		T s = sin(angle);
		Matrix44 t;
		t.e[0]	= 1; t.e[4]	= 0; t.e[8]	= 0; t.e[12] = 0;
		t.e[1]	= 0; t.e[5]	= c; t.e[9]	= -s; t.e[13] = 0;
		t.e[2]	= 0; t.e[6]	= s; t.e[10]= c; t.e[14] = 0;
		t.e[3]	= 0; t.e[7]	= 0; t.e[11]= 0; t.e[15] = 1; 
		return t;
	}

	// Rotations around single axes (angle is in radians).
	static Matrix44 rotateY(T angle)
	{
		T c = cos(angle);
		T s = sin(angle);
		Matrix44 t;
		t.e[0]	= c; t.e[4]	= 0; t.e[8]	= s; t.e[12] = 0;
		t.e[1]	= 0; t.e[5]	= 1; t.e[9]	= 0; t.e[13] = 0;
		t.e[2]	= -s; t.e[6]= 0; t.e[10]= c; t.e[14] = 0;
		t.e[3]	= 0; t.e[7]	= 0; t.e[11]= 0; t.e[15] = 1; 
		return t;
	}

	// Rotations around single axes (angle is in radians).
	static Matrix44 rotateZ(T angle)
	{
		T c = cos(angle);
		T s = sin(angle);
		Matrix44 t;
		t.e[0]	= c; t.e[4]	= -s; t.e[8]= 0; t.e[12] = 0;
		t.e[1]	= s; t.e[5]	= c; t.e[9]	= 0; t.e[13] = 0;
		t.e[2]	= 0; t.e[6]	= 0; t.e[10]= 1; t.e[14] = 0;
		t.e[3]	= 0; t.e[7]	= 0; t.e[11]= 0; t.e[15] = 1; 
		return t;
	}

	static Matrix44 mirror(const Vector3<T>& n) // Spegling på plan (normal), genom origo.
	{
		Matrix44 t; 
	/*
		L(v) = v - 2(v * n)n
		L(e1) = (1, 0, 0) -2((1,0,0)*(n.x, n.y, n.z)) * (n.x, n.y, n.z) = (1-2n.x^2, -2n.x*n.y,  -2n.x*n.z)
	*/
		t.cell[0][0] = 1 - (2* (T)pow(n.x, 2));
		t.cell[1][0] = -(2*n.x*n.y);			
		t.cell[2][0] = -(2*n.x*n.z);		
		t.cell[0][1] = -(2*n.x*n.y);
		t.cell[1][1] = 1-(2* (T)pow(n.y, 2));
		t.cell[2][1] = -(2*n.y*n.z);		
		t.cell[0][2] = -(2*n.x*n.z);
		t.cell[1][2] = -(2*n.y*n.z);
		t.cell[2][2] = 1-(2* (T)pow(n.z, 2));	

		return t;
	}

	// MAKEME: kolla upp buggar på den här!!!
	// A shadow transformation.  (project a shadow on a 'plane' of the thing being multiplied with).
	static Matrix44 shadow(const Vector3<T>& l, const Vector3<T>& n, const T d)
	{
		Matrix44 t;
		T dot, lw = 1;

		// dot product of plane and light position
		dot = (n * l) + (d * lw);

		// first column
		t.e[0] = dot - l.x * n.x;
		t.e[4] = 0.0f - l.x * n.y;
		t.e[8] = 0.0f - l.x * n.z;
		t.e[12] = 0.0f - l.x * d;
		// second column
		t.e[1] = 0.0f - l.y * n.x;
		t.e[5] = dot - l.y * n.y;
		t.e[9] = 0.0f - l.y * n.z;
		t.e[13] = 0.0f - l.y * d;
		// third column
		t.e[2] = 0.0f - l.z * n.x;
		t.e[6] = 0.0f - l.z * n.y;
		t.e[10] = dot - l.z * n.z;
		t.e[14] = 0.0f - l.z * d;

		t.e[3] = 0.0f - lw * n.x;
		t.e[7] = 0.0f - lw * n.y;
		t.e[11] = 0.0f - lw * n.z;
		t.e[15] = dot - lw * d;   // dot - fLightPos[3] * fPlane[3];
		return t;
	}

	// make itself inverted (if possible).
	bool inverse()
	{
		T d = det();
		if(!d)
			return false;
		// Inverse for A is defined by:  A-inverse =  (1 / det(A)) * Adjoint(A)
		*this = adjoint();
		*this *= (1 / d);
		return true;
	}

	//Note: for ortagonal matrices then transpose = inverse!
	Matrix44 transpose() const
	{ 
		Matrix44 t;
		int col, row;
		for (col = 0; col < 4; col++) {
			for (row = 0; row < 4; row++) {
				t.cell[col][row] = cell[row][col];
			}
		}
		return t;
	}

	// returns false if Matrix44 is singular (not invertible)
	bool inverse(Matrix44 *m) const
	{
		T d = det();
		if(!d)
			return false;
		// Inverse for A is defined by:  A-inverse =  (1 / det(A)) * Adjoint(A)
		*m = adjoint() * (1 / d);
		return true;
	}

	// Returns the adjoint for Matrix44.
	Matrix44 adjoint() const
	{
		Matrix44 m, tmp;
		int r, c, r1, c1, rt, ct;

		for(r = 0; r < 4; r++) {
			for(c = 0; c < 4; c++) {

				rt = ct = 0;
				// Create tmp. (sub Matrix44 used to find a cofactor).
				for(r1 = 0; r1 < 4; r1++) {
					if(r == r1)
						continue;
					ct = 0;
					for(c1 = 0; c1 < 4; c1++) {
						if(c == c1)
							continue;
						tmp.setrc(rt, ct, cell[c1][r1]);
						ct ++;
					}
					rt ++;
				}
				// Note adjoint is transpose stylee (therefore c,r instead of r, c)
				m.setrc(c, r, (T)pow(-1, r+c) * tmp.det33() );
			}
		}
		return m;
	}

	T det33() const
	{
		// Formula for 3x3 determinants:  (a_(1,1)a_(2,2)a_(3,3))+(a_(1,2)a_(2,3)a_(3,1))+(a_(1,3)a_(2,1)a_(3,2)) -(a_(3,1)a_(2,2)a_(1,3)) -(a_(3,2)a_(2,3)a_(1,1)) -(a_(3,3)a_(2,1)a_(1,2))
		return (a_(1,1)*a_(2,2)*a_(3,3))+(a_(1,2)*a_(2,3)*a_(3,1))+(a_(1,3)*a_(2,1)*a_(3,2)) -(a_(3,1)*a_(2,2)*a_(1,3)) -(a_(3,2)*a_(2,3)*a_(1,1)) -(a_(3,3)*a_(2,1)*a_(1,2));
	}

	// MAKEME: complete.
	// Find the determinant by cofactor expansion.
	T det() const
	{
		Matrix44 ad;
		int i;
		T d;
		// Start by calculating the adjoint....
		ad = adjoint();
		d = 0;
		for(i = 0; i < 4; i++) {
			d += cell[i][3] * ad.cell[3][i];
		}
		return d;
	}


	static Matrix44 lookAt(const Vector3<T>& eye, const Vector3<T>& center, Vector3<T> up) 
	{
		Matrix44 t;
		Vector3<T> f, u, s;

		up.normalize();

		f = (center - eye).normalize();
		s = (f ^ up).normalize();
		u = (s ^ f);
		f *= -1;

		t.e[0]	= s[0]; t.e[4]	= s[1]; t.e[8]	= s[2]; t.e[12] = 0;
		t.e[1]	= u[0]; t.e[5]	= u[1]; t.e[9]	= u[2]; t.e[13] = 0;
		t.e[2]	= f[0]; t.e[6]	= f[1]; t.e[10] = f[2]; t.e[14] = 0;
		t.e[3]	= 0;    t.e[7]	= 0;    t.e[11] = 0;    t.e[15] = 1; 
		return t * translate(-eye.x, -eye.y, -eye.z);
	}

	// Angle is given in degrees.
	static Matrix44 perspective(T fov, T aspect, T nearClip, T farClip) {
		Matrix44 t;
		fov = fov / 2 / (180 / (float)M_PI); // convert to radians!
		T a = farClip / (nearClip - farClip);
		T b = (nearClip * farClip) /  (nearClip - farClip);
		T k = 1 / (T)tan(fov);
		t.e[0]	= k / aspect;
		t.e[1]	= 0;
		t.e[2]	= 0;
		t.e[3]	= 0;

		t.e[4]	= 0;
		t.e[5]	= k;
		t.e[6]	= 0;
		t.e[7]	= 0;
		
		t.e[8]	= 0;
		t.e[9]	= 0;
		t.e[10]	= a;
		t.e[11]	= -1;

		t.e[12]	= 0;
		t.e[13]	= 0;
		t.e[14]	= b;
		t.e[15]	= 0;
		return t;

	}

	
	// Used to debug.
	void toString(char *str) const {
		char row[256];
		str[0] = 0;
		for(int i = 0; i < 4; i ++) {
			for(int j = 0; j < 4; j ++) {
				sprintf(row, "%8.1f", cell[j][i]);
				strcat(str, row);
			}
			strcat(str, "\n");
		}
	}


	#define ROT(a,i,j,k,l) g=a[i][j];h=a[k][l];a[i][j]=g-s*(h+g*tau);a[k][l]=h+s*(g-h*tau);
	#define MAX_ROTATIONS 60


//	bool Matrix44::internalJacobi(const Matrix44 a, float w[3], Vector3<T> v[3])
	bool internalJacobi(T w[3], Vector3<T> v[3]) 
	{
		int i, j, k, iq, ip;
		T tresh, theta, tau, t, sm, s, h, g, c;
		T b[3], z[3], tmp;

		// initialize
		for (ip=0; ip<3; ip++) {
			for (iq=0; iq<3; iq++)
				v[ip][iq] = 0.0;
			v[ip][ip] = 1.0;
		}
		for (ip=0; ip<3; ip++) {
			b[ip] = w[ip] = cell[ip][ip];
			z[ip] = 0.0;
		}

		// begin rotation sequence
		for (i=0; i<MAX_ROTATIONS; i++) {
			sm = 0.0;
			for (ip=0; ip<2; ip++) 	{
				for (iq=ip+1; iq<3; iq++) sm += fabsf(cell[ip][iq]);
			}
			if (sm == 0.0)
				break;

			if (i < 4)
				tresh = float(0.2)*sm/(9);
			else
				tresh = 0.0;
			for (ip=0; ip<2; ip++) 	{
				for (iq=ip+1; iq<3; iq++) {
					g = float(100.0)*fabsf(cell[ip][iq]);
					if (i > 4 && (fabsf(w[ip])+g) == fabsf(w[ip]) && (fabsf(w[iq])+g) == fabsf(w[iq])) {
						cell[ip][iq] = 0.0;
					}
					else if (fabsf(cell[ip][iq]) > tresh) {
						h = w[iq] - w[ip];
						if ( (fabsf(h)+g) == fabsf(h))
							t = (cell[ip][iq]) / h;
						else {
							theta = float(0.5)*h / (cell[ip][iq]);
							t = float(1.0) / (fabsf(theta)+sqrtf(float(1.0)+theta*theta));
							if (theta < 0.0) t = -t;
						}
						c = float(1.0) / sqrtf(float(1)+t*t);
						s = t*c;
						tau = s/(float(1.0)+c);
						h = t*cell[ip][iq];
						z[ip] -= h;
						z[iq] += h;
						w[ip] -= h;
						w[iq] += h;
						cell[ip][iq]=0.0;
						for (j=0;j<ip-1;j++) 
							ROT(cell,j,ip,j,iq)
						for (j=ip+1;j<iq-1;j++) 
							ROT(cell,ip,j,j,iq)
						for (j=iq+1; j<3; j++) 
							ROT(cell,ip,j,iq,j)
						for (j=0; j<3; j++) 
							ROT(v,j,ip,j,iq)

					}
				}
			}

			for (ip=0; ip<3; ip++) 	{
				b[ip] += z[ip];
				w[ip] = b[ip];
				z[ip] = 0.0;
			}
		}

		if ( i >= MAX_ROTATIONS ) {
			return false;
		}

		// sort eigenfunctions
		for (j=0; j<3; j++) {
			k = j;
			tmp = w[k];
			for (i=j; i<3; i++)	{
				if (w[i] >= tmp) 
					{
					k = i;
					tmp = w[k];
				}
			}
			if (k != j) {
				w[k] = w[j];
				w[j] = tmp;
				for (i=0; i<3; i++) 
					{
					tmp = v[j][i];
					v[j][i] = v[k][i];
					v[k][i] = tmp;
				}
			}
		}
		// insure eigenvector consistency (i.e., Jacobi can compute vectors that
		// are negative of one another (.707,.707,0) and (-.707,-.707,0). This can
		// reek havoc in hyperstreamline/other stuff. We will select the most
		// positive eigenvector.
		int numPos;
		for (j=0; j<3; j++)	{
			for (numPos=0, i=0; i<3; i++) if ( v[i][j] >= 0.0 ) numPos++;
			if ( numPos < 2 ) for(i=0; i<3; i++) v[i][j] *= -1.0;
		}
		return true;
	}

	#undef ROT
	#undef MAX_ROTATIONS

	// Desc: Solves the eigenvalues and eigenvectors of a float symmetric 3x3 matrix. If the
	// returnvalue is false, no solution exists.
	bool SolveEigenvalues(float eigenvalues[3] , Vector3<T> eigenvectors[3])
	{
		Matrix44<T> temp;
		temp = *this;
		return temp.internalJacobi(eigenvalues, eigenvectors);
	}

	const Matrix44& operator = (const T k)
	{
		int i;
		for(i = 0; i < 16; i++) 
			e[i] = k;
		return *this;
	}


	static Matrix44 covariance(Vector3<T> p[], int num)
	{
		int i;
		Vector3<T> m = 0;
		for(i = 0; i < num; i++)
			m += p[i];
		m /= (float)num; // calculate "medelpunkt"
		// Create a 3x3 covariance matrix (altough it contained in a 4x4 only the 3x3 part , upper left corner , is valid.)
		Matrix44<T> Ci, C;
		C = 0;
		for(i = 0; i < num; i++) {
			float dx = p[i].x - m.x;
			float dy = p[i].y - m.y;
			float dz = p[i].z - m.z;

			Ci.cell[0][0] = dx * dx;
			Ci.cell[0][1] = dx * dy;
			Ci.cell[0][2] = dx * dz;

			Ci.cell[1][0] = dy * dx;
			Ci.cell[1][1] = dy * dy;
			Ci.cell[1][2] = dy * dz;

			Ci.cell[2][0] = dz * dx;
			Ci.cell[2][1] = dz * dy;
			Ci.cell[2][2] = dz * dz;

			C += Ci;
		}
		C /= (float)num;
		return C;
	}

	// Principal Component analysis
	static bool PCA(Vector3<T> vec[3], Vector3<T> p[], int num)
	{
		Matrix44<T> m;
		float w[3]; // eigenvalues (not used)
		m = Matrix44<T>::covariance(p, num);
		return m.internalJacobi(w, vec);
	}

	const Matrix44<T>&  rowInterchange(int from, int to) {
		int i;
		T tmp[4];  // temporary storage for row.
		for(i = 0; i < 4; i++) {
			tmp[i] = cell[i][to];
			cell[i][to] = cell[i][from];
			cell[i][from] = tmp[i];
		}
		return *this;
	}

	const Matrix44<T>&  addRowMultiple(int from, int to, T mult) {
		int i;
		for(i = 0; i < 4; i++)
			cell[i][to] += cell[i][from] * mult;
		return *this;
	}


	// manipulates the matrix into reduced row echelon form.
	const Matrix44<T>& reducedRowEchelon()
	{
		int leftmostColumn;
		int i, top, row;
		T a;

		for(top = 0; top < 3; top++) {
			// Locate the leftmost column that does not consist entirely of zeroes.
			bool zero = true;
			for(leftmostColumn = 0; leftmostColumn < 4 && zero; leftmostColumn++) {
				for(row = top; row < 4; row++) {
					a = cell[leftmostColumn][row];
					if(a != 0) {
						zero = false;
						leftmostColumn --; // since outer loop will increment it once even when zero flag is set.
						break;
					}
				}
			}
			// Move row with leftmost nonzero column to top.
			rowInterchange(row, top); 

			// Multiply the first row with 1 / a
			for(i = 0; i  < 4; i ++) {
				cell[i][top] *= 1 / a;
			}

			// Add multiples of the top row to the rows below so that all entries below the leading 1 become zeros.
			for(i = top + 1; i < 4; i++) { // for all the below rows.
				addRowMultiple(top, i, -cell[leftmostColumn][i]);
			}
			// Now cover the tpå row (by for loop increment) and begin again.
		}

		return *this;
	}

	void characteristicEquation(T& l3, T& l2, T& l1, T& l0) const
	{
		float a, b, c,  x, y;
		Matrix44<T> m = *this;
		m *= -1;

		// 3x3 determinant evalutation to get eigenvalues using formula: det(Il - A) = 0
		// l - a    e[4]   e[8] ?
		//  e[1]   l - b   e[9] ?
		//  e[2]    e[6]  l - c ?
		//   ?       ?      ?   ?
		// l = lambda, l3 = (l3 * lambda^3), l2 = (l2 * lambda^2) ... in the characteristic polynomial.
		// (l - a) * (l - b) * (l - c) = l3 
		a = m.e[0];
		b = m.e[5];
		c = m.e[10];
		x = a + b;
		y = a * b;
		// find the factors for each lamda^x
		l3 = 1;
		l2 = x + c;
		l1 = (x * c) + y - (m.e[2] * m.e[8]) - (m.e[6] * m.e[9]) - (m.e[1] * m.e[4]);
		l0 = (y * c) + (m.e[4] * m.e[9] * m.e[2]) + (m.e[8] * m.e[1] * m.e[6]) - (m.e[2] * m.e[8] * b) + (m.e[6] * m.e[9] * a) - (m.e[1] * m.e[4] * c);  
	}

	// Cast operators.
	operator int*() {
		return (int*)e;
	} 
	operator float*() {
		return (float*)e;
	} 
	operator double*() {
		return (double*)e;
	}

	// DEBUG: should be protected.
	union {
		T cell[4][4]; // four by four cells.  cell[column][row]
		T e[16]; 
	};
protected:

};


// Used with debugging.
template <class T>
std::ostream& operator << (std::ostream& s, const Matrix44<T>& m)
{
	char buf[1024];
	m.toString(buf);
	s << buf;
	return s;
}


#endif

