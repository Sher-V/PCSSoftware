using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace PCSSoftware
{
    public partial class fMain : Form
    {
        public Site[] NET = null;

        public Process[] PNET = null;

        public Phase[] PhNET = null;

        public Dictionary<int, int> LiNET = null;

        public fMain()
        {
            InitializeComponent();
            rbPhase.Checked = true;
        }

        public class Process
        {
            public List<int> Inc;
            public List<int> NInc;
            public bool Send;
            public int PNum;
            public Dictionary<int, bool> Neighb;
            public Process(bool aS, List<int> aN, int aNum)
            {
                PNum = aNum;
                Send = aS;
                Inc = new List<int>();
                NInc = new List<int>();
                Inc.Add(PNum);
                Neighb = new Dictionary<int, bool>();
                if (aN.Count == 0)
                {
                    NInc.Add(PNum);
                    Send = true;
                }
                foreach (int n in aN)
                    Neighb.Add(n, false);
            }
        }

        public class Site
        {
            public int ancestor;
            public List<int> neighbours;
            public int echo;
            public bool returnEcho;
            public bool start;
            public Site(int aAnc, List<int> aNb, int aEcho)
            {
                ancestor = aAnc;
                neighbours = aNb;
                echo = aEcho;
                returnEcho = false;
                start = false;
            }
        }

        public class Phase
        {
            public int Counter_out;
            public Dictionary<int, int> Counter_in;
            public List<int> OutP;
            public bool Send(int D)
            {
                int min = 100 * D;
                foreach (KeyValuePair<int, int> nbIn in Counter_in)
                {
                    if (nbIn.Value < min)
                        min = nbIn.Value;
                }
                if ((min >= Counter_out) && (Counter_out < D))
                {
                    Counter_out += 1;
                    return true;
                }
                return false;
            }
            public bool CheckStopCond(int D)
            {
                foreach(KeyValuePair<int, int> nbIn in Counter_in)
                {
                    if (nbIn.Value < D)
                        return false;
                }
                return true;
            }
            public Phase(List<int> aNbIn, List<int> aNbOut)
            {
                Counter_in = new Dictionary<int, int>();
                OutP = aNbOut;
                Counter_out = 0;
                foreach (int nb in aNbIn)
                {
                    Counter_in.Add(nb, 0);
                }
            }
        }

        private int CheckColor(int site)
        {
            if (!NET[site].start && !NET[site].returnEcho)
                return 0;
            if (NET[site].start && !NET[site].returnEcho)
                return 1;
            if (NET[site].start && NET[site].returnEcho)
                return 2;
            return 0;
        }

        private void DrawPicture(List<List<int>> hierarchy, double[,] mtx)
        {
            System.Threading.Thread.Sleep(1000);
            DrawPanel.Refresh();
            Graphics graph = DrawPanel.CreateGraphics();
            Pen pen = new Pen(Color.Black);
            Pen pen1 = new Pen(Color.Red);
            Pen pen2 = new Pen(Color.Green);
            Pen pen3 = new Pen(Color.Black);
            pen3.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            Font font = new Font("Times New Roman", 10, FontStyle.Regular);
            int levels = hierarchy.Count;
            int maxElems = 0;
            int mtSize = (int)Math.Sqrt(mtx.Length);
            Tuple<int, int>[] pos = new Tuple<int, int>[mtSize];
            for(int i = 0; i < hierarchy.Count; ++i)
            {
                if (hierarchy[i].Count > maxElems)
                    maxElems = hierarchy[i].Count;
            }
            int levelHeight = DrawPanel.Height / levels;            
            int size = 50;
            int x = 0;
            int y = 0;
            foreach (List<int> level in hierarchy)
            {                
                int CurrentWidth = DrawPanel.Width / level.Count;
                x = CurrentWidth / 2;
                foreach (int elem in level)
                {
                    int color = CheckColor(elem);
                    if (color == 0)
                        graph.DrawEllipse(pen, x - size / 2, y, size, size); // Note: x, y for upper left corner of square for circle
                    else if(color == 1)
                        graph.DrawEllipse(pen1, x - size / 2, y, size, size); 
                    else if(color == 2)
                        graph.DrawEllipse(pen2, x - size / 2, y, size, size); 
                    graph.DrawString((elem + 1).ToString(), font, pen.Brush, x - size / 2 + 20, 20 + y);
                    pos[elem] = new Tuple<int, int>(x, y + size/2);
                    x += CurrentWidth;
                }
                y += levelHeight;
            }
            // Draw arrow
            //pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            //pen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            for(int i = 0; i < mtSize; ++i)
            {
                for(int j = 0; j < mtSize; ++j)
                {
                    if (mtx[i, j] != 0)
                        graph.DrawLine(pen3, pos[i].Item1, pos[i].Item2, pos[j].Item1, pos[j].Item2);
                }
            }            
        }

        private List<List<int>> GeneratePicture(double[,] matrix)
        {            
            int size = (int)Math.Sqrt(matrix.Length);

            NET = new Site[size];
            List<int> fn = new List<int>();
            for (int k = 0; k < size; ++k)
                if (matrix[0, k] != 0)
                    fn.Add(k);
            Site tm1 = new Site(-1, fn, 0);
            NET[0] = tm1;

            List<int> wrkSites = new List<int>();
            for (int i = 1; i < size; ++i) wrkSites.Add(i);
            List<List<int>> hrch = new List<List<int>>();
            List<int> startSite = new List<int>();
            startSite.Add(0); // 0 site is start vertex in graph
            hrch.Add(startSite);
            while (wrkSites.Count != 0)
            {
                List<int> currentLevel = hrch[hrch.Count - 1];
                List<int> nextLevel = new List<int>();
                for(int i = 0; i < size; ++i)
                {
                    foreach (int j in currentLevel)
                    {
                        if (matrix[j, i] != 0)
                        {
                            if (wrkSites.Contains(i))
                            {
                                nextLevel.Add(i);
                                List<int> neigh = new List<int>();
                                for (int k = 0; k < size; ++k)
                                    if ((matrix[i, k] != 0) && (k != j))
                                        neigh.Add(k);
                                Site tmp = new Site(j, neigh, 0);
                                NET[i] = tmp;
                                wrkSites.Remove(i);
                            }
                        }
                    }                            
                }
                hrch.Add(nextLevel);
            }
            return hrch;
        }

        private List<List<int>> InitFinn(double[,] matrix)
        {            
            int size = (int)Math.Sqrt(matrix.Length);
            PNET = new Process[size];            
            // Mass initialization
            for(int i = 0; i < size; ++i)
            {
                List<int> Nb = new List<int>();
                for (int j = 0; j < size; ++j)
                    if (matrix[j, i] > 0)
                        Nb.Add(j);
                bool s = false;
                if (i == 0)
                    s = true;
                Process tmp = new Process(s, Nb, i);
                PNET[i] = tmp;
            }

            List<int> wrkSites = new List<int>();
            for (int i = 1; i < size; ++i) wrkSites.Add(i);
            List<List<int>> hrch = new List<List<int>>();
            List<int> startSite = new List<int>();
            startSite.Add(0); // 0 site is start vertex in graph
            hrch.Add(startSite);
            while (wrkSites.Count != 0)
            {
                List<int> currentLevel = hrch[hrch.Count - 1];
                List<int> nextLevel = new List<int>();
                for (int i = 0; i < size; ++i)
                {
                    foreach (int j in currentLevel)
                    {
                        if ((matrix[j, i] != 0) || (matrix[i, j] != 0))
                        {
                            if (wrkSites.Contains(i))
                            {
                                nextLevel.Add(i);                                                                
                                wrkSites.Remove(i);
                            }
                        }
                    }
                }
                hrch.Add(nextLevel);
            }
            return hrch;
        }

        private List<List<int>> InitPhase(double[,] matrix)
        {
            int size = (int)Math.Sqrt(matrix.Length);
            PhNET = new Phase[size];
            // Mass initialization
            for (int i = 0; i < size; ++i)
            {
                List<int> NbIn = new List<int>();
                List<int> NbOut = new List<int>();
                for (int j = 0; j < size; ++j)
                {
                    if (matrix[j, i] > 0)
                        NbIn.Add(j);
                    if (matrix[i, j] > 0)
                        NbOut.Add(j);
                }
                Phase tmp = new Phase(NbIn, NbOut);
                PhNET[i] = tmp;
            }

            List<int> wrkSites = new List<int>();
            for (int i = 1; i < size; ++i) wrkSites.Add(i);
            List<List<int>> hrch = new List<List<int>>();
            List<int> startSite = new List<int>();
            startSite.Add(0); // 0 site is start vertex in graph
            hrch.Add(startSite);
            while (wrkSites.Count != 0)
            {
                List<int> currentLevel = hrch[hrch.Count - 1];
                List<int> nextLevel = new List<int>();
                for (int i = 0; i < size; ++i)
                {
                    foreach (int j in currentLevel)
                    {
                        if ((matrix[j, i] != 0) || (matrix[i, j] != 0))
                        {
                            if (wrkSites.Contains(i))
                            {
                                nextLevel.Add(i);
                                wrkSites.Remove(i);
                            }
                        }
                    }
                }
                hrch.Add(nextLevel);
            }
            return hrch;
        }

        private List<List<int>> InitLi(double[,] matrix)
        {
            int size = (int)Math.Sqrt(matrix.Length);
            LiNET = new Dictionary<int, int>();
            LiNET.Add(0, 0);
            // Mass initialization
            for (int i = 1; i < size; ++i)            
                LiNET.Add(i, -1);                            
            List<int> wrkSites = new List<int>();
            for (int i = 1; i < size; ++i) wrkSites.Add(i);
            List<List<int>> hrch = new List<List<int>>();
            List<int> startSite = new List<int>();
            startSite.Add(0); // 0 site is start vertex in graph
            hrch.Add(startSite);
            while (wrkSites.Count != 0)
            {
                List<int> currentLevel = hrch[hrch.Count - 1];
                List<int> nextLevel = new List<int>();
                for (int i = 0; i < size; ++i)
                {
                    foreach (int j in currentLevel)
                    {
                        if ((matrix[j, i] != 0) || (matrix[i, j] != 0))
                        {
                            if (wrkSites.Contains(i))
                            {
                                nextLevel.Add(i);
                                wrkSites.Remove(i);
                            }
                        }
                    }
                }
                hrch.Add(nextLevel);
            }
            return hrch;
        }

        private double[,] ReadInputData(string FileName)
        {
            double[,] x = null;            
            try
            {                
                StreamReader streamReader = new StreamReader(FileName);                
                string firstMassLine = streamReader.ReadLine();
                int colRowCount = firstMassLine.Split(',').Count();
                x = new double[colRowCount, colRowCount];
                for (int j = 0; j < colRowCount; ++j)
                {
                    for (int i = 0; i < colRowCount; ++i)
                    {
                        x[j, i] = double.Parse(firstMassLine.Split(',')[i]);
                    }
                    firstMassLine = streamReader.ReadLine();
                }                                
                streamReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return x;
        }

        private void Signal(double[,] matrix, int site, int anc, List<List<int>> hrch)
        {
            if (NET[site].returnEcho)
            {
                NET[anc].echo += 1;
                return;
            }
            if (!NET[site].start)
            {
                NET[site].start = true;
                DrawPicture(hrch, matrix);
            }
            List<int> curNb = NET[site].neighbours;
            curNb.Remove(anc);
            if (curNb.Count == 0)
            {
                NET[anc].echo += 1;
                NET[site].returnEcho = true;
                DrawPicture(hrch, matrix);
            }
            else
            {
                foreach (int elem in curNb)
                    Signal(matrix, elem, site, hrch);
                if (NET[site].echo == NET[site].neighbours.Count)
                {
                    if (anc != -1)
                        NET[anc].echo += 1;
                    NET[site].returnEcho = true;
                    DrawPicture(hrch, matrix);
                }
            }
        }

        private void SolveEcho(double[,] matrix, List<List<int>> hrch)
        {
            Signal(matrix, 0, -1, hrch);   
        }

        private bool CheckFinn(int size)
        {
            for(int i = 0; i < size; ++i)
            {
                if (PNET[i].Inc.Count == PNET[i].NInc.Count)
                {
                    bool res = true;
                    foreach(int p in PNET[i].Inc)
                    {
                        if (!PNET[i].NInc.Contains(p))
                        {
                            res = false;
                            break;
                        }
                    }
                    if (res)
                    {
                        MessageBox.Show((i+1).ToString() + " process send signal", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return true;
                    }
                }
            }
            return false;
        }

        private void SolveFinn(double[,] matrix, List<List<int>> hrch)
        {
            int size = (int)Math.Sqrt(matrix.Length);
            List<int> queue = new List<int>();
            DrawFinnPicture(hrch, matrix);
            while (!CheckFinn(size))
            {
                for (int i = 0; i < size; ++i)
                {
                    if (PNET[i].Send) queue.Add(i);
                }
                foreach(int sender in queue)
                {
                    for(int j = 0; j < size; ++j)
                    {
                        if (matrix[sender, j] > 0)
                        {
                            PNET[j].Neighb[sender] = true;
                            foreach (int p in PNET[sender].Inc)
                            {
                                if (!PNET[j].Inc.Contains(p))
                                {
                                    PNET[j].Inc.Add(p);
                                    PNET[j].Send = true;
                                }
                            }
                            foreach (int p in PNET[sender].NInc)
                            {
                                if (!PNET[j].NInc.Contains(p))
                                {
                                    PNET[j].NInc.Add(p);
                                    PNET[j].Send = true;
                                }
                            }
                        }
                    }
                }
                queue.Clear();
                for (int i = 0; i < size; ++i)
                {
                    bool res = true;
                    foreach(bool rec in PNET[i].Neighb.Values)
                    {
                        if (!rec)
                        {
                            res = false;
                            break;
                        }
                    }
                    if (res)
                    {
                        if (!PNET[i].NInc.Contains(i))
                        {
                            PNET[i].NInc.Add(i);
                            PNET[i].Send = true;
                        }
                    }
                }
                DrawFinnPicture(hrch, matrix);
            }
        }

        private bool CheckPhase(int size, int d)
        {
            for (int i = 0; i < size; ++i)
                if (PhNET[i].CheckStopCond(d))
                {
                    MessageBox.Show((i + 1).ToString() + " process send signal", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return false;
                }
            return true;
        }

        private void SolvePhase(double[,] matrix, List<List<int>> hrch)
        {
            int size = (int)Math.Sqrt(matrix.Length);            
            List<int> queue = new List<int>();
            int d = 3;
            queue.Add(0); // send from first process
            DrawPhasePicture(hrch, matrix, queue);
            // Note : green circle - recieve token at current moment
            while (CheckPhase(size, d))
            {                
                List<int> tmpQ = new List<int>();
                foreach (int sender in queue)
                    if (PhNET[sender].Send(d))
                    {
                        for (int i = 0; i < size; ++i)
                            if (matrix[sender, i] > 0)
                            {
                                tmpQ.Add(i);
                                PhNET[i].Counter_in[sender] += 1;
                            }
                    }
                queue.Clear();
                queue = tmpQ;
                DrawPhasePicture(hrch, matrix, queue);
            }            
        }

        private void SolveLi(double[,] matrix, List<List<int>> hrch)
        {
            int size = (int)Math.Sqrt(matrix.Length);
            List<Tuple<int, int>> queue = new List<Tuple<int, int>>();            
            queue.Add(new Tuple<int, int>(0, 0)); // send from first process
            DrawLiPicture(hrch, matrix);            
            while (queue.Count > 0)
            {
                List<Tuple<int, int>> tmpQ = new List<Tuple<int, int>>();
                foreach (Tuple<int, int> sender in queue)
                {
                    for (int i = 0; i < size; ++i)
                    {
                        if (matrix[sender.Item1, i] > 0)
                            if ((LiNET[i] > sender.Item2) || (LiNET[i] == -1))
                            {
                                tmpQ.Add(new Tuple<int, int>(i, sender.Item2 + 1));
                                LiNET[i] = sender.Item2 + 1;
                            }
                    }                    
                }
                queue.Clear();
                queue = tmpQ;
                DrawLiPicture(hrch, matrix);
            }
            MessageBox.Show("All processes are achived!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void DrawFinnPicture(List<List<int>> hierarchy, double[,] mtx)
        {
            System.Threading.Thread.Sleep(1000);
            DrawPanel.Refresh();
            Graphics graph = DrawPanel.CreateGraphics();
            Pen pen = new Pen(Color.Black);
            Pen pen1 = new Pen(Color.Red);
            Pen pen2 = new Pen(Color.Green);
            Pen pen3 = new Pen(Color.Black);
            pen3.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            Font font = new Font("Times New Roman", 10, FontStyle.Regular);
            Font font2 = new Font("Times New Roman", 7, FontStyle.Regular);
            int levels = hierarchy.Count;            
            int mtSize = (int)Math.Sqrt(mtx.Length);
            Tuple<int, int>[] pos = new Tuple<int, int>[mtSize];            
            int levelHeight = DrawPanel.Height / levels;
            int size = 50;
            int x = 0;
            int y = 0;
            foreach (List<int> level in hierarchy)
            {
                int CurrentWidth = DrawPanel.Width / level.Count;
                x = CurrentWidth / 2;
                foreach (int elem in level)
                {                    
                    graph.DrawEllipse(pen, x - size / 2, y, size, size); // Note: x, y for upper left corner of square for circle                    
                    graph.DrawString((elem + 1).ToString(), font, pen.Brush, x - size / 2 + 20, 20 + y);
                    string str = "[" + (PNET[elem].Inc[0] + 1).ToString();
                    for (int p = 1; p < PNET[elem].Inc.Count; ++p)
                        str += ", " + (PNET[elem].Inc[p] + 1).ToString();
                    str += " ||| ";
                    if (PNET[elem].NInc.Count == 0)
                    {
                        str += "(/)]";
                    }
                    else
                    {
                        str += (PNET[elem].NInc[0] + 1).ToString();
                        for (int p = 1; p < PNET[elem].NInc.Count; ++p)
                            str += ", " + (PNET[elem].NInc[p] + 1).ToString();
                        str += "]";
                    }
                    graph.DrawString(str, font2, pen.Brush, x - (int)(size * 1.4), size + y + 10);
                    pos[elem] = new Tuple<int, int>(x, y + size / 2);
                    x += CurrentWidth;
                }
                y += levelHeight;
            }
            // Draw arrow
            AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);            
            pen3.CustomEndCap = bigArrow;                        
            for (int i = 0; i < mtSize; ++i)
            {
                for (int j = 0; j < mtSize; ++j)
                {
                    if (mtx[i, j] != 0)
                    {
                        int dx = Math.Abs(pos[i].Item1 - pos[j].Item1);
                        int dy = Math.Abs(pos[i].Item2 - pos[j].Item2);
                        double a = Math.Atan((float)dy / (float)dx);
                        double cos = Math.Cos(a);
                        double sin = Math.Sin(a);
                        if ((pos[i].Item1 > pos[j].Item1) && (pos[i].Item2 <= pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 - (int)(size * cos / 2), pos[i].Item2 + (int)(size * sin / 2), pos[j].Item1 + (int)(size * cos / 2), pos[j].Item2 - (int)(size * sin / 2));
                        if ((pos[i].Item1 < pos[j].Item1) && (pos[i].Item2 <= pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 + (int)(size * cos / 2), pos[i].Item2 + (int)(size * sin / 2), pos[j].Item1 - (int)(size * cos / 2), pos[j].Item2 - (int)(size * sin / 2));
                        if ((pos[i].Item1 > pos[j].Item1) && (pos[i].Item2 > pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 - (int)(size * cos / 2), pos[i].Item2 - (int)(size * sin / 2), pos[j].Item1 + (int)(size * cos / 2), pos[j].Item2 + (int)(size * sin / 2));
                        if ((pos[i].Item1 < pos[j].Item1) && (pos[i].Item2 > pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 + (int)(size * cos / 2), pos[i].Item2 - (int)(size * sin / 2), pos[j].Item1 - (int)(size * cos / 2), pos[j].Item2 + (int)(size * sin / 2));
                    }
                }
            }
        }

        private void DrawPhasePicture(List<List<int>> hierarchy, double[,] mtx, List<int> Tok)
        {
            System.Threading.Thread.Sleep(1000);
            DrawPanel.Refresh();
            Graphics graph = DrawPanel.CreateGraphics();
            Pen pen = new Pen(Color.Black);
            Pen pen2 = new Pen(Color.Green);
            Pen pen3 = new Pen(Color.Black);
            pen3.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            Font font = new Font("Times New Roman", 10, FontStyle.Regular);
            Font font2 = new Font("Times New Roman", 8, FontStyle.Regular);
            int levels = hierarchy.Count;
            int mtSize = (int)Math.Sqrt(mtx.Length);
            Tuple<int, int>[] pos = new Tuple<int, int>[mtSize];
            int levelHeight = DrawPanel.Height / levels;
            int size = 50;
            int x = 0;
            int y = 0;
            foreach (List<int> level in hierarchy)
            {
                int CurrentWidth = DrawPanel.Width / level.Count;
                x = CurrentWidth / 2;
                foreach (int elem in level)
                {
                    if (Tok.Contains(elem))
                        graph.DrawEllipse(pen2, x - size / 2, y, size, size); // Note: x, y for upper left corner of square for circle                    
                    else
                        graph.DrawEllipse(pen, x - size / 2, y, size, size); // Note: x, y for upper left corner of square for circle                    
                    graph.DrawString((elem + 1).ToString(), font, pen.Brush, x - size / 2 + 20, 20 + y);
                    string str = "[";
                    foreach(KeyValuePair<int, int> kvp in PhNET[elem].Counter_in)
                    {
                        str += "(" + (kvp.Key + 1).ToString() + "|" + kvp.Value.ToString() + ")";
                    }
                    str += "] Cout = " + PhNET[elem].Counter_out.ToString();
                    graph.DrawString(str, font2, pen.Brush, x - (int)(size * 1.4), size + y + 10);
                    pos[elem] = new Tuple<int, int>(x, y + size / 2);
                    x += CurrentWidth;
                }
                y += levelHeight;
            }
            // Draw arrow
            AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);
            pen3.CustomEndCap = bigArrow;
            for (int i = 0; i < mtSize; ++i)
            {
                for (int j = 0; j < mtSize; ++j)
                {
                    if (mtx[i, j] != 0)
                    {
                        int dx = Math.Abs(pos[i].Item1 - pos[j].Item1);
                        int dy = Math.Abs(pos[i].Item2 - pos[j].Item2);
                        double a = Math.Atan((float)dy / (float)dx);
                        double cos = Math.Cos(a);
                        double sin = Math.Sin(a);
                        if ((pos[i].Item1 > pos[j].Item1) && (pos[i].Item2 <= pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 - (int)(size * cos / 2), pos[i].Item2 + (int)(size * sin / 2), pos[j].Item1 + (int)(size * cos / 2), pos[j].Item2 - (int)(size * sin / 2));
                        if ((pos[i].Item1 < pos[j].Item1) && (pos[i].Item2 <= pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 + (int)(size * cos / 2), pos[i].Item2 + (int)(size * sin / 2), pos[j].Item1 - (int)(size * cos / 2), pos[j].Item2 - (int)(size * sin / 2));
                        if ((pos[i].Item1 > pos[j].Item1) && (pos[i].Item2 > pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 - (int)(size * cos / 2), pos[i].Item2 - (int)(size * sin / 2), pos[j].Item1 + (int)(size * cos / 2), pos[j].Item2 + (int)(size * sin / 2));
                        if ((pos[i].Item1 < pos[j].Item1) && (pos[i].Item2 > pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 + (int)(size * cos / 2), pos[i].Item2 - (int)(size * sin / 2), pos[j].Item1 - (int)(size * cos / 2), pos[j].Item2 + (int)(size * sin / 2));
                    }
                }
            }
        }

        private void DrawLiPicture(List<List<int>> hierarchy, double[,] mtx)
        {
            System.Threading.Thread.Sleep(1000);
            DrawPanel.Refresh();
            Graphics graph = DrawPanel.CreateGraphics();
            Pen pen = new Pen(Color.Black);
            Pen pen3 = new Pen(Color.Black);
            pen3.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            Font font = new Font("Times New Roman", 10, FontStyle.Regular);
            Font font2 = new Font("Times New Roman", 8, FontStyle.Regular);
            int levels = hierarchy.Count;
            int mtSize = (int)Math.Sqrt(mtx.Length);
            Tuple<int, int>[] pos = new Tuple<int, int>[mtSize];
            int levelHeight = DrawPanel.Height / levels;
            int size = 50;
            int x = 0;
            int y = 0;
            foreach (List<int> level in hierarchy)
            {
                int CurrentWidth = DrawPanel.Width / level.Count;
                x = CurrentWidth / 2;
                foreach (int elem in level)
                {
                    graph.DrawEllipse(pen, x - size / 2, y, size, size); // Note: x, y for upper left corner of square for circle                    
                    graph.DrawString((elem + 1).ToString(), font, pen.Brush, x - size / 2 + 20, 20 + y);
                    if (LiNET[elem] >= 0)
                    {
                        string str = LiNET[elem].ToString();
                        graph.DrawString(str, font2, pen.Brush, x, size + y + 10);
                    }
                    pos[elem] = new Tuple<int, int>(x, y + size / 2);
                    x += CurrentWidth;
                }
                y += levelHeight;
            }
            // Draw arrow            
            for (int i = 0; i < mtSize; ++i)
            {
                for (int j = i; j < mtSize; ++j)
                {
                    if (mtx[i, j] != 0)
                    {
                        int dx = Math.Abs(pos[i].Item1 - pos[j].Item1);
                        int dy = Math.Abs(pos[i].Item2 - pos[j].Item2);
                        double a = Math.Atan((float)dy / (float)dx);
                        double cos = Math.Cos(a);
                        double sin = Math.Sin(a);
                        if ((pos[i].Item1 > pos[j].Item1) && (pos[i].Item2 <= pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 - (int)(size * cos / 2), pos[i].Item2 + (int)(size * sin / 2), pos[j].Item1 + (int)(size * cos / 2), pos[j].Item2 - (int)(size * sin / 2));
                        if ((pos[i].Item1 < pos[j].Item1) && (pos[i].Item2 <= pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 + (int)(size * cos / 2), pos[i].Item2 + (int)(size * sin / 2), pos[j].Item1 - (int)(size * cos / 2), pos[j].Item2 - (int)(size * sin / 2));
                        if ((pos[i].Item1 > pos[j].Item1) && (pos[i].Item2 > pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 - (int)(size * cos / 2), pos[i].Item2 - (int)(size * sin / 2), pos[j].Item1 + (int)(size * cos / 2), pos[j].Item2 + (int)(size * sin / 2));
                        if ((pos[i].Item1 < pos[j].Item1) && (pos[i].Item2 > pos[j].Item2))
                            graph.DrawLine(pen3, pos[i].Item1 + (int)(size * cos / 2), pos[i].Item2 - (int)(size * sin / 2), pos[j].Item1 - (int)(size * cos / 2), pos[j].Item2 + (int)(size * sin / 2));
                    }
                }
            }
        }

        private void bPrint_Click(object sender, EventArgs e)
        {
            double[,] x = null;
            if ((rbFinn.Checked) || (rbPhase.Checked))
                x = ReadInputData("D:\\tig\\magistr\\3sem\\groshev\\Общесистемное_ПО_ПВС\\Graph_1_Finn.csv");
            else if ((rbEcho.Checked) || (rbLi.Checked))
                x = ReadInputData("D:\\tig\\magistr\\3sem\\groshev\\Общесистемное_ПО_ПВС\\Graph_1_Echo.csv");
            if (x != null)
            {
                if (rbLi.Checked)
                {
                    List<List<int>> hierarchy = InitLi(x);
                    SolveLi(x, hierarchy);
                }
                else if (rbPhase.Checked)
                {
                    List<List<int>> hierarchy = InitPhase(x);
                    SolvePhase(x, hierarchy);
                }
                else if (rbFinn.Checked)
                {
                    List<List<int>> hierarchy = InitFinn(x);                    
                    SolveFinn(x, hierarchy);
                }
                else if (rbEcho.Checked)
                {
                    List<List<int>> hierarchy = GeneratePicture(x);
                    SolveEcho(x, hierarchy);
                    DrawPicture(hierarchy, x);
                    MessageBox.Show("Echo!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void rbEcho_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEcho.Checked)
            {
                rbFinn.Checked = false;
                rbPhase.Checked = false;
                rbLi.Checked = false;
            }
        }

        private void rbFinn_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFinn.Checked)
            {
                rbEcho.Checked = false;
                rbPhase.Checked = false;
                rbLi.Checked = false;
            }
        }

        private void rbPhase_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPhase.Checked)
            {
                rbEcho.Checked = false;
                rbFinn.Checked = false;
                rbLi.Checked = false;
            }
        }

        private void rbLi_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLi.Checked)
            {
                rbEcho.Checked = false;
                rbFinn.Checked = false;
                rbPhase.Checked = false;
            }
        }
    }    
}
