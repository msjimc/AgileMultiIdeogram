using System;
using System.Collections.Generic;
using System.Text;

namespace  MultiIdeogram_CS
{
    public class SeqVariant
    {
        private int chrom = -1;
        private int pos = -1;
        private string id = null;
        private string refBase = null;
        private string altBase = null;
        private bool isCorrect = true;
        private float alleleRatio = 0;
        private int alleleDepth = 0;
        private int readDepth = 0;
        private float genotypeQuality = 0;
        private int normalizedQualityScore = 0;

        private int Count = 0;
        public SeqVariant(int Chromosome, int Position, string ReferenceString, string AlternativeString, string RSID)
        {
            chrom = Chromosome;
            pos = Position;
            refBase = ReferenceString;
            altBase = AlternativeString;
            id = RSID;
        }

        public void AddVariant(VCFPharser vp)
        {
            if (altBase.Equals(vp.AlternateBase) == false)
            {
                isCorrect = false;
            }
            try
            {
                alleleRatio = vp.AlleleRatio;
                alleleDepth = vp.AlleleDepth;
                readDepth = vp.ReadDepth;
                genotypeQuality = vp.GenotypeQuality;
                Count += 1;
            }
            catch 
            {
                isCorrect = false;
            }

        }

        public string Name
        {
            get { return id; }
        }

        public bool HasError
        {
            get { return !isCorrect; }
        }

        public int Chromosome
        {
            get { return chrom; }
        }

        public int ReadDepth
        { get { return readDepth; } }

        public string ChromosomeString
        {
            get
            {
                string answer = null;
                switch (chrom)
                {
                    case 23:
                        answer = "X";
                        break; 
                    case 24:
                        answer = "Y";
                        break;
                    case 25:
                        answer = "M";
                        break;
                    default:
                        answer = chrom.ToString();
                        break; 
                }
                return answer;
            }
        }

        public int Position
        {
            get { return pos; }
        }

        public bool DataFromAll(int theCount)
        {
            return theCount == Count;
        }

        public static string Titles()
        {
            return "SNP ID" + Constants.vbTab + "dbSNP RS ID" + Constants.vbTab + "Chromosome" + Constants.vbTab + "Physical Position" + Constants.vbTab + "Result_Call";
        }

        public string Genotype(int minmumReadDepth, bool harsh)
        {

            string answer = "NoCall";
            if (id.Length < 3)
                minmumReadDepth = minmumReadDepth * 5;

            if (readDepth > minmumReadDepth)
            {
                if (harsh == true)
                {
                    if (alleleRatio < 0.17f)
                    {
                        answer = "BB";
                    }
                    else if (alleleRatio > 0.33f && alleleRatio < 0.67f)
                    {
                        answer = "AB";
                    }
                    else if (alleleRatio > 0.64f)
                    {
                        answer = "AA";
                    }
                    else
                    {
                        answer = "NoCall";
                    }
                }
                else
                {
                    if (alleleRatio < 0.17f)
                    {
                        answer = "BB";
                    }
                    else if (alleleRatio > 0.33f && alleleRatio < 0.67f)
                    {
                        answer = "AB";
                    }
                    else if (alleleRatio > 0.64f)
                    {
                        answer = "AA";
                    }
                    else
                    {
                        answer = "NoCall";
                    }
                }


            }

            return answer;
        }

        public string Write(int counter, int minmumReadDepth, bool harshGenotyping)
        {
            string answer = null;
            string rs = null;
            if (id.Length > 2)
            {
                rs = id;
            }
            else
            {
                rs = "rsx" + counter.ToString();
            }

            answer = "SNP" + counter.ToString() + Constants.vbTab + rs + Constants.vbTab + ChromosomeString + Constants.vbTab + pos.ToString() + Constants.vbTab + Genotype(minmumReadDepth, harshGenotyping) + Constants.vbTab + readDepth.ToString() + Constants.vbTab + alleleDepth.ToString();

            return answer;
        }

    }
}
