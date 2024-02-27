using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ABCapproche2
{
    class Bee
    {
        static int NoSegments=18;
        public int NoS = NoSegments;
        private int TrackNo;
        public int index = 0;
        public float maxValue=0;
        public PointF[] BeeSource = new PointF[NoSegments];
        public PointF[] BeeSourceC1 = new PointF[NoSegments];
        public PointF[] BeeSourceC2 = new PointF[NoSegments];
        public float[] BeeSourceW = new float[NoSegments];
        public Bee()
        {
            
        }
        public int Track
        {
            get { return TrackNo; }
            set { TrackNo = value; }
        }
        public PointF getC1(int index)
        {
            this.index = index;
            PointF result= new PointF();
            result.X = BeeSourceC1[index].X;
            result.Y = BeeSourceC1[index].Y;
            return result;
        }

        public PointF getC2(int index)
        {
            this.index = index;
            PointF result = new PointF();
            result.X = BeeSourceC2[index].X;
            result.Y = BeeSourceC2[index].Y;
            return result;
        }

        public float getW(int index)
        {
            this.index = index;
            float result = 0f;
            result = BeeSourceW[index];
            
            return result;
        }

        public PointF getBee(int index)
        {
            this.index = index;
            PointF result = new PointF();
            result.X = BeeSource[index].X;
            result.Y = BeeSource[index].Y;
            return result;
        }

      
        public void feedBee(int index, PointF P0, PointF C1, PointF C2, float W)
        {
            this.index = index;
            BeeSource[index] = P0;
            BeeSourceC1[index] = C1;
            BeeSourceC2[index] = C2;
            BeeSourceW[index] = W;
        }
        public float getFitness()
        {
            float t = 0;
            for (int i = 0; i < BeeSourceW.Length; i++)
            {
                t = t + BeeSourceW[i];

            }
            return t;
        }
        public float getMaxSegmentWeight()
        {
            this.maxValue = BeeSourceW.Max();
            return BeeSourceW.Max();
        }
        public int indexMax(float maxValue)
        {
            return Array.IndexOf(BeeSourceW, maxValue);
        }
        public float getMinSegmentWeight()
        {
            this.maxValue = BeeSourceW.Min();
            return BeeSourceW.Min();
        }

        public void saveTrack(string path)
        {
            StreamWriter BizFile = new StreamWriter(Path.Combine(path, "TrackBz" + this.Track + ".txt"));
            for (int i = 0; i < NoS-1; i++)
                BizFile.WriteLine(BeeSource[i].X + "," + BeeSource[i].Y + "," + BeeSourceC1[i].X + "," + BeeSourceC1[i].Y + "," 
                    + BeeSourceC2[i].X + "," + BeeSourceC2[i].Y+ "," + BeeSource[i + 1].X + "," + BeeSource[i + 1].Y + "," + BeeSourceW[i]);
            BizFile.Close();
        }
        public void DuplicateBee(Bee DupBee)
        {

            this.Track = DupBee.Track;
            for (int i = 0; i < NoSegments; i++)
            {
                this.BeeSource[i] = DupBee.BeeSource[i];
                this.BeeSourceC1[i] = DupBee.BeeSourceC1[i];
                this.BeeSourceC2[i] = DupBee.BeeSourceC2[i];
                this.BeeSourceW[i] = DupBee.BeeSourceW[i];
            }
        }
    }
}

