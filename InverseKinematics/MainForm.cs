using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InverseKinematics
{
    public partial class MainForm : Form
    {
        Segment seg1, seg2, root, selected1, selected2;
        bool clickOne, clickTwo;
        Vector2D start, end;

        public MainForm()
        {
            InitializeComponent();
        }

        static float Radians(float angle)
        {
            return (float) (Math.PI / 180) * angle;
        }

        static float CalculateAngle(float x1, float y1, float x2, float y2)
        {
            var w = x2 - x1;
            var h = y2 - y1;

            var atan = Math.Atan(h / w) / Math.PI * 180;
            if (w < 0 || h < 0)
                atan += 180;
            if (w > 0 && h < 0)
                atan -= 180;
            if (atan < 0)
                atan += 360;

            return (float) atan % 360;
        }

        static float CalculateLength(Vector2D start, Vector2D end)
        {
            return (float) Math.Sqrt(Math.Pow((end.y - start.y), 2) + Math.Pow((end.x - start.x), 2));
        }

        static void print(object o)
        {
            Console.WriteLine(Convert.ToString(o));
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(600, 400);
            seg1 = new Segment(50, 200, 100, Radians(60));
            seg2 = new Segment(seg1, 100, Radians(60));

            clickOne = false;
            clickTwo = false;
            start = new Vector2D();
            end = new Vector2D();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            seg1.show(e.Graphics);
            seg2.show(e.Graphics);

        }

        private void find(Segment seg, float ex, float ey)
        {

            for (int i = 0; i < seg.children.Count; i++)
            {
                Segment child = (Segment) seg.children[i];
                find(child, ex, ey);
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            Graphics g = CreateGraphics();

            // root creation
            if (root == null)
            {
                // root start point
                if (!clickOne)
                {
                    start.set(e.X, e.Y);
                    g.FillRectangle(Brushes.Red, start.x - 5, start.y - 5, 10, 10);
                    clickOne = true;
                }
                // root end point, calculating angle and lenght
                else if (!clickTwo)
                {
                    end.set(e.X, e.Y);
                    float angle = CalculateAngle(start.x, start.y, end.x, end.y);
                    float len = CalculateLength(start, end);

                    root = new Segment(start.x, start.y, len, Radians(angle));
                    root.show(g);

                    clickOne = false;
                    clickTwo = false;
                    start = new Vector2D();
                    end = new Vector2D();

                }
            }
            else
            {
                //// TODO: creating child between 2 Segments
                Segment current = root;
                find(current, e.X, e.Y);

                // creating children - parent was not selected
                if (selected1 == null && selected2 == null)
                {
                    
                    if (root.children.Count == 0)
                    {
                        start.set(root.b.x, root.b.y);
                        end.set(e.X, e.Y);
                        float angle = CalculateAngle(start.x, start.y, end.x, end.y);
                        float len = CalculateLength(start, end);

                        Segment child = new Segment(start.x, start.y, len, Radians(angle));
                        root.children.Add(child);
                        child.show(g);
                    }
                    else
                    {
                        current = root;
                        while (current.children.Count != 0)
                        {
                            current = (Segment) current.children[0];
                        }

                        start.set(current.b.x, current.b.y);
                        end.set(e.X, e.Y);
                        float angle = CalculateAngle(start.x, start.y, end.x, end.y);
                        float len = CalculateLength(start, end);
                        
                        Segment child = new Segment(start.x, start.y, len, Radians(angle));
                        current.children.Add(child);
                        child.show(g);
                    }



                }
                else
                {
                    // add new child to selected segment
                }
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            seg1.move();
            seg1.update();
            seg2.update();
            Invalidate();
            //Debug.WriteLine(Convert.ToString(seg1.angle));
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {

        }
    }
}
