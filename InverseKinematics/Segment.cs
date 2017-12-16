﻿using System;
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

        // Constructors
        public Segment(float x, float y, float len_, float angle_)
        {
            a = new Vector2D(x, y);
            len = len_;
            angle = angle_;
            parent = null;
            children = new ArrayList();
            calculateB();
        }

        public Segment(Segment parent_, float len_, float angle_)
        {
            parent = parent_;
            a = new Vector2D(parent.b.x, parent.b.y);
            len = len_;
            angle = angle_;
            selfAngle = angle;
            parent_.children.Add(this);
            children = new ArrayList();

            calculateB();
        }

        public override string ToString()
        {
            return "Segment< A:(" + this.a.x + "," + this.a.y + ") Count: " + Convert.ToString(this.children.Count) + ">";
        }

        public void calculateB()
        {
            // calculates automatically B by the length and the angle

            float dx = Convert.ToSingle(len * Math.Cos(angle));
            float dy = Convert.ToSingle(len * Math.Sin(angle));
            b = new Vector2D(a.x + dx, a.y + dy);

            foreach (Segment item in this.children)
            {
                item.a = b;
            }
        }

        public void calculateBAndChangeChildrenA()
        {
            // calculate B and changes children A point by the previous change - used in FK
            calculateB();

            foreach (Segment item in this.children)
            {
                item.a = b;
            }
        }

        // repaints color of a segment
        public void show(Graphics g, Color color)
        {
            Pen pen = new Pen(color, WIDTH);
            g.DrawLine(pen, a.x, a.y, b.x, b.y);
            g.FillEllipse(Brushes.Red, a.x - 5, a.y - 5, 10, 10);

            g.FillEllipse(Brushes.Red, b.x - 5, b.y - 5, 10, 10);

        }
    }
}
