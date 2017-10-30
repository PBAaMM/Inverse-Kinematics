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
        Segment seg1, seg2, root, selected1, selected2, last, current;
        bool clickOne, clickTwo, clickTree, creation, forward, inverse;
        int click;
        Vector2D start, end;
        Graphics g;

        public MainForm()
        {
            InitializeComponent();
            g = CreateGraphics();
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

            click = 0;
            clickOne = false;
            clickTwo = false;
            clickTree = false;
            creation = false;
            forward = false;
            inverse = false;
            start = new Vector2D();
            end = new Vector2D();
        }

        private void skeleton_creation_Click(object sender, EventArgs e)
        {
            creation = true;
            forward = false;
            inverse = false;
            status_bar.Text = "Create skeleton by clicking. Select parent and it's one child to create a new branch.";
        }

        private void forward_kinematics_Click(object sender, EventArgs e)
        {
            creation = false;
            forward = true;
            inverse = false;
            status_bar.Text = "Click on bone and use forward kinematics by moving the mouse cursor.";
        }

        private void inverse_kinematics_Click(object sender, EventArgs e)
        {
            creation = false;
            forward = false;
            inverse = true;
            status_bar.Text = "Select beginning and end of inverse kinematics sequence. Then click elsewhere to use IK.";
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            seg1.show(e.Graphics, Color.Black);
            seg2.show(e.Graphics, Color.Black);
        }

        private float distance(float aX, float aY, float bX, float bY)
        {
            return Convert.ToSingle(Math.Sqrt((Math.Pow(bX - aX, 2) + Math.Pow(bY - aY, 2))));
        }

        private void find_selected(Segment seg, float ex, float ey)
        {
            float distanceAX = distance(seg.a.x, seg.a.y, ex, ey);
            float distanceXB = distance(ex, ey, seg.b.x, seg.b.y);
            float distanceAB = distance(seg.a.x, seg.a.y, seg.b.x, seg.b.y);

            int cross = Convert.ToInt32(distanceAB) - Convert.ToInt32(distanceAX + distanceXB);
            if (Math.Abs(cross) < 2)
            {
                if (selected1 == null)
                    selected1 = seg;
                else if (selected2 == null)
                    selected2 = seg;
            }

            for (int i = 0; i < seg.children.Count; i++)
            {
                Segment child = (Segment) seg.children[i];
                find_selected(child, ex, ey);
            }
        }

        private Segment create_next(Vector2D start, Vector2D end)
        {
            float angle = CalculateAngle(start.x, start.y, end.x, end.y);
            float len = CalculateLength(start, end);
            return new Segment(start.x, start.y, len, Radians(angle));
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            // SKELETON CREATION
            if (creation)
            {
                // root creation
                if (root == null)
                {
                    click += 1;
                    // root start point when clicked first time
                    if (click == 1)
                    {
                        start.set(e.X, e.Y);
                        g.FillRectangle(Brushes.Red, start.x - 5, start.y - 5, 10, 10);

                    }
                    // root end point, when clicked second time, calculating angle and lenght
                    else if (click == 2)
                    {
                        end.set(e.X, e.Y);
                        root = create_next(start, end);
                        //root.show(g, Color.Black);
                        last = root;
                        last.show(g, Color.Orange);
                        click = 0;
                    }
                }
                else
                {
                    // find first selected bone
                    if (!clickOne)
                    {
                        current = root;
                        find_selected(current, e.X, e.Y);
                        if (selected1 != null)
                            clickOne = true;
                    }
                    // find second selected bone
                    else if (!clickTwo)
                    {
                        current = root;
                        find_selected(current, e.X, e.Y);
                        if (selected2 != null)
                            clickTwo = true;
                    }
                    else
                    {
                        clickTree = true;
                    }
                    // if not selected1 and selected2, creating child to the last one added
                    if (selected1 == null && selected2 == null)
                    {
                        start.set(last.b.x, last.b.y);
                        end.set(e.X, e.Y);
                        Segment child = create_next(start, end);
                        last.children.Add(child);
                        last.show(g, Color.Black);
                        last = child;
                        last.show(g, Color.Orange);
                        click = 0;
                    }
                    else
                    {
                        // add new child to the first selected bone
                        if (selected1 != null)
                        {
                            selected1.show(g, Color.Blue);
                        }
                        if (selected2 != null)
                        {
                            selected2.show(g, Color.Blue);
                            if (clickTree)
                            {
                                if (selected1.b.x != selected2.a.x)
                                {
                                    status_bar.Text = "Wrong! Choose two bones in hierarchy - Parent and it's one child.";
                                }
                                else
                                {
                                    start.set(selected1.b.x, selected1.b.y);
                                    end.set(e.X, e.Y);
                                    Segment child = create_next(start, end);
                                    selected1.children.Add(child);
                                    last.show(g, Color.Black);
                                    last = child;
                                    last.show(g, Color.Orange);
                                }
                                selected1.show(g, Color.Black);
                                selected2.show(g, Color.Black);
                                selected1 = null;
                                selected2 = null;
                                clickOne = false;
                                clickTwo = false;
                                clickTree = false;
                            }
                        }
                    }
                }
            }
            // FORWARD KINEMATICS 
            else if (forward)
            {
                // find first selected bone
                if (!clickOne)
                {
                    current = root;
                    find_selected(current, e.X, e.Y);
                    if (selected1 != null)
                        clickOne = true;
                }
                if (selected1 != null)
                {
                    selected1.show(g, Color.Blue);
                }
            }
            // INVERSE KINEMATICS 
            else if (inverse)
            {
                // find first selected bone
                if (!clickOne)
                {
                    current = root;
                    find_selected(current, e.X, e.Y);
                    if (selected1 != null)
                        clickOne = true;
                }
                // find second selected bone
                else if (!clickTwo)
                {
                    current = root;
                    find_selected(current, e.X, e.Y);
                    if (selected2 != null)
                        clickTwo = true;
                }
                else
                {
                    clickTree = true;
                }

                // add new child to the first selected bone
                if (selected1 != null)
                {
                    selected1.show(g, Color.Blue);
                }
                if (selected2 != null)
                {
                    selected2.show(g, Color.Blue);
                    if (clickTree)
                    {
                        // TODO: apply inverse kinematics
                        print("Folow e.X and e.Y");
                    }
                }
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (forward)
            {
                if (clickOne)
                {
                    if (selected1 != null)
                    {
                        // TODO: apply forward kinematics
                        print("Apply FK");
                    }
                }
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (forward)
            {
                if (clickOne)
                {
                    if (selected1 != null)
                    {
                        selected1.show(g, Color.Black);
                        selected1 = null;
                    }
                    clickOne = false;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //seg1.move();
            //seg1.update();
            //seg2.update();
            //Invalidate();
            //Debug.WriteLine(Convert.ToString(seg1.angle));
        }
    }
}
