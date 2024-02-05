
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace  MultiIdeogram_CS
{

    public enum TheVariantType
    {
        Exonic = 0,
        Intronic = 1,
        SpliceSite = 2,
        KozakSite = 3,
        Indels = 4,
        SNPs = 5,
        All = 6,
        Severity = 7
    }

    public enum SNPGenotypeType
    {
        Both = 0,
        Homozygous = 1,
        Heterozygous = 2
    }


    public enum Severity
    {
        NoAffect = 0,
        PossibleAffect = 1,
        Affect = 2,
        Unknown = 3
    }

    public enum VariantState
    {
        NotSet = 0,
        homozygousWildType = 1,
        homozygousMutant = 2,
        Heterozygous = 3,
        LowReadDepth = 4
    }

    public enum Status
    {
        NotFiltered = 0,
        NotKnown = 1,
        In1000genomes = 2,
        HasRSnumber = 3
    }

    public enum SNPType
    {
        SNP = 0,
        Deletion = 1,
        Insertion = 2
    }

    public enum SNPLocation
    {
        Intronic = 0,
        Intron5prime = 1,
        Intron3Prime = 2,
        SpliceSite3Prime = 3,
        SpliceSite5Prime = 4,
        Exon = 5,
        kozaksite = 6,
        ExonDeletion =7
    }

    public enum Nucleotide
    {
        Notset = 0,
        A = 65,
        delA = 66,
        C = 67,
        delC = 68,
        G = 71,
        delG = 72,
        K = 75,
        M = 77,
        N = 78,
        R = 82,
        S = 83,
        T = 84,
        delT = 85,
        W = 87,
        y = 89
    }

    public enum RelativePosition
    {
        NotSet = 0,
        Before = 1,
        InMe = 2,
        After = 3
    }

    public enum Criteria
    {
        NotSet = 0,
        ByName = 1,
        ByRegion = 2,
        ByChromosome = 3
    }

    public struct Sequence
    {
        private byte[] Read;
        internal int Position;

        internal byte ExonIndex;
        internal Sequence(string TheRead, string TheQuality, int TheReadPosition, int TheExonIndex)
        {
            Read = SharedFunctions.StringToBytes(TheRead);
            byte[] Quality = SharedFunctions.StringToBytes(TheQuality);

            for (int Base = 0; Base <= TheRead.Length - 1; Base++)
            {
                try
                {
                    if (DNA.GetPhred(Quality[Base]) < 20)
                    {
                        Read[Base] = 78;
                    }

                }
                catch (Exception ex)
                {
                }
            }

            ExonIndex = Convert.ToByte(TheExonIndex);
            Position = TheReadPosition;

        }

        internal byte[] ReadByte
        {
            get { return Read; }
        }


        internal string ReadString
        {
            get { return SharedFunctions.BytesToString(Read); }
        }

    }

    public struct DNAFragment
    {
        internal int StartPoint;
        internal int EndPoint;
        internal DNAFragment(int TheStartPoint, int TheEndPoint)
        {
            StartPoint = TheStartPoint;
            EndPoint = TheEndPoint;
        }

        internal int Length
        {
            get { return EndPoint - StartPoint; }
        }

        public static bool operator ==(DNAFragment ThisFragment, DNAFragment ThatFragment)
        {
            if (ThisFragment.StartPoint == ThatFragment.StartPoint && ThisFragment.EndPoint == ThatFragment.EndPoint)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(DNAFragment ThisFragment, DNAFragment ThatFragment)
        {
            return !(ThisFragment == ThatFragment);
        }

        public bool Equals(DNAFragment aFragment)
        {
            if (aFragment == this)
            { return true; }
            else { return false; }
        }

        public override int GetHashCode()
        {
            return StartPoint + EndPoint;
        }

    }

    public class DNARegion: IComparable<DNARegion>
    {
        internal int StartPoint;
        internal int EndPoint;

        internal int Chromosome;

        internal DNARegion(){}

        internal DNARegion(DNARegion aRegion)
        {
            Chromosome = aRegion.Chromosome;
            StartPoint = aRegion.StartPoint;
            EndPoint = aRegion.EndPoint;
        }

        internal DNARegion(string Data)
        {
            string[] items = Data.Split('\t');
            Chromosome = Convert.ToInt32(items[0]);
            StartPoint = Convert.ToInt32(items[1]);
            EndPoint = Convert.ToInt32(items[2]);
        }

        internal DNARegion(int TheChromosome, int TheStartPoint, int TheEndPoint)
        {
            StartPoint = TheStartPoint;
            EndPoint = TheEndPoint;
            Chromosome = TheChromosome;
        }

        internal int Length
        {
            get { return EndPoint - StartPoint; }
        }

        public static bool operator ==(DNARegion ThisFragment, DNARegion ThatFragment)
        {
            try
            {
                if (ThisFragment.Chromosome == ThatFragment.Chromosome)
                {
                    if (ThisFragment.StartPoint == ThatFragment.StartPoint && ThisFragment.EndPoint == ThatFragment.EndPoint)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            { return false; }
        }

        public Int32 CompareTo(DNARegion aRegion)
        {
            if (aRegion == null)
            { return 1; }
            else
            {
                if (Chromosome == aRegion.Chromosome)
                { return StartPoint.CompareTo(aRegion.StartPoint); }
                else { return Chromosome.CompareTo(aRegion.Chromosome); }
            }

        }

        public static bool operator !=(DNARegion ThisFragment, DNARegion ThatFragment)
        {
            return !(ThisFragment == ThatFragment);
        }

        public bool Equals(DNARegion aFragment)
        {
            if (aFragment == this)
            { return true; }
            else { return false; }
        }

        public override int GetHashCode()
        {
            return StartPoint + EndPoint;
        }

        public override string ToString()
        {
            return Chromosome.ToString() + '\t' + StartPoint.ToString() + '\t' + EndPoint.ToString() + '\t' + Length;
        }

        public string ToStringTerse()
        {
            return "chr" + Chromosome.ToString() + ":" + StartPoint.ToString() + "-" + EndPoint.ToString();
        }
                
        public string Description()
        {
            return Chromosome.ToString() + ':' + StartPoint.ToString("0,0") + '-' + EndPoint.ToString("0,0");
        }

        public void AdjustEnd(int newEndPoint)
        { EndPoint = newEndPoint; }

        public void adjustStart(int newStartPoint)
        { StartPoint = newStartPoint; }
    }

    public struct thedata
    {
        public int geneID;
        public int transcriptID;
        public int exonID;
    }

}

