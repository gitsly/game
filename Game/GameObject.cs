using System;
using System.Runtime.InteropServices;
using SlimDX;

namespace Game
{
    public class GameObject
    {
        public Matrix Transformation // Returns absolute transformation for gameobject in world space
        {
            get
            {
                return Matrix.Identity;
            }
        }

        Vector3 position;
        Vector3 rotation;

        public RenderObject RenderObj { get; set; }  

        public GameObject()
        {

        }

    }
}
