using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace InverseKinematics
{
    public partial class MainForm : Form
    {
        Segment root, selected1, selected2, last, current;
        bool clickOne, clickTwo, clickThree, creation, forward, inverse;
        int click;
        Vector2D start, end, start_base;
        Graphics g;
        Random rnd;
        int n = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Size = new Size(600, 400);
            g = CreateGraphics();
            rnd = new Random();

            click = 0;
            clickOne = false;
            clickTwo = false;
            creation = false;
            forward = false;
            inverse = false;
            start_base = new Vector2D();
            start = new Vector2D();
            end = new Vector2D();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (root != null)
                draw(root, root.children);
        }
        
        public void draw(Segment node, ArrayList children)
        {
            // draws skeleton
            if (children.Count > 0)
            {
                foreach (Segment child in children)
                {
                    g.DrawLine(new Pen(node.color, 5), node.a.x, node.a.y, child.a.x, child.a.y);
                    draw(child, child.children);
                }
                g.FillEllipse(Brushes.Red, node.a.x - 5, node.a.y - 5, 10, 10);
            }
            else
            {
                g.DrawLine(new Pen(node.color, 5), node.a.x, node.a.y, node.b.x, node.b.y);
                g.FillEllipse(Brushes.Red, node.a.x - 5, node.a.y - 5, 10, 10);
                g.FillEllipse(Brushes.Red, node.b.x - 5, node.b.y - 5, 10, 10);
            }
        }


        static void print(object o)
        {
            Console.WriteLine(Convert.ToString(o));
        }

        float CalculateAngle(float x1, float y1, float x2, float y2)
        {
            // calculate angle when creating new segment
            var dx = x2 - x1;
            var dy = y2 - y1;

            var angleRadians = Math.Atan2(dy, dx);
            return (float)angleRadians;
        }

        private float CalculateLength(float aX, float aY, float bX, float bY)
        {
            // calculate length between 2 points
            return Convert.ToSingle(Math.Sqrt((Math.Pow(bX - aX, 2) + Math.Pow(bY - aY, 2))));
        }

        private void find_selected(ref Segment seg, float ex, float ey)
        {
            // finds bone on which has been clicked
            float distanceAX = CalculateLength(seg.a.x, seg.a.y, ex, ey);
            float distanceXB = CalculateLength(ex, ey, seg.b.x, seg.b.y);
            float distanceAB = CalculateLength(seg.a.x, seg.a.y, seg.b.x, seg.b.y);

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
                find_selected(ref child, ex, ey);
            }
        }

        private Segment find_clicked(ref Segment seg, float ex, float ey)
        {
            // finds bone on which has been clicked
            float distanceAX = CalculateLength(seg.a.x, seg.a.y, ex, ey);
            float distanceXB = CalculateLength(ex, ey, seg.b.x, seg.b.y);
            float distanceAB = CalculateLength(seg.a.x, seg.a.y, seg.b.x, seg.b.y);

            int cross = Convert.ToInt32(distanceAB) - Convert.ToInt32(distanceAX + distanceXB);
            if (Math.Abs(cross) < 2)
            {
                return seg;
            }

            for (int i = 0; i < seg.children.Count; i++)
            {
                Segment child = (Segment)seg.children[i];
                Segment result = find_clicked(ref child, ex, ey);
                if (result != null)
                    return result;
            }
            return null;
        }

        private Segment create_next(Vector2D start, Vector2D end)
        {
            // creates new segment in hierarchy
            float angle = CalculateAngle(start.x, start.y, end.x, end.y);
            float len = CalculateLength(start.x, start.y, end.x, end.y);
            return new Segment(start.x, start.y, len, angle);
        }



        // MOUSE EVENTS
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > 120)
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
                            g.FillEllipse(Brushes.Red, start.x - 5, start.y - 5, 10, 10);
                        }
                        // root end point, when clicked second time, calculating angle and lenght
                        else if (click == 2)
                        {
                            end.set(e.X, e.Y);
                            root = create_next(start, end);

                            g.DrawLine(new Pen(root.color, 5), root.a.x, root.a.y, root.b.x, root.b.y);
                            g.FillEllipse(Brushes.Red, root.a.x - 5, root.a.y - 5, 10, 10);
                            g.FillEllipse(Brushes.Red, root.b.x - 5, root.b.y - 5, 10, 10);

                            last = root;
                            click = 0;
                        }
                    }
                    else
                    {
                        // find first selected bone
                        if (!clickOne)
                        {
                            current = root;
                            //find_selected(ref current, e.X, e.Y);
                            selected1 = find_clicked(ref current, e.X, e.Y);

                            if (selected1 != null)
                            {
                                selected1.show(g, Color.Blue);
                                clickOne = true;
                            }
                        }
                        // add new bone to the selected1
                        else
                        {
                            start.set(selected1.b.x, selected1.b.y);
                            end.set(e.X, e.Y);

                            Segment child = create_next(start, end);
                            // set child's parent
                            child.parent = selected1;
                            // add child to parent's children
                            selected1.children.Add(child);
                            // show them
                            selected1.show(g, Color.Black);
                            child.show(g, Color.Orange);
                        
                            if (last != null)
                            {
                                last.show(g, Color.Black);
                            }
                            last = child;

                            selected1 = null;
                            clickOne = false;
                        }

                        // if not selected1 and selected2, creating child to the last one added
                        if (selected1 == null && selected2 == null)
                        {
                            if (last != null)
                            {
                                start.set(last.b.x, last.b.y);
                                end.set(e.X, e.Y);

                                Segment child = create_next(start, end);
                                child.show(g, Color.Orange);
                                child.parent = last;

                                last.children.Add(child);
                                last.show(g, Color.Black);
                                last = child;
                            }
                        }
                    }
                }
                // FORWARD KINEMATICS 
                else if (forward)
                {
                    if (root != null)
                    {
                        // find first selected bone
                        if (!clickOne)
                        {
                            current = root;
                            find_selected(ref current, e.X, e.Y);
                            if (selected1 != null)
                            {
                                selected1.show(g, Color.Blue);
                                clickOne = true;
                            }
                        }
                    }

                }
                // INVERSE KINEMATICS 
                else if (inverse)
                {
                    if (root != null)
                    {
                        // find first selected bone
                        if (!clickOne)
                        {
                            current = root;
                            find_selected(ref current, e.X, e.Y);
                            if (selected1 != null)
                            {
                                selected1.show(g, Color.Blue);
                                clickOne = true;
                            }
                        }
                        // find second selected bone
                        else if (!clickTwo)
                        {
                            current = root;
                            find_selected(ref current, e.X, e.Y);
                            if (selected2 != null)
                            {
                                selected2.show(g, Color.Blue);
                                clickTwo = true;
                            }
                        }
                        else
                        {
                            // apply IK
                            print("Folow e.X and e.Y");
                            g.FillEllipse(Brushes.Green, e.X - 5, e.Y - 5, 10, 10);

                            start_base.set(selected1.a.x, selected1.a.y);

                            print("GAAGAG");

                            clickThree = true;
                            //reset_selected();
                        }
                    }
                }
            }
        }

        public void reset_selected()
        {
            // resets used variables
            if(selected1 != null)
            {
                selected1.show(g, Color.Black);
                selected1 = null;
            }
                
            if(selected2 != null)
            {
                selected2.show(g, Color.Black);
                selected2 = null;
            }
 
            clickOne = false;
            clickTwo = false;
            clickThree = false;
        }

        private float DegreeToRadian(float angle)
        {
            return Convert.ToSingle(Math.PI * angle / 180.0);
        }

        private float RadianToDegree(float angle)
        {
            return Convert.ToSingle(angle * (180.0 / Math.PI));
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (forward)
            {
                if (clickOne)
                {
                    if (selected1 != null)
                    {
                        // apply forward kinematics

                        float a = CalculateAngle(selected1.a.x, selected1.a.y, e.X, e.Y);
                        float da = selected1.angle - a;

                        //selected1.angle = a;
                        //selected1.calculateBAndChangeChildrenA();
                        //selected1.show(g, Color.Blue);




                        /*foreach (Segment item in selected1.children)
                        {
                            q.Enqueue(item);
                        }*/

                        float centerX = selected1.a.x;
                        float centerY = selected1.a.y;

                        var dx = e.X - centerX;
                        var dy = e.Y - centerY;

                        var angleRadians = Math.Atan2(dy, dx);

                        var vysl_uhol = angleRadians - selected1.angle;

                        Queue<Segment> q = new Queue<Segment>();
                        q.Enqueue(selected1);

                        while (q.Count > 0)
                        {
                            Segment seg = q.Dequeue();

                            /*seg.angle -= da;
                            seg.calculateBAndChangeChildrenA();*/

                            float x1 = seg.b.x - centerX;
                            float y1 = seg.b.y - centerY;

                            // using rotation matrix
                            float x2 = Convert.ToSingle(x1 * Math.Cos(vysl_uhol) - y1 * Math.Sin(vysl_uhol));
                            float y2 = Convert.ToSingle(x1 * Math.Sin(vysl_uhol) + y1 * Math.Cos(vysl_uhol));

                            seg.b.x = x2 + centerX;
                            seg.b.y = y2 + centerY;


                            var seg_dx = seg.b.x - seg.a.x;
                            var seg_dy = seg.b.y - seg.a.y;

                            var new_angle = Math.Atan2(seg_dy, seg_dx);
                            seg.angle = Convert.ToSingle(new_angle);

                            //seg.angle = CalculateAngle(seg.a.x, seg.a.y, seg.b.x, seg.b.y);
                            //seg.calculateB();

                            //break;

                            foreach (Segment s in seg.children)
                            {
                                s.a.x = seg.b.x;
                                s.a.y = seg.b.y;

                                q.Enqueue(s);
                            }
                        }                       
                        Invalidate();
                    }
                }
            }
            if (inverse)
            {
                if(clickThree)
                {

                    /*// follow mouse 
                    current = selected2;
                    current.follow(e.X, e.Y);
                    while (true)
                    {
                        //print(current);
                        
                        if ((current.a.x == selected1.a.x && current.a.y == selected1.a.y) &&
                            (current.b.x == selected1.b.x && current.b.y == selected1.b.y))
                        {
                            break;
                        }

                        current.parent.follow(current.a.x, current.a.y);
                        current = current.parent;
                        
                    }*/



                    
                    Invalidate();
                }

            }

            /*if(root != null)
            {
                print("ASFASF");
                root.follow(e.X, e.Y);
                //root.show(g, Color.Black);
                Invalidate();
            }*/
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

        // BUTTON EVENTS
        private void skeleton_creation_Click(object sender, EventArgs e)
        {
            // change variables by the selected button - skelet creation
            creation = true;
            forward = false;
            inverse = false;
            status_bar.Text = "Create skeleton by clicking. Select parent and click elswhere to end the new bone.";
        }

        private void forward_kinematics_Click(object sender, EventArgs e)
        {
            // change variables by the selected button - FK
            creation = false;
            forward = true;
            inverse = false;
            status_bar.Text = "Click on bone and use forward kinematics by moving the mouse cursor.";
        }

        private void inverse_kinematics_Click(object sender, EventArgs e)
        {
            // change variables by the selected button - IK
            creation = false;
            forward = false;
            inverse = true;
            status_bar.Text = "Select beginning and end of inverse kinematics sequence. Then click elsewhere to use IK.";
        }

        private void clear_Click(object sender, EventArgs e)
        {
            creation = true;
            forward = false;
            inverse = false;

            // cleares the skelet
            root = null;
            last = null;
            reset_selected();
            Invalidate();
        }


        // LOAD AND SAVE SKELETON BUTTONS
        private void load_skeleton_Click(object sender, EventArgs e)
        {
            // loads skelet from the file if exists
            string fname = "skeleton.dat";
            if (File.Exists(fname))
            {
                FileStream s = new FileStream(fname, FileMode.Open);
                BinaryFormatter f = new BinaryFormatter();
                root = f.Deserialize(s) as Segment;
                s.Close();

                creation = true;
                forward = false;
                inverse = false;

                current = root;
                draw(current, current.children);
                status_bar.Text = "Loaded succesfully!";
            }
            else
            {
                status_bar.Text = "Skeleton.dat file is missing, please save skeleton first.";
            }            
        }

        private void save_skeleton_Click_1(object sender, EventArgs e)
        {
            // saves current skelet if existing
            if (root != null)
            {
                FileStream s = new FileStream("skeleton.dat", FileMode.Create);
                BinaryFormatter f = new BinaryFormatter();
                f.Serialize(s, root);
                s.Close();
                status_bar.Text = "Saved!";
            }
            else
            {
                status_bar.Text = "Skeleton obejct missing, please create skeleton object first.";
            }
        }
    }
}
