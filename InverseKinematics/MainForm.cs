﻿using System;
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
        bool clickOne, clickTwo, creation, forward, inverse;
        int click;
        Vector2D start, end;
        Graphics g;
        Random rnd;
        int n = 0;

        public MainForm()
        {
            //DoubleBuffered = true;
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
            start = new Vector2D();
            end = new Vector2D();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {        
            if (forward)
            {
                if(selected1 != null)
                {
                    print($"DRAWING root {++n}");
                    draw(root, root.children);
                }
            }
        }

        public void draw(Segment node, ArrayList children)
        {
            // recurse
            foreach (Segment child in children)
            {
                g.DrawLine(new Pen(node.color, 5), node.a.x, node.a.y, child.a.x, child.a.y);
                draw(child, child.children);
            }
            g.FillEllipse(Brushes.Red, node.a.x - 5, node.a.y - 5, 10, 10);            
        }


        static void print(object o)
        {
            Console.WriteLine(Convert.ToString(o));
        }

        /*static float Radians(float angle)
        {
            return (float)(Math.PI / 180) * angle;
        }*/

        float CalculateAngle(float x1, float y1, float x2, float y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;

            var angleRadians = Math.Atan2(dy, dx);

            //print($"UHOL {angleRadians}");

            return (float)angleRadians;
        }

        private float CalculateLength(float aX, float aY, float bX, float bY)
        {
            return Convert.ToSingle(Math.Sqrt((Math.Pow(bX - aX, 2) + Math.Pow(bY - aY, 2))));
        }

        private void find_selected(ref Segment seg, float ex, float ey)
        {
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

            /*foreach (Segment child in seg.children)
            {
                find_selected(ref child, ex, ey);
            }*/
        }

        private Segment create_next(Vector2D start, Vector2D end)
        {
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
                            find_selected(ref current, e.X, e.Y);
                            if (selected1 != null)
                            {
                                selected1.show(g, Color.Blue);
                                clickOne = true;
                            }
                        }
                        // add new bone to the selevted1
                        else
                        {
                            start.set(selected1.b.x, selected1.b.y);
                            end.set(e.X, e.Y);

                            Segment child = create_next(start, end);
                            child.show(g, Color.Orange);

                            selected1.children.Add(child);
                            selected1.show(g, Color.Black);
                            selected1 = null;

                            if(last != null)
                            {
                                last.show(g, Color.Black);
                            }
                            last = child;
                            
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

                            reset_selected();
                        }                        
                    }
                }
            }
            draw(root, root.children);
        }

        public void reset_selected()
        {
            selected1.show(g, Color.Black);
            selected2.show(g, Color.Black);
            selected1 = null;
            selected2 = null;
            clickOne = false;
            clickTwo = false;
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

                        //print(selected1 + " " + Convert.ToString(e.X) + " " + Convert.ToString(e.Y));

                        
                        float a = CalculateAngle(selected1.a.x, selected1.a.y, e.X, e.Y);
                        print($"angle {a} ");
                        print($"selected angle {selected1.angle} ");

                        float da = selected1.angle - a;
                        print($"da {da} ");


                        selected1.angle += 0.01f; //da % Radians(360);
                        selected1.calculateB();
                        selected1.show(g, Color.Blue);
                        Invalidate();
                        //draw(selected1, selected1.children);


                    }
                }
            }
        }

        public void change_segment()
        {

        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (forward)
            {
                if (clickOne)
                {
                    if (selected1 != null)
                    {
                        selected1 = null;
                    }
                    clickOne = false;
                }
            }
        }



        // TIMER
        private void timer1_Tick(object sender, EventArgs e)
        {
        }



        // BUTTON EVENTS
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

        private void clear_Click(object sender, EventArgs e)
        {
            root = null;
            selected1 = null;
            selected2 = null;
            Invalidate();
        }


        // LOAD AND SAVE SKELETON BUTTONS
        private void load_skeleton_Click(object sender, EventArgs e)
        {
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

                draw(root, root.children);
                status_bar.Text = "Loaded succesfully!";
            }
            else
            {
                status_bar.Text = "Skeleton.dat file is missing, please save skeleton first.";
            }            
        }

        private void save_skeleton_Click_1(object sender, EventArgs e)
        {
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
