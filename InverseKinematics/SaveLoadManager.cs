using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace InverseKinematics
{
    /// <summary>
    /// Manages save and load functions that saves data into file
    /// </summary>
    class SaveLoadManager
    
    {
        MainForm mainform;
        string path;

        public SaveLoadManager(MainForm mainform)
        {
            this.mainform = mainform;
            path = "skeleton.dat";
        }

        public void save(Segment root)
        {
            // saves current skelet if existing
            FileStream s = new FileStream(path, FileMode.Create);
            BinaryFormatter f = new BinaryFormatter();
            f.Serialize(s, root);
            s.Close();
        }

        public Segment load()
        {
            // loads root if file in path existing
            if (File.Exists(path))
            {
                FileStream s = new FileStream(path, FileMode.Open);
                BinaryFormatter f = new BinaryFormatter();
                Segment root = f.Deserialize(s) as Segment;
                s.Close();
                return root;
            }
            else
            {
                return null;
            }
        }
    }
}
