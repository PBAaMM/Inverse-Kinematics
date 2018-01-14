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


namespace InverseKinematics
{
    public partial class MainForm : Form
    {
        Segment root, selected1, selected2, last, current;
        bool clickOne, clickTwo, creation, forward, inverse;
        Vector2D start, end;
        Graphics g;
        Random rnd;
        List<Segment> chain;
        float chain_length;
        List<Segment> origin_bones;

        SaveLoadManager sl_manager;

        public MainForm()
        {
            InitializeComponent();

            save_skeleton.Hide();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            g = CreateGraphics();
            rnd = new Random();

            sl_manager = new SaveLoadManager(this);
                        
            chain_length = 0;
            clickOne = false;
            clickTwo = false;
            creation = false;
            forward = false;
            inverse = false;
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
                    if (start == null)
                    {
                        start = new Vector2D(e.X, e.Y);
                        root = new Segment(start.x, start.y, 0, 0);
                    }
                    else if (end == null)
                    {
                        end = new Vector2D(e.X, e.Y);
                        root = create_next(start, end);
                        last = root;
                    }                    
                    else if(root != null)
                    {
                        // find first selected bone
                        if (!clickOne)
                        {
                            current = root;
                            selected1 = find_clicked(ref current, e.X, e.Y);

                            if (selected1 != null)
                            {
                                selected1.color = Color.Blue;
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
                            selected1.color = Color.Black;
                            //child.color = Color.Orange;
                        
                            if (last != null)
                            {
                                last.color = Color.Black;
                            }
                            last = child;
                            last.color = Color.Orange;

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
                                child.color = Color.Orange;
                                child.parent = last;

                                last.children.Add(child);
                                last.color = Color.Black;
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
                                selected1.color = Color.Blue;
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
                                if (selected1.children.Count == 0)
                                {
                                    reset_selected();
                                    status_bar.Text = "Bad IK sequence";
                                }
                                else
                                {
                                    selected1.color = Color.Blue;
                                    clickOne = true;
                                    status_bar.Text = "Select beginning and end of inverse kinematics sequence. Then click elsewhere to use IK.";
                                }
                            }
                        }
                        // find second selected bone
                        else if (!clickTwo)
                        {
                            current = root;
                            find_selected(ref current, e.X, e.Y);
                            if (selected2 != null)
                            {
                                selected2.color = Color.Blue;

                                // create IK chain    
                                chain = new List<Segment>();
                                chain_length = 0;

                                bool bad_IK = false;

                                current = selected2;
                                while (true)
                                {
                                    if(current == null)
                                    {
                                        
                                        bad_IK = true;
                                        break;
                                    }
                                    chain_length += current.len;
                                    chain.Add(current);

                                    if ((current.a.x == selected1.a.x && current.a.y == selected1.a.y) &&
                                        (current.b.x == selected1.b.x && current.b.y == selected1.b.y))
                                    {
                                        break;
                                    }

                                    current = current.parent;
                                }
                                if (bad_IK)
                                {
                                    status_bar.Text = "Bad IK sequence";
                                    reset_selected();
                                }
                                else
                                {
                                    chain.Reverse();
                                    clickTwo = true;
                                    status_bar.Text = "Select beginning and end of inverse kinematics sequence. Then click elsewhere to use IK.";
                                }
                            }
                        }
                        else
                        {
                            // apply IK
                            print("Folow e.X and e.Y");
                            g.FillEllipse(Brushes.Green, e.X - 5, e.Y - 5, 10, 10);

                            if (selected2.children.Count == 0)
                            {
                                status_bar.Text = "Select beginning and end of inverse kinematics sequence. Then click elsewhere to use IK.";
                                apply_IK(e);
                            }
                            else
                            {
                                reset_selected();
                                status_bar.Text = "Bad IK sequence";
                            }
                        }
                    }
                }
                Invalidate();
            }
        }

        public void apply_IK(MouseEventArgs e)
        {

            // apply inverse kinematics - IK

            float distance_target = Convert.ToSingle(Math.Sqrt((Math.Pow(e.X - chain[0].a.x, 2) + Math.Pow(e.Y - chain[0].a.y, 2))));
            origin_bones = new List<Segment>(chain);

            if (chain_length <= distance_target)
            {
                foreach (Segment seg in chain)
                {
                    float centerX = seg.a.x;
                    float centerY = seg.a.y;

                    var dx = e.X - centerX;
                    var dy = e.Y - centerY;

                    var new_angle = Math.Atan2(dy, dx);
                    var vysl_uhol = new_angle - seg.angle;

                    Queue<Segment> q = new Queue<Segment>();
                    q.Enqueue(seg);

                    while (q.Count > 0)
                    {
                        Segment s = q.Dequeue();

                        float x1 = s.b.x - centerX;
                        float y1 = s.b.y - centerY;

                        // using rotation matrix
                        float x2 = Convert.ToSingle(x1 * Math.Cos(vysl_uhol) - y1 * Math.Sin(vysl_uhol));
                        float y2 = Convert.ToSingle(x1 * Math.Sin(vysl_uhol) + y1 * Math.Cos(vysl_uhol));

                        s.b.x = x2 + centerX;
                        s.b.y = y2 + centerY;

                        var seg_dx = s.b.x - s.a.x;
                        var seg_dy = s.b.y - s.a.y;

                        var s_new_angle = Math.Atan2(seg_dy, seg_dx);
                        s.angle = Convert.ToSingle(s_new_angle);

                        foreach (Segment child in s.children)
                        {
                            child.a.x = s.b.x;
                            child.a.y = s.b.y;

                            q.Enqueue(child);
                        }
                    }
                }
            }
            else
            {
                float centerX = chain[0].a.x;
                float centerY = chain[0].a.y;

                float distance = Convert.ToSingle(Math.Sqrt((Math.Pow(e.X - chain[chain.Count - 1].a.x, 2) + Math.Pow(e.Y - chain[chain.Count - 1].a.y, 2))));
                int error = 1;
                int iterations = 20;

                while (distance > error && iterations != 0)
                {
                    chain[chain.Count - 1].b.x = e.X;
                    chain[chain.Count - 1].b.y = e.Y;

                    for (int i = chain.Count; (i--) > 0;)
                    {
                        Segment s1 = chain[i];
                        float dis = Convert.ToSingle(Math.Sqrt((Math.Pow(s1.b.x - s1.a.x, 2) + Math.Pow(s1.b.y - s1.a.y, 2))));

                        float d = dis - s1.len;

                        s1.a.x = s1.a.x + ((s1.b.x - s1.a.x) / dis * d);
                        s1.a.y = s1.a.y + ((s1.b.y - s1.a.y) / dis * d);

                        if (s1.parent != null)
                        {
                            s1.parent.b.x = s1.a.x;
                            s1.parent.b.y = s1.a.y;
                        }
                    }
                    chain[0].a.x = centerX;
                    chain[0].a.y = centerY;

                    for (int i = 0; i < chain.Count; i++)
                    {
                        Segment s1 = chain[i];
                        float dis = Convert.ToSingle(Math.Sqrt((Math.Pow(s1.b.x - s1.a.x, 2) + Math.Pow(s1.b.y - s1.a.y, 2))));
                        float d = dis - s1.len;

                        s1.b.x = s1.b.x - ((s1.b.x - s1.a.x) / dis * d);
                        s1.b.y = s1.b.y - ((s1.b.y - s1.a.y) / dis * d);

                        if (i + 1 < chain.Count)
                        {
                            chain[i + 1].a.x = s1.b.x;
                            chain[i + 1].a.y = s1.b.y;
                        }
                    }
                    distance = Convert.ToSingle(Math.Sqrt((Math.Pow(e.X - chain[chain.Count - 1].b.x, 2) + Math.Pow(e.Y - chain[chain.Count - 1].b.y, 2))));
                    iterations -= 1;
                }

                // uprava kosti co neboli v retazi ale maju parenta v retazi
                for (int i = 1; i < chain.Count; i++)
                {
                    Segment seg = chain[i - 1];

                    foreach (Segment child in seg.children)
                    {
                        if ((child != chain[i]))
                        {
                            float shift_x = chain[i - 1].b.x - origin_bones[i - 1].b.x;
                            float shift_y = chain[i - 1].b.y - origin_bones[i - 1].b.y;

                            Queue<Segment> queue = new Queue<Segment>();
                            queue.Enqueue(child);
                            while (queue.Count > 0)
                            {
                                Segment child_seg = queue.Dequeue();

                                child_seg.b.x += shift_x;
                                child_seg.b.y += shift_y;

                                if (child_seg.parent != null)
                                {
                                    child_seg.a.x = child_seg.parent.b.x;
                                    child_seg.a.y = child_seg.parent.b.y;
                                }

                                foreach (Segment ch_seg in child_seg.children)
                                {
                                    queue.Enqueue(ch_seg);
                                }
                            }
                        }
                    }
                }

                // prepocitanie ulhol vsetkym kostiam
                current = root;
                Queue<Segment> q = new Queue<Segment>();
                q.Enqueue(current);

                while (q.Count > 0)
                {
                    Segment s = q.Dequeue();

                    var seg_dx = s.b.x - s.a.x;
                    var seg_dy = s.b.y - s.a.y;

                    var s_new_angle = Math.Atan2(seg_dy, seg_dx);
                    s.angle = Convert.ToSingle(s_new_angle);

                    foreach (Segment child in s.children)
                    {
                        q.Enqueue(child);
                    }
                }
            }           
        }

        public void reset_selected()
        {
            // resets used variables
            if(selected1 != null)
            {
                selected1.color = Color.Black;
                selected1 = null;
            }
                
            if(selected2 != null)
            {
                selected2.color = Color.Black;
                selected2 = null;
            }
 
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
                        // apply forward kinematics - FK

                        float a = CalculateAngle(selected1.a.x, selected1.a.y, e.X, e.Y);
                        float da = selected1.angle - a;

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
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (forward)
            {
                if (clickOne)
                {
                    if (selected1 != null)
                    {
                        selected1.color = Color.Black;
                        selected1 = null;
                    }
                    clickOne = false;
                }
            }
            Invalidate();
        }

        // BUTTON EVENTS
        private void skeleton_creation_Click(object sender, EventArgs e)
        {
            // change variables by the selected button - skelet creation
            reset_selected();
            Invalidate();

            creation = true;
            forward = false;
            inverse = false;
            status_bar.Text = "Create skeleton by clicking. Select parent and click elswhere to end the new bone.";
        }

        private void forward_kinematics_Click(object sender, EventArgs e)
        {
            // change variables by the selected button - FK
            reset_selected();
            Invalidate();

            creation = false;
            forward = true;
            inverse = false;
            status_bar.Text = "Click on bone and use forward kinematics by moving the mouse cursor.";
        }

        private void inverse_kinematics_Click(object sender, EventArgs e)
        {
            // change variables by the selected button - IK
            reset_selected();
            Invalidate();

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

            // cleares the skeleton
            root = null;
            last = null;
            start = null;
            end = null;

            reset_selected();
            Invalidate();
        }


        // LOAD AND SAVE SKELETON BUTTONS
        private void load_skeleton_Click(object sender, EventArgs e)
        {
            // loads skelet from the file if exists

            root = sl_manager.load();
            if (root != null)
            {
                creation = true;
                forward = false;
                inverse = false;

                start = new Vector2D();
                end = new Vector2D();

                current = root;
                draw(current, current.children);
                status_bar.Text = "Loaded succesfully!";

            }
            else
            {
                status_bar.Text = "You haven't saved skeleton yet. Please save skeleton first.";
            }
        }

        private void save_skeleton_Click_1(object sender, EventArgs e)
        {
            if (root != null)
            {
               sl_manager.save(root);
               status_bar.Text = "Saved!";
            }
            else
            {
                status_bar.Text = "Skeleton obejct missing, please create skeleton object first.";
            }
        }

        private void help_Click(object sender, EventArgs e)
        {
            string popis = @"Pre načítanie pripraveneho skeletona kliknite na tlacidlo 'Load Skeleton'

Vytváranie kostry - Kliknite na tlačidlo 'Skeleton Creation'
    1. Ak neexistuje skeleton, pravím kliknutím do plochy vytvorite koreň a dalším klikom vytvoríte prvú kosť.
    2. Ak existuje kosť, kliknutím do plochy sa vytvori nová kosť pre posledne pridanú kosť.
    3. Kliknutím na kosť (označí sa na modro) možete vytvoríť pod-kosť oznacenej kosti.

Forward Kinematics - Kliknite na tlačidlo 'Forward Kinematics'
    1. Kliknite a tahajte kosť, na ktorú chcete aplikovať FK.

Inverse Kinematics - Kliknite na tlačidlo 'Inverse Kinematics'
    1. Kliknite na prvú (začiatočnú) kosť IK (mala by mať pod-kosť).
    2. KLiknite na poslednú (koncovú) kosť IK (nemala by mať pod-kosť).
    3. Kliknite mimo kosti a postuponosť kostí sa upravi podľa IK.

Tlačidlom 'Clear' vymažete plochu";

            Prompt.ShowDialog(popis, "Návod na použitie");
        }

        public static class Prompt
        {
            public static void ShowDialog(string text, string caption)
            {
                Form prompt = new Form();
                prompt.Width = 700;
                prompt.Height = 400;
                prompt.Text = caption;

                Label textLabel = new Label() { Left = 35, Top = 20, Text = text , Font = new Font("Arial", 12) };
                textLabel.MaximumSize = new Size(650, 0);
                textLabel.AutoSize = true;

                prompt.Controls.Add(textLabel);
                prompt.ShowDialog();
            }
        }
    }
}
