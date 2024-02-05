using System;
using System.Collections.Generic;
using System.Text;

namespace  MultiIdeogram_CS
{
    class SeqSNP
    {
        public enum State
        {
            Unknown = 0,
            Homozygous = 1
        }

        private byte TheGenotype;
        private double snpDistance;
        private string SNPName;
        private bool Failed;
        private int Runs;

        private bool theInCluster;

        public SeqSNP(double Distance, string Name, string Genotype)
        {
            snpDistance = Distance;
            SNPName = Name;

            switch (Genotype.ToUpper(System.Globalization.CultureInfo.InvariantCulture))
            {
                case "AA":
                    TheGenotype = 1;
                    break;
                case "BB":
                    TheGenotype = 2;
                    break;
                case "AB":
                    TheGenotype = 3;
                    break;
                default:
                    if (Genotype.Length == 2)
                    {
                        if (Genotype[0] == Genotype[1])
                        { TheGenotype = 1; }
                        else
                        { TheGenotype = 3; }
                    }
                    else
                    {
                         Failed = true;
                    }
                    break;
            }

        }

        public byte Genotype
        {
            get { return TheGenotype; }
        }

        public string Name
        {
            get { return SNPName; }
        }

        public double Position
        {
            get { return snpDistance; }
            set { snpDistance = value; }
        }

        public bool InCluster
        {
            get { return theInCluster; }
            set { theInCluster = value; }
        }        

        public bool FailedData
        {
            get { return Failed; }
        }

        public int HomozygousRuns(int CurrentRun)
        {

            if (this.TheGenotype == 3)
            {
                CurrentRun = 0;
            }
            else if (TheGenotype > 0)
            {
                CurrentRun += 1;
            }

            Runs = CurrentRun;

            return CurrentRun;

        }

        public int MaximumRun(int CurrentMaximum)
        {
            if (Runs == 0)
            { CurrentMaximum = 0;   }
            else if (Runs > CurrentMaximum)
            {  CurrentMaximum = Runs; }
            else if (CurrentMaximum > Runs)
            { Runs = CurrentMaximum; }

            return CurrentMaximum;

        }


        public void WrongCallHarsh(int before, int After, int RunsCutOff, float fraction)
        {
            if (this.Runs == 0)
            {
                if (before + After >= RunsCutOff)
                {
                    if (before > RunsCutOff * fraction && After > RunsCutOff * fraction)
                    {
                        Failed = true;
                    }
                }
            }
        }


        public void WrongCall(int before, int After, int RunsCutOff)
        {
            if (this.Runs == 0)
            {
                if (before + After >= RunsCutOff)
                {
                    Failed = true;
                }
            }
        }

        public int MyRuns
        {
            get { return Runs; }
        }

        public void ResetHomozygousRuns()
        {
            Runs = 0;
        }

    }
}
