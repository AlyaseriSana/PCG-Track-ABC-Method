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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int noPoints;

        static int maxiDance = 600;
        int maxIteration = 1;
        int Maxloop = 10;
        int MaxTrial = 2;
        System.Random r = new System.Random();

        bool foodsource = false;

        public static string getPath1 = @"H:\16-11-PHD\PhDprojects\dataABC2\";
        public static string getPath2 = @"H:\16-11-PHD\PhDprojects\SampleABC2\";
        public static string getPath3 = @"H:\16-11-PHD\PhDprojects\Testfile\";
        public static int CurrentCurve = Directory.GetFiles(getPath1).Length;
        //public StreamWriter testFile = new StreamWriter(Path.Combine(getPath3, "testfileABC.txt"));
        ExcelFile excelTest = new ExcelFile(@"H:\16-11-PHD\PhDprojects\Testfile\testABC3.xlsx", 1);

        int row = 1;
        int col = 1;

        Bee employeeB = new Bee();
        Bee employPartner = new Bee();
        Bee onlookerB = new Bee();
        Bee scoutB = new Bee();
        Bee bestBee = new Bee();

        

        
        float[] ProFoodsource = new float[maxiDance]; // save the fitness of each bee
        int[] limitSource = Enumerable.Repeat(0, maxiDance).ToArray(); // save the the number of improvment of each track
        GenerateTrack trackCurve = new GenerateTrack();
        private void Form1_Load(object sender, EventArgs e)
        {
            noPoints = employeeB.NoS;
            excelTest.excelclear();
            row = 1;
           for (int i = 0; i < maxiDance; i++)
                trackCurve.GenerateNewTrack(i);
            for (int i = 0; i < maxIteration; i++)
            {
                System.Diagnostics.Debug.WriteLine(" run no  " + i);
                DoABC();

                //col = col + 3;
            }



           excelTest.ExcelSave();
            excelTest.excelClose();
            MessageBox.Show("the work is done at " + scoutB.Track + " with wieght is " + scoutB.getFitness());
            for (int i = 0; i < noPoints; i++)
            {
                System.Diagnostics.Debug.WriteLine(scoutB.BeeSourceW[i]);
                
            }
            dataGridView1.DataSource = scoutB.BeeSourceW.ToList();
            dataGridView1
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
           GenerateTrack track1 = new GenerateTrack();
            Pen pen1 = new Pen(Brushes.Red, 4);
            Pen pen2 = new Pen(Brushes.Black, 4);
            Pen pen3 = new Pen(Brushes.Green, 4);
            track1.drawTrack(scoutB,pen1, e);
            //track1.drawTrack(employPartner,pen2, e);
            //track1.drawTrack(employeeB, pen3, e); 
        }


        void DoABC()
        {
            int k = 0;
            bool solution = false;

            int flag = 0;
            
            int rEbee = 0; //random number to get the random track 
            int rEbeeP = 0;//random number to get the random track 
            
            float EbeeFitness = 0;
            float UpdateFitness = 0;
            List<Bee> EmployeePhase = new List<Bee>(maxiDance);   // save all the Bees which has to be find the partner
            foodsource = false;
            for (int i = 0; i < maxiDance; i++)
            {
                GetTracks(i, employeeB, foodsource);
                EmployeePhase.Insert(i, new Bee());
                EmployeePhase[i].DuplicateBee(employeeB);
            }
            

                
                for (int iDance = 0; iDance < maxiDance; iDance++)
                {
                    // employee phase ( get maximum dance)
                   
                    foodsource = false;
                    int limit = 0;
                    while (!foodsource & (limit < CurrentCurve + 1))
                    {
                        rEbeeP = r.Next(0, CurrentCurve);
                        
                        
                        if (iDance == rEbeeP)
                            foodsource = false;
                        limit++;
                    }
                    employeeB.DuplicateBee(EmployeePhase[iDance]);
                    employPartner.DuplicateBee(EmployeePhase[rEbeeP]);
                    EbeeFitness = employeeB.getFitness();
                   
                    doUpdatemployBee(employeeB, employPartner, EmployeePhase[iDance]);
                  
                    UpdateFitness = EmployeePhase[iDance].getFitness();
                    System.Diagnostics.Debug.WriteLine("k is "+ k+ " the orginal fitness is " + EbeeFitness + " new fitness is " + UpdateFitness+ " track no is "+employeeB.Track);
                    if (UpdateFitness < EbeeFitness)
                        limitSource[rEbee] = 0;
                    else
                        limitSource[rEbee]++;

                  
                    
                }   //end of dance loop 
                    // Food source information
            do
            {
                float MaxFitnees = getMaxFitness(EmployeePhase);
                float[] p = new float[maxiDance];
                int Z = 0;
                foreach (Bee aBee in EmployeePhase)
                {
                    p[Z] = aBee.getFitness();
                    Z++;
                }

                for (int iDance = 0; iDance < maxiDance; iDance++)
                  {
                     ProFoodsource[iDance] = (p[iDance] / MaxFitnees)*100;
                   
                  }

                // Onlooker Phase
                
               
                for (int iDance = 0; iDance < maxiDance; iDance++)
                {
                    int rFood = r.Next(1, 100);
                    
                    if (rFood< ProFoodsource[iDance])
                    {
                        int rPartner = r.Next(0, maxiDance);
                        EbeeFitness = EmployeePhase[iDance].getFitness();
                        doUpdatemployBee2(EmployeePhase[iDance], EmployeePhase[rPartner]);
                        UpdateFitness = EmployeePhase[iDance].getFitness();
                        
                        if (UpdateFitness < EbeeFitness)
                        {
                            limitSource[EmployeePhase[iDance].Track] = 0;
                            
                            
                        }
                        else
                            limitSource[EmployeePhase[iDance].Track]++;
                        
                    }

                 } // end of looker phase 
                
                
                // Scout Bee Phase
                for (int i = 0; i < maxiDance; i++)
                {
                    int j = EmployeePhase[i].Track;
                    if (limitSource[j] > MaxTrial)
                    {  
                        GenerateTrack trackCurve = new GenerateTrack(); // generat a new track insted of the old one
                        trackCurve.GenerateNewTrack(j);
                    }
                } // end scout phase
                
                float x = ProFoodsource.Min();
                int bestBeeIndex = Array.IndexOf(ProFoodsource, x);
                System.Diagnostics.Debug.WriteLine("the min is " + bestBeeIndex + " and the value is " + x);
                bestBee.DuplicateBee(EmployeePhase[bestBeeIndex]);
                System.Diagnostics.Debug.WriteLine("the best bee is " + bestBee.getFitness() + " and the scout is  " + scoutB.getFitness());
                if (k == 0)
                   scoutB.DuplicateBee(bestBee);
                    
                    
               // System.Diagnostics.Debug.WriteLine(" ---- "+ k + " best index is  "+bestBeeIndex+ " , " + x + "(" + bestBee.Track + ")" + bestBee.getFitness() + "   , " + scoutB.getFitness());
                if (bestBee.getFitness() < scoutB.getFitness())
                {
                    flag = 0;
                    scoutB.DuplicateBee(bestBee);
                    System.Diagnostics.Debug.WriteLine(k + "( scot bee is  " + scoutB.Track + ")" + scoutB.getFitness());
                }
                k++;
                flag++;
                if (flag > maxiDance-1)
                {
                    solution = true;
                    scoutB.saveTrack(getPath2);
                    scoutB.saveTrack(getPath1);
                }
                row++;
                excelTest.writeXcelsheet(row, col, k.ToString());

                excelTest.writeXcelsheet(row, col + 2, scoutB.getFitness().ToString());

            } while ((k < Maxloop) & !solution);

            
            

        }
        void GetTracks(int index, Bee Xbee, bool FoodSource)
        {

            string[] lines = new string[noPoints];
            PointF pare = new PointF();
            PointF  ctr1 = new PointF();
            PointF ctr2 = new PointF();
            float w = 0;

            if (File.Exists(Path.Combine(getPath1, "TrackBz" + index + ".txt")))
            {
                StreamReader readTrack = new StreamReader(getPath1 + "TrackBz" + index + ".txt");
                Xbee.Track = index;
                int i = 0;
                while (!readTrack.EndOfStream)
                {
                    lines[i] = readTrack.ReadLine();
                    System.Console.WriteLine(lines[i]);
                    i++;
                }
                readTrack.Close();
                for (int j = 0; j < i; j++)
                {
                    
                    pare = ConPoint(lines[j]);
                    ctr1 = ConC1(lines[j]);
                    ctr2 = ConC2(lines[j]);
                    w = ConW(lines[j]);

                    Xbee.feedBee(j, pare, ctr1, ctr2, w);
                    if (j==0)
                        Xbee.feedBee(noPoints-1, pare, ctr1, ctr2, w);
                }
                
             
                foodsource = true;
            }
            else
                FoodSource = false;
        }

        PointF ConPoint(string rowfile)
        {
            string[] sArray = rowfile.Split(',');
            PointF result = new PointF(float.Parse(sArray[0]), float.Parse(sArray[1]));
            return result;
        }
        PointF ConC1(string rowfile)
        {
            string[] sArray = rowfile.Split(',');
            PointF result = new PointF(float.Parse(sArray[2]), float.Parse(sArray[3]));
            return result;
        }
        PointF ConC2(string rowfile)
        {
            string[] sArray = rowfile.Split(',');
            PointF result = new PointF(float.Parse(sArray[4]), float.Parse(sArray[5]));
            return result;
        }
        float ConW(string rowfile)
        {
            string[] sArray = rowfile.Split(',');
            float result = float.Parse(sArray[8]);
            return result;
        }





 

        void doUpdatemployBee(Bee EB, Bee EBp, Bee OB)
        {
            
            int rSegment = r.Next(0, noPoints-1);
            int rSegmentPlus = rSegment + 1;
            if (rSegmentPlus == noPoints)
                rSegmentPlus = 0;
            
            OB.Track = EB.Track;
            for (int i = 0; i < noPoints; i++)
            {
                OB.BeeSource[i] = EB.BeeSource[i];
                OB.BeeSourceW[i] = EB.BeeSourceW[i];
                OB.BeeSourceC1[i] = EB.BeeSourceC1[i];
                OB.BeeSourceC2[i] = EB.BeeSourceC2[i];
            }
               
            
            OB.BeeSource[rSegment] = EBp.BeeSource[rSegment];
            if (rSegmentPlus == noPoints - 1)
                OB.BeeSource[0] = EBp.BeeSource[rSegmentPlus];
            else
                OB.BeeSource[rSegmentPlus] = EBp.BeeSource[rSegmentPlus];
            OB.BeeSourceW[rSegment] = EBp.BeeSourceW[rSegment];
            OB.BeeSourceC1[rSegment]= EBp.BeeSourceC1[rSegment];
            OB.BeeSourceC2[rSegment] = EBp.BeeSourceC2[rSegment];

           
        }

        void doUpdatemployBee2(Bee EB, Bee EBp)
        {
            Bee OB = new Bee();
            int rSegment = r.Next(0, noPoints - 1);
            int rSegmentPlus = rSegment + 1;
            if (rSegmentPlus == noPoints)
                rSegmentPlus = 0;

            OB.Track = EB.Track;
            for (int i = 0; i < noPoints; i++)
            {
                OB.BeeSource[i] = EB.BeeSource[i];
                OB.BeeSourceW[i] = EB.BeeSourceW[i];
                OB.BeeSourceC1[i] = EB.BeeSourceC1[i];
                OB.BeeSourceC2[i] = EB.BeeSourceC2[i];
            }


            OB.BeeSource[rSegment] = EBp.BeeSource[rSegment];
            if (rSegmentPlus == noPoints - 1)
                OB.BeeSource[0] = EBp.BeeSource[rSegmentPlus];
            else
                OB.BeeSource[rSegmentPlus] = EBp.BeeSource[rSegmentPlus];
            OB.BeeSourceW[rSegment] = EBp.BeeSourceW[rSegment];
            OB.BeeSourceC1[rSegment] = EBp.BeeSourceC1[rSegment];
            OB.BeeSourceC2[rSegment] = EBp.BeeSourceC2[rSegment];

            float F1 = OB.getFitness();
            float F2 = EB.getFitness();
            if (F1 < F2)
            {
                for (int i = 0; i < noPoints; i++)
                {
                    EB.BeeSource[i] = OB.BeeSource[i];
                    EB.BeeSourceW[i] = OB.BeeSourceW[i];
                    EB.BeeSourceC1[i] = OB.BeeSourceC1[i];
                    EB.BeeSourceC2[i] = OB.BeeSourceC2[i];
                }
            }
        }



        float getMaxFitness(List<Bee> BeeList)
        {
            float[] WList = new float[maxiDance];
            int i=0;
            foreach (Bee aBee in BeeList)
            {
                WList[i] = aBee.getFitness();
                i++;
            }
            
            return WList.Max();
        }


    }








}
