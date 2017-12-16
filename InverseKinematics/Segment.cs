using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Threading.Tasks;

namespace InverseKinematics
{
    [Serializable]
    public class Segment
    {
        static int WIDTH = 5;
        public Vector2D a;
        public float len;
        public float angle;
        public float selfAngle;
        public Color color = Color.Black;

        public Segment parent;
        public ArrayList children;

        public Vector2D b;

        public bool is_last = false;

        public Segment(float x, float y, float len_, float angle_)
        {
            a = new Vector2D(x, y);
            len = len_;
            angle = angle_;
            calculateB();
            parent = null;
            children = new ArrayList();
        }

        public Segment(Segment parent_, float len_, float angle_)
        {
            parent = parent_;
            a = new Vector2D(parent.b.x, parent.b.y);
            len = len_;
            angle = angle_;
            selfAngle = angle;
            calculateB();
            
            parent_.children.Add(this);
            children = new ArrayList();
        }

        public void set_roots_angle_and_length(float angle, float len)
        {
            this.angle = angle;
            this.selfAngle = angle;
            this.len = len;
            calculateB();
        }

        public override string ToString()
        {
            return "Segment< A:(" + this.a.x + "," + this.a.y + ") Count: " + Convert.ToString(this.children.Count) + ">";
        }

        public void calculateB()
        {
            float dx = Convert.ToSingle(len * Math.Cos(angle));
            float dy = Convert.ToSingle(len * Math.Sin(angle));
            b = new Vector2D(a.x + dx, a.y + dy);

            foreach (Segment item in this.children)
            {
                item.a = b;
            }
        }

        public void move()
        {
            selfAngle = selfAngle + 0.05f;
        }


        public void update()
        {
            angle = selfAngle;
            if (parent != null)
            {
                a = new Vector2D(parent.b.x, parent.b.y);
                angle += parent.angle;
            }
            calculateB();
        }

        public void show(Graphics g, Color color)
        {
            Pen pen = new Pen(color, WIDTH);
            g.DrawLine(pen, a.x, a.y, b.x, b.y);
            g.FillEllipse(Brushes.Red, a.x - 5, a.y - 5, 10, 10);

            g.FillEllipse(Brushes.Red, b.x - 5, b.y - 5, 10, 10);

        }

        public void save()
        {

        }

        public void load()
        {

        }

        internal bool clicked(float ex, float ey)
        {
            bool result = false;

            

            return result;
        }
    }
}
