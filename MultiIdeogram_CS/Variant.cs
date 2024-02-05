using System;
using System.Collections.Generic;
using System.Text;

namespace  MultiIdeogram_CS
{
    public class Variant
    {
        private int chrom = -1;
        private int pos = -1;
        private string id = null;
        private string refBase = null;
        private string altBase = null;
        private string[] refSequence = null;
        private string[] altSequence = null;
        private bool isCorrect = true;
        private float[] alleleRatio = null;
        private int[] alleleDepth = null;
        private int[] readDepth = null;
        private float[] genotypeQuality = null;
        private int[] normalizedQualityScore = null;
        private int Count = 0;
        private bool isInCluser;
        private int AffectedGenotype = -1;
        private int AffectedGenotypeAll = -1;
        private int AffectedBB = 0;

        public Variant(int NumberOfFiles, int Chromosome, int Position, string ReferenceString, string AlternativeString, string RSID)
        {
            chrom = Chromosome;
            pos = Position;
            refBase = ReferenceString;
            altBase = AlternativeString;
            id = RSID;

            refSequence = new string[NumberOfFiles];
            altSequence = new string[NumberOfFiles];
            alleleRatio = new float[NumberOfFiles];
            alleleDepth = new int[NumberOfFiles];
            readDepth = new int[NumberOfFiles];
            genotypeQuality = new float[NumberOfFiles];
            normalizedQualityScore = new int[NumberOfFiles];
           
        }
             
        public void AddVariant(VCFPharser vp, int index)
        {

            if (altBase.Equals(vp.AlternateBase) == false)
            { isCorrect = false; }
            try
            {
                alleleRatio[index] = vp.AlleleRatio;
                alleleDepth[index] = vp.AlleleDepth;
                readDepth[index] = vp.ReadDepth;
                float r = (float)(readDepth[index] - alleleDepth[index]) / readDepth[index];
                               
                genotypeQuality[index] = vp.GenotypeQuality;
               Count++;
            }
            catch (Exception ex)
            { isCorrect = false; }

        }

        public void UpdateReadDepth(int NewReadDepth, int index)
        {
            if (readDepth[index] == 0)
            {
                alleleRatio[index] = 1;
                readDepth[index] = NewReadDepth;
                genotypeQuality[index] = 1000;
                normalizedQualityScore[index] = 1000;
            }
        }

        public bool HasError { get { return !isCorrect; } }
        public int Chromosome { get { return chrom; } }
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

        public int Position { get { return pos; } }
        public bool DataFromAll(int theCount) { return theCount == Count; }
        public bool InCluster { get { return isInCluser; } set { isInCluser = value; } }
        public static string Titles()
        {
            return "SNP ID" + "\t" + "dbSNP RS ID" + "\t" + "Chromosome" + "\t" + "Physical Position" + "\t" + "Result_Call" +
                "\t" + "Variant" + "\t" + "Read depth" + "\t" + "Variant depth";
        }
        public int ReadDepth(int index) { return readDepth[index]; }
        public int AlleleDepth(int index) { return alleleDepth[index]; }
        public string Name(int counter)
        {
            string rs = null;
            if (id.Length > 2)
            { rs = id; }
            else { rs = "rsx" + counter.ToString(); }

            return rs;
        }

        public void SetToSecondLowestReadDepth()
        {
            if (readDepth.Length > 1)
            {
                int[] temp = new int[readDepth.Length];
                Array.Copy(readDepth, temp, readDepth.Length);
                Array.Sort(temp);
                if (temp[0] == 0 && temp[1] != 0)
                {
                    for (int index = 0; index < readDepth.Length; index++)
                    {
                        if (readDepth[index] == 0)
                        {
                            alleleRatio[index] = 1;
                            readDepth[index] = temp[1];
                        }
                    }
                }
            }
        }

        public void setGenotypeAffectedCode(int minmumReadDepth, bool[] Affecteds)
        {
            int affected = 0;
            int aa = 0;
            int bb = 0;
            int ab = 0;
            int unbb = 0;
            int score = 0;
            AffectedBB = 0;
            AffectedGenotype = -1;

            for (int index = 0; index < Affecteds.Length; index++)
            {
                score = GenotypeNumber(index, minmumReadDepth);
                if (Affecteds[index] == true)
                {
                    affected++;
                    if (score == 2)
                    { bb++; }
                    else if (score == 1)
                    { aa++; }
                    else if (score == 3) { ab++; }
                }
                else
                {
                    if (score == 2)
                    { unbb++; }
                }
            }

            if (aa == 0 && bb > 0)
            { AffectedGenotype = 2; }
            else if (bb == 0 && aa > 0)
            { AffectedGenotype = 1; }


            if (bb == affected)
            {
                AffectedGenotypeAll = 2;
                if (unbb == 0)
                { AffectedBB = 2; }
            }
            else if (aa == affected)
            { AffectedGenotypeAll = 1; }

        }

        public bool isAffectedGenotype(int index, int minmumReadDepth)
        {
            bool answer = false;
            if (AffectedBB == 2)
            {
                if (GenotypeNumber(index, minmumReadDepth) == 2)
                { answer = true; }
            }

            return answer;
        }      

        //0 = nocall, 1 = match, 5 = no match
        public int isCommonAffectedGenotype(int index, int minmumReadDepth)
        {
            int answer = 0;
            int score = GenotypeNumber(index, minmumReadDepth);

            if (score == AffectedGenotype)
            { answer = 1; }
            else if (score == 0)
            { answer = 0; }
            else 
            { answer = 5; }

            return answer;
        }

        public string Genotype(int index, int minmumReadDepth)
        {

            string answer = "NoCall";
            if (id.Equals(".") == true) { minmumReadDepth = minmumReadDepth * 5; }

            if (readDepth[index] >= minmumReadDepth)
            {
                if (alleleRatio[index] <= 0.20f)
                { answer = "BB"; }
                else if (alleleRatio[index] >= 0.35f && alleleRatio[index] <= 0.65f)
                { answer = "AB"; }
                else if (alleleRatio[index] >= 0.8f)
                { answer = "AA"; }
                else { answer = "NoCall"; }
            }
            else
            {
                int half = (int)(minmumReadDepth + 1) / 2;
                if (alleleRatio[index] <= 0.2f && readDepth[index] >= half)
                { answer = "BB"; }
                else if (alleleRatio[index] >= 0.8f && readDepth[index] >= half)
                { answer = "AA"; }
                else { answer = "NoCall"; }
            }

            return answer;
        }

        public string GenotypeBases(int index, int minmumReadDepth)
        {

            string answer = "NoCall";

            if (id.Equals(".") == true) { minmumReadDepth = minmumReadDepth * 5; }

            if (readDepth[index] >= minmumReadDepth)
            {
                if (alleleRatio[index] <= 0.2f)
                { answer = altBase + ":" + altBase; }
                else if (alleleRatio[index] >= 0.4f && alleleRatio[index] <= 0.6f)
                { answer = refBase + ":" + altBase; }
                else if (alleleRatio[index] >= 0.8f)
                { answer = refBase + ":" + refBase; }
                else { answer = "NoCall"; }
            }
            else
            {
                int half = (int)(minmumReadDepth + 1) / 2;
                if (alleleRatio[index] <= 0.2f && readDepth[index] >= half)
                { answer = altBase + ":" + altBase; }
                else if (alleleRatio[index] >= 0.8f && readDepth[index] >= half)
                { answer = refBase + ":" + refBase; }
                else { answer = "NoCall"; }
            }

            return answer;
        }

        public int GenotypeType(int index, int minmumReadDepth)
        {
            // nocall =1, aa = 4, bb = 16 ab = 64
            int answer = 1;

            if (id.Equals(".") == true) { minmumReadDepth = minmumReadDepth * 5; }

            if (readDepth[index] >= minmumReadDepth)
            {
                if (alleleRatio[index] <= 0.2f)
                { answer = 16; }
                else if (alleleRatio[index] >= 0.35f && alleleRatio[index] <= 0.65f)
                { answer = 64; }
                else if (alleleRatio[index] >= 0.8f)
                { answer = 4; }
                else { answer = 1; }
            }
            else
            {
                int half = (int)(minmumReadDepth + 1) / 2;
                if (alleleRatio[index] <= 0.2f && readDepth[index] >= half)
                { answer = 16; }
                else if (alleleRatio[index] >= 0.8f && readDepth[index] >= half)
                { answer = 4; }
                else { answer = 1; }
            }

            return answer;
        }

        public int GenotypeNumber(int index, int minmumReadDepth)
        {
            // nocall =0, aa = 1, bb = 2 ab = 3
            int answer = 0;

            if (id.Equals(".") == true) { minmumReadDepth = minmumReadDepth * 2; }

            if (readDepth[index] >= minmumReadDepth)
            {
                if (alleleRatio[index] < 0.20f)
                { answer = 2; }
                else if (alleleRatio[index] >= 0.4f && alleleRatio[index] <= 0.6f)
                { answer = 3; }
                else if (alleleRatio[index] > 0.80f)
                { answer = 1; }
                else { answer = 0; }
            }
            else
            {
                int half = (int)(minmumReadDepth + 1) / 2;
                if (alleleRatio[index] <= 0.2f && readDepth[index] >= half)
                { answer = 2; }
                else if (alleleRatio[index] >= 0.8f && readDepth[index] >= half)
                { answer = 1; }
                else { answer = 0; }
            }

            return answer;
        }

        public string Write(int index, int counter, int minmumReadDepth)
        {
            string answer = null;
            string rs = null;
            if (id.Length > 2)
            { rs = id; }
            else { rs = "rsx" + counter.ToString(); }

            answer = "SNP" + counter.ToString() + "\t" + rs + "\t" + ChromosomeString + "\t" + pos.ToString() + "\t" + Genotype(index, minmumReadDepth) + "\t"
                + refBase + ">" + altBase + "\t" + readDepth[index].ToString() + "\t" + alleleDepth[index].ToString();

            return answer;
        }

        public override string ToString()
        {
            return Position.ToString() + " " + refBase + " " + altBase;
        }

        public string ReferenceBase { get { return refBase; } }
        public string VariantBase { get { return altBase; } }

        public void clearVariants()
        {
            refSequence =new string[1];
            altSequence= new string[1];
        }

          private string getAllele(string theAllele, string OtherAllele)
        {
            if (theAllele.Length > 1 || OtherAllele.Length > 1)
            { return theAllele.Substring(1); }
            else { return theAllele; }
        }

        #region de novo

        private bool testKids(int[] iKidsAffected, int[] iKidsUnAffected, int[] NonMendelian, bool deletions, int minimumReadDepth)
            {
            bool answer = true;

            int Score = -1;

            for (int index = 0; index < iKidsAffected.Length; index++)
                {
                Score = GenotypeType(iKidsAffected[index], minimumReadDepth);
                if (deletions == false)
                { if (Score == 4 || Score == 16) { answer = false; } }
                else
                { if (Score == 64) { answer = false; } }
                }

            if (answer == true)
                {
                for (int index = 0; index < iKidsUnAffected.Length; index++)
                    {
                    Score = GenotypeType(iKidsUnAffected[index], minimumReadDepth);
                    if (Score != 1)
                        {
                        foreach (int value in NonMendelian)
                        { if (value == Score) { answer = false; } }
                        }
                    }
                }

            return answer;

            }
        
        public bool HasDeNovo(int iDad, int iMum, int[] iKidsAffected, int[] iKidsUnAffected, int minimumReadDepth, bool NoDeletions)
        {
        int[] NonMendelian = null;
        
            bool answer = false;
            int kidsGenotype = 0;
            int parentsGenotype = GenotypeType(iDad, minimumReadDepth) + GenotypeType(iMum, minimumReadDepth);

            switch (parentsGenotype)
                {
                case 2:
                case 5:
                case 17:
                case 65:
                    answer = false;// nocall
                    break;
                case 8: //AA AA
                    for (int index = 0; index < iKidsAffected.Length; index++)
                        {
                        kidsGenotype = GenotypeType(iKidsAffected[index], minimumReadDepth);
                        if (kidsGenotype != 1 && kidsGenotype != 4)
                            {
                            NonMendelian = new int[] { 16, 64 };
                            answer = testKids(iKidsAffected, iKidsUnAffected, NonMendelian, NoDeletions, minimumReadDepth);
                            }
                        }
                    // can only be AA 4
                    break;
                case 32://BB BB
                    for (int index = 0; index < iKidsAffected.Length; index++)
                        {
                        kidsGenotype = GenotypeType(iKidsAffected[index], minimumReadDepth);
                        if (kidsGenotype != 1 && kidsGenotype != 16)
                            {
                             //mendelian = new int[] { 16 };
                            NonMendelian = new int[] { 4, 64 };
                            answer = testKids(iKidsAffected, iKidsUnAffected, NonMendelian, NoDeletions, minimumReadDepth);
                            }
                        }
                    // can only be BB 16
                    break;
                case 20: //AA BB
                    for (int index = 0; index < iKidsAffected.Length; index++)
                        {
                        kidsGenotype = GenotypeType(iKidsAffected[index], minimumReadDepth);
                        if (kidsGenotype != 1 && kidsGenotype != 64)
                            {
                             NonMendelian = new int[] { 4, 16 };
                            answer = testKids(iKidsAffected, iKidsUnAffected, NonMendelian, NoDeletions, minimumReadDepth);
                            }
                        }
                    // can only be AB 64
                    break;
                case 68://AA AB
                    for (int index = 0; index < iKidsAffected.Length; index++)
                        {
                        kidsGenotype = GenotypeType(iKidsAffected[index], minimumReadDepth);
                        if (kidsGenotype != 1 && kidsGenotype == 16)
                            {
                            //mendelian = new int[] { 4, 64 };
                            NonMendelian = new int[] { 16 };
                            answer = testKids(iKidsAffected, iKidsUnAffected, NonMendelian, NoDeletions, minimumReadDepth);
                            }
                        }
                    // be only AA or AB 4 or 64
                    break;
                case 80://BB AB
                    for (int index = 0; index < iKidsAffected.Length; index++)
                        {
                        kidsGenotype = GenotypeType(iKidsAffected[index], minimumReadDepth);
                        if (kidsGenotype != 1 && kidsGenotype == 4)
                            {
                           NonMendelian = new int[] { 4 };
                            answer = testKids(iKidsAffected, iKidsUnAffected, NonMendelian, NoDeletions, minimumReadDepth);
                            }
                        }
                    // can be only BB or AB 16 or 64
                    break;
                case 128:
                    //answer = false;
                    // can be anything
                    break;
                default:
                    // failed = no
                    //answer = false;
                    break;
                }

            return answer;
        }

        public static string DeNovoLineTitles(string dad, string mum, string[] kidsAffected, string[] kidsUnAffected)
        {
            string answer = "RS name" + "\t" + "Chromosome" + "\t" + "Position" + '\t' + "Reference" + "\t" + "Alternative" + '\t' +
                dad + "\tRef|Alt read depth\t" +
                mum + "\tRef|Alt read depth";

            for (int index = 0; index < kidsAffected.Length; index++)
            { answer += '\t' + kidsAffected[index] + "\tRef/Alt read depth"; }

            for (int index = 0; index < kidsUnAffected.Length; index++)
            { answer += '\t' + kidsUnAffected[index] + "\tRef/Alt read depth"; }

            return answer;

        }

        public string WriteDeNovoLine(int iDad, int iMum, int[] iKidsAffected, int[] iKidsUnAffected, int minimumReadDepth)
        {
            string answer = id + "\t" + ChromosomeString + "\t" + pos.ToString() + "\t" + refBase + "\t" + altBase + "\t" +
                GenotypeBases(iDad, minimumReadDepth) + '\t' + (readDepth[iDad] - alleleDepth[iDad]) + "|" + alleleDepth[iDad] + '\t' +
                GenotypeBases(iMum, minimumReadDepth) + '\t' + (readDepth[iMum] - alleleDepth[iMum]) + "|" + alleleDepth[iMum];

            for (int index = 0; index < iKidsAffected.Length; index++)
            { answer += '\t' + GenotypeBases(iKidsAffected[index], minimumReadDepth) + '\t' + (readDepth[iKidsAffected[index]] - alleleDepth[iKidsAffected[index]]) + "|" + alleleDepth[iKidsAffected[index]]; }

            for (int index = 0; index < iKidsUnAffected.Length; index++)
            { answer += '\t' + GenotypeBases(iKidsUnAffected[index], minimumReadDepth) + '\t' + (readDepth[iKidsUnAffected[index]] - alleleDepth[iKidsUnAffected[index]]) + "|" + alleleDepth[iKidsUnAffected[index]]; }

            return answer;
        }

        #endregion     
    }
}
