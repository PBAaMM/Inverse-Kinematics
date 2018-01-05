using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InverseKinematics
{
    // A class to describe a two dimensional vector. 
    [Serializable]
    public class Vector2D
    {
        public float x;
        public float y;

        // Empty constructor. 
        public Vector2D()
        {
        }

        // Constructor for a 2D vector. 
        public Vector2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // Get a copy of this vector. 
        public Vector2D get()
        {
            return new Vector2D(x, y);
        }

        // Set x, y coordinates. 
        public void set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // Set x, and y coordinates from a Vector2D object.
        public void set(Vector2D v)
        {
            x = v.x;
            y = v.y;
        }

        // Calculate the magnitude (length) of the vector.
        public float mag()
        {
            return (float) Math.Sqrt(x * x + y * y);
        }

        // Add a vector to this vector.
        public void add(Vector2D v)
        {
            x += v.x;
            y += v.y;
        }

        public void add(float x, float y)
        {
            this.x += x;
            this.y += y;
        }

        /**
         * Add two vectors 
         * @param v1 a vector 
         * @param v2 another vector 
         * @return a new vector that is the sum of v1 and v2 
         */
        static public Vector2D add(Vector2D v1, Vector2D v2)
        {
            return add(v1, v2, null);
        }


        /**
         * Add two vectors into a target vector 
         * @param v1 a vector 
         * @param v2 another vector 
         * @param target the target vector (if null, a new vector will be created) 
         * @return a new vector that is the sum of v1 and v2 
         */
        static public Vector2D add(Vector2D v1, Vector2D v2, Vector2D target)
        {
            if (target == null)
            {
                target = new Vector2D(v1.x + v2.x, v1.y + v2.y);
            }
            else
            {
                target.set(v1.x + v2.x, v1.y + v2.y);
            }
            return target;
        }

        // Subtract a vector from this vector.
        public void sub(Vector2D v)
        {
            x -= v.x;
            y -= v.y;
        }

        public void sub(float x, float y)
        {
            this.x -= x;
            this.y -= y;
        }

        static public Vector2D sub(Vector2D v1, Vector2D v2, Vector2D target)
        {
            if (target == null)
            {
                target = new Vector2D(v1.x - v2.x, v1.y - v2.y);
            }
            else
            {
                target.set(v1.x - v2.x, v1.y - v2.y);
            }
            return target;
        }

        static public Vector2D sub(Vector2D v1, Vector2D v2)
        {
            return sub(v1, v2, null);
        }

        // Multiply this vector by a scalar.
        public void mult(float n)
        {
            x *= n;
            y *= n;
        }

        // Multiply each element of one vector by the elements of another vector.
        public void mult(Vector2D v)
        {
            x *= v.x;
            y *= v.y;
        }

        // Divide this vector by a scalar 
        public void div(float n)
        {
            x /= n;
            y /= n;
        }

        // Divide each element of one vector by the elements of another vector. 
        public void div(Vector2D v)
        {
            x /= v.x;
            y /= v.y;
        }

        /**
          * Normalize the vector to length 1 (make it a unit vector) 
          */
        public void normalize()
        {
            float m = mag();
            if (m != 0 && m != 1)
            {
                div(m);
            }
        }
    }
}
