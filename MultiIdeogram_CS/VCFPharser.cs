using System;
using System.Collections.Generic;
using System.Text;

namespace  MultiIdeogram_CS
    {
    public class VCFPharser
        {
        private bool isReady = false;
        private int chrom = -1;
        private int pos = -1;
        private int id = -1;
        private int refBase = -1;
        private int altBase = -1;
        private int qual = -1;
        private int filter = -1;
        private int info = -1;
        private int format = -1;
        private string[] formats = null;
        private int values = -1;
        private string[] valuess = null;
        private string name = null;
        private float alleleRatio = -1;
        private int alleleDepth = -1;
        private int readDepth = -1;
        private float genotypeQuality = -1;
        private int[] normalizedQualityScore = {-1,-1,-1};

        string[] items = null;

        public VCFPharser() { }

        public bool ReadLine(string thisLine, int readDepthCutOff, bool isGVCF, bool VCFGenotypes)
        {
            bool response = false;

            if (isReady == false && thisLine.StartsWith("#") == true)
            { response = NextFormatLine(thisLine); }
            else
            { response = NextDataLine(thisLine, readDepthCutOff,isGVCF, VCFGenotypes); }

            return response;

        }

        public bool ReadRSLine(string thisLine, int readDepthCutOff, bool isGVCF, bool VCFGenotypes) 
        {
            bool response = false;

            if (isReady == false && thisLine.StartsWith("#") == true)
            { response = NextFormatLine(thisLine); }
            else 
            { response = NextDataLineWithRS(thisLine, readDepthCutOff, isGVCF, VCFGenotypes); } 

            return response;

        }

        private bool NextFormatLine(string thisLine)
            {
            bool response = false;
            try
                {
                if (thisLine.StartsWith("##") != true)
                    {
                    isReady = true;

                    string[] items = thisLine.Split('\t');

                    for (int index = 0; index < items.Length; index++)
                        {
                        switch (items[index])
                            {
                            case "#CHROM":
                                chrom = index;
                                break;
                            case "POS":
                                pos = index;
                                break;
                            case "ID":
                                id = index;
                                break;
                            case "REF":
                                refBase = index;
                                break;
                            case "ALT":
                                altBase = index;
                                break;
                            case "QUAL":
                                qual = index;
                                break;
                            case "FILTER":
                                filter = index;
                                break;
                            case "INFO":
                                info = index;
                                break;
                            case "FORMAT":
                                format = index;
                                break;
                            default:
                                values = index;
                                name = items[index];
                                break;
                            }
                        }
                    }
                response = true;
                }
            catch { }

            return response;

            }

        private bool NextDataLineWithRS(string thisLine, int readDepthCutOff, bool isGVCF, bool VCFGenotypes) 
            {
            bool response = false;
            try
            {
                items = thisLine.Split('\t');
                formats = items[format].Split(':');
                valuess = items[values].Split(':');
                if (items[id].ToLower().StartsWith("rs") == true)
                {
                    if (formats.Length == valuess.Length)
                    { response = GetValues(readDepthCutOff, isGVCF, VCFGenotypes); }
                }
            }
            catch
            { }
            return response;
            }

       private bool NextDataLine(string thisLine, int readDepthCutOff, bool isGVCF, bool VCFGenotypes)
        {
            bool response = false;
            try
            {
                items = thisLine.Split('\t');
                formats = items[format].Split(':');
                valuess = null;
                valuess = items[values].Split(':');
                if (formats.Length == valuess.Length)
                { response = GetValues(readDepthCutOff, isGVCF, VCFGenotypes); } 
            }
            catch
            { }
            return response;

        } 

        private bool GetValues(int readDepthCutOff, bool isGVCF, bool VCFGenotypes)
        {
            string genotype = "";
            bool notTrialleleic = true;
            alleleRatio = 0;
            alleleDepth = 0;
            readDepth = 0;
            genotypeQuality = 0;
            normalizedQualityScore[0] = -1;
            normalizedQualityScore[1] = -1;
            normalizedQualityScore[2] = -1;

            bool hasAB = false;

            for (int index = 0; index < formats.Length; index++)
            {
                switch (formats[index])
                {
                    case "AB":
                        alleleRatio = Convert.ToSingle(valuess[index]);
                        hasAB = true;
                        break;
                    case "AD":
                        string[] t = valuess[index].Split(',');
                        if (t.Length == 3 && isGVCF == false)
                        {
                            notTrialleleic = false;
                        }
                        else { alleleDepth = Convert.ToInt32(t[1]); }
                        break;
                    case "DP":
                        readDepth = Convert.ToInt32(valuess[index]);
                        break;
                    case "GQ":
                        genotypeQuality = Convert.ToSingle(valuess[index]);
                        break;
                    case "GP":
                        string[] tt = valuess[index].Split(',');
                        if (tt.Length == 3)
                        {
                            if (char.IsDigit(tt[0][0]) == true) { normalizedQualityScore[0] = Convert.ToInt32(tt[0]); }
                            if (char.IsDigit(tt[1][0]) == true) { normalizedQualityScore[1] = Convert.ToInt32(tt[1]); }
                            if (char.IsDigit(tt[2][0]) == true) { normalizedQualityScore[2] = Convert.ToInt32(tt[2]); }
                        }
                        break;
                    case "GT":
                        genotype = valuess[index];
                        break;
                }
            }

            if (VCFGenotypes == false)
            {
                if (hasAB == false)
                { alleleRatio = (float)(readDepth - alleleDepth) / readDepth; }

                if (readDepth < readDepthCutOff)
                { notTrialleleic = false; }
            }
            else
            {
                if (readDepth == 0)
                { readDepth = 50; }
                if (genotype == "0/1" || genotype=="1/0")
                { 
                    alleleRatio = 0.5f;
                    alleleDepth =(int) (readDepth * alleleRatio);
                }
                else if (genotype == "1/1")
                {
                    alleleRatio = 1.0f;
                    alleleDepth = readDepth;
                }
                else
                { 
                    alleleRatio = 0.0f;
                    alleleDepth = 10;
                    notTrialleleic = false;
                }
            }

            return notTrialleleic;
        }

        public bool IsReady { get { return isReady; } }

        public int ChromosomeNumber
            {
            get
                {
                int chromosome = -1;
                if (ChromosomeString.Length < 6)
                {
                    switch (items[chrom].ToLower())
                    {
                        case "chrx":
                        case "x":
                            chromosome = 23;
                            break;
                        case "chry":
                        case "y":
                            chromosome = 24;
                            break;
                        case "chrm":
                        case "m":
                        case "mt":
                            chromosome = 25;
                            break;
                        default:
                            try
                            {
                                chromosome = Convert.ToInt32(ChromosomeString);
                            }
                            catch (Exception ex)
                            {
                                if (ChromosomeString.ToLower().Contains("mt") == true)
                                { chromosome = 25; }
                            }
                            break;
                    }
                }
                    return chromosome;
                }
            }

        public string ChromosomeString { get { return items[chrom].ToLower().Replace("chr", ""); } }
        public int Position { get { return Convert.ToInt32(items[pos]); } }
        public string ID { get { return items[id]; } }
        public string ReferenceBase { get { return items[refBase]; } }
        public string AlternateBase { get { return items[altBase]; } }
        public float QualityScore { get { return Convert.ToSingle(items[qual]); } }
        public string Filter { get { return items[filter]; } }
        public string Information { get { return items[info]; } }
        public string Format { get { return items[format]; } }
        public string Values { get { return items[values]; } }

        public float AlleleRatio { get { return alleleRatio; } }
        public int AlleleDepth { get { return alleleDepth; } }
        public int ReadDepth { get { return readDepth; } }
        public float GenotypeQuality { get { return genotypeQuality; } }
        public int[] NormalizedQualityScore { get { return normalizedQualityScore; } }

        public string Name { get { return name; } }

        }
    }
