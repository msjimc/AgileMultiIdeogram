using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace  MultiIdeogram_CS
{
    public class SharedFunctions
    {

        public static string TranslateCodon(string Codon)
        {

            if (Codon == "TTC" | Codon == "TTT")
            {
                return ("F");
            }
            else if (Codon == "TTA" | Codon == "TTG")
            {
                return ("L");
            }
            else if (Codon.StartsWith("TC"))
            {
                return ("S");
            }
            else if (Codon == "TAC" | Codon == "TAT")
            {
                return ("Y");
            }
            else if (Codon == "TAA" | Codon == "TAG")
            {
                return ("*");
            }
            else if (Codon == "TGC" | Codon == "TGT")
            {
                return ("C");
            }
            else if (Codon == "TGA")
            {
                return ("*");
            }
            else if (Codon == "TGG")
            {
                return ("W");
            }
            else if (Codon.StartsWith("CT"))
            {
                return ("L");
            }
            else if (Codon.StartsWith("CC"))
            {
                return ("P");
            }
            else if (Codon.StartsWith("CG"))
            {
                return ("R");
            }
            else if (Codon == "CAA" | Codon == "CAG")
            {
                return ("Q");
            }
            else if (Codon == "CAC" | Codon == "CAT")
            {
                return ("H");
            }
            else if (Codon.StartsWith("AT"))
            {
                if (Codon == "ATG")
                {
                    return ("M");
                }
                else
                {
                    return ("I");
                }
            }
            else if (Codon.StartsWith("AC"))
            {
                return ("T");
            }
            else if (Codon == "AAA" | Codon == "AAG")
            {
                return ("K");
            }
            else if (Codon == "AAC" | Codon == "AAT")
            {
                return ("N");
            }
            else if (Codon == "AGC" | Codon == "AGT")
            {
                return ("S");
            }
            else if (Codon == "AGA" | Codon == "AGG")
            {
                return ("R");
            }
            else if (Codon.StartsWith("GT"))
            {
                return ("V");
            }
            else if (Codon.StartsWith("GC"))
            {
                return ("A");
            }
            else if (Codon == "GAC" | Codon == "GAT")
            {
                return ("D");
            }
            else if (Codon == "GAA" | Codon == "GAG")
            {
                return ("E");
            }
            else if (Codon.StartsWith("GG"))
            {
                return ("G");
            }
            else
            {
                return ("X");
            }

        }

        public static string TranslateNoSpaces(string DNA)
        {
            int Length = DNA.Length - 1;
            int lop = 0;
            System.Text.StringBuilder aminoacid = new System.Text.StringBuilder();
            string caharact = null;
            DNA = DNA.ToLower();

            for (lop = 0; lop <= Length; lop += 3)
            {
                if (lop < Length - 2)
                {
                    caharact = DNA.Substring(lop, 3);

                    if (caharact == "ttc" | caharact == "ttt" | caharact == "tty")
                    {
                        aminoacid.Append("F");
                    }
                    else if (caharact == "tta" | caharact == "ttg" | caharact == "ttr")
                    {
                        aminoacid.Append("L");
                    }
                    else if (caharact.Substring(0, 2) == "tc")
                    {
                        aminoacid.Append("S");
                    }
                    else if (caharact == "tac" | caharact == "tat" | caharact == "tay")
                    {
                        aminoacid.Append("Y");
                    }
                    else if (caharact == "taa" | caharact == "tag" | caharact == "tar")
                    {
                        aminoacid.Append("*");
                    }
                    else if (caharact == "tgc" | caharact == "tgt" | caharact == "tgy")
                    {
                        aminoacid.Append("C");
                    }
                    else if (caharact == "tga")
                    {
                        aminoacid.Append("*");
                    }
                    else if (caharact == "tgg")
                    {
                        aminoacid.Append("W");
                    }
                    else if (caharact.Substring(0, 2) == "ct")
                    {
                        aminoacid.Append("L");
                    }
                    else if (caharact.Substring(0, 2) == "cc")
                    {
                        aminoacid.Append("P");
                    }
                    else if (caharact.Substring(0, 2) == "cg")
                    {
                        aminoacid.Append("R");
                    }
                    else if (caharact == "caa" | caharact == "cag" | caharact == "car")
                    {
                        aminoacid.Append("Q");
                    }
                    else if (caharact == "cac" | caharact == "cat" | caharact == "cay")
                    {
                        aminoacid.Append("H");
                    }
                    else if (caharact.Substring(0, 2) == "at")
                    {
                        if (caharact == "atg")
                        {
                            aminoacid.Append("M");
                        }
                        else
                        {
                            aminoacid.Append("I");
                        }
                    }
                    else if (caharact.Substring(0, 2) == "ac")
                    {
                        aminoacid.Append("T");
                    }
                    else if (caharact == "aaa" | caharact == "aag" | caharact == "aar")
                    {
                        aminoacid.Append("K");
                    }
                    else if (caharact == "aac" | caharact == "aat" | caharact == "aay")
                    {
                        aminoacid.Append("N");
                    }
                    else if (caharact == "agc" | caharact == "agt" | caharact == "agy")
                    {
                        aminoacid.Append("S");
                    }
                    else if (caharact == "aga" | caharact == "agg" | caharact == "agr")
                    {
                        aminoacid.Append("R");
                    }
                    else if (caharact.Substring(0, 2) == "gt")
                    {
                        aminoacid.Append("V");
                    }
                    else if (caharact.Substring(0, 2) == "gc")
                    {
                        aminoacid.Append("A");
                    }
                    else if (caharact == "gac" | caharact == "gat" | caharact == "gay")
                    {
                        aminoacid.Append("D");
                    }
                    else if (caharact == "gaa" | caharact == "gag" | caharact == "gar")
                    {
                        aminoacid.Append("E");
                    }
                    else if (caharact.Substring(0, 2) == "gg")
                    {
                        aminoacid.Append("G");
                    }
                    else
                    {
                        aminoacid.Append("X");
                    }
                }
            }


            return aminoacid.ToString().ToUpper();

        }
      
        public static string Translate(string DNA)
        {
            int Length = DNA.Length - 1;
            int lop = 0;
            System.Text.StringBuilder aminoacid = new System.Text.StringBuilder();
            string caharact = null;
            DNA = DNA.ToLower();

            for (lop = 0; lop <= Length; lop += 3)
            {
                if (lop < Length - 2)
                {
                    caharact = DNA.Substring(lop, 3);

                    if (caharact == "ttc" | caharact == "ttt" | caharact == "tty")
                    {
                        aminoacid.Append("f  ");
                    }
                    else if (caharact == "tta" | caharact == "ttg" | caharact == "ttr")
                    {
                        aminoacid.Append("l  ");
                    }
                    else if (caharact.Substring(0, 2) == "tc")
                    {
                        aminoacid.Append("s  ");
                    }
                    else if (caharact == "tac" | caharact == "tat" | caharact == "tay")
                    {
                        aminoacid.Append("y  ");
                    }
                    else if (caharact == "taa" | caharact == "tag" | caharact == "tar")
                    {
                        aminoacid.Append("*  ");
                    }
                    else if (caharact == "tgc" | caharact == "tgt" | caharact == "tgy")
                    {
                        aminoacid.Append("c  ");
                    }
                    else if (caharact == "tga")
                    {
                        aminoacid.Append("*  ");
                    }
                    else if (caharact == "tgg")
                    {
                        aminoacid.Append("w  ");
                    }
                    else if (caharact.Substring(0, 2) == "ct")
                    {
                        aminoacid.Append("l  ");
                    }
                    else if (caharact.Substring(0, 2) == "cc")
                    {
                        aminoacid.Append("p  ");
                    }
                    else if (caharact.Substring(0, 2) == "cg")
                    {
                        aminoacid.Append("r  ");
                    }
                    else if (caharact == "caa" | caharact == "cag" | caharact == "car")
                    {
                        aminoacid.Append("q  ");
                    }
                    else if (caharact == "cac" | caharact == "cat" | caharact == "cay")
                    {
                        aminoacid.Append("h  ");
                    }
                    else if (caharact.Substring(0, 2) == "at")
                    {
                        if (caharact == "atg")
                        {
                            aminoacid.Append("m  ");
                        }
                        else
                        {
                            aminoacid.Append("i  ");
                        }
                    }
                    else if (caharact.Substring(0, 2) == "ac")
                    {
                        aminoacid.Append("t  ");
                    }
                    else if (caharact == "aaa" | caharact == "aag" | caharact == "aar")
                    {
                        aminoacid.Append("k  ");
                    }
                    else if (caharact == "aac" | caharact == "aat" | caharact == "aay")
                    {
                        aminoacid.Append("n  ");
                    }
                    else if (caharact == "agc" | caharact == "agt" | caharact == "agy")
                    {
                        aminoacid.Append("s  ");
                    }
                    else if (caharact == "aga" | caharact == "agg" | caharact == "agr")
                    {
                        aminoacid.Append("r  ");
                    }
                    else if (caharact.Substring(0, 2) == "gt")
                    {
                        aminoacid.Append("v  ");
                    }
                    else if (caharact.Substring(0, 2) == "gc")
                    {
                        aminoacid.Append("a  ");
                    }
                    else if (caharact == "gac" | caharact == "gat" | caharact == "gay")
                    {
                        aminoacid.Append("d  ");
                    }
                    else if (caharact == "gaa" | caharact == "gag" | caharact == "gar")
                    {
                        aminoacid.Append("e  ");
                    }
                    else if (caharact.Substring(0, 2) == "gg")
                    {
                        aminoacid.Append("g  ");
                    }
                    else
                    {
                        aminoacid.Append("x  ");
                    }
                }
            }


            return aminoacid.ToString().ToUpper();

        }

        static internal ArrayList ListOfChromosomes(string FileContents)
        {
            string[] Lines = FileContents.Split(Convert.ToChar(Constants.vbCr));
            string item = null;
            ArrayList c = new ArrayList();

            for (int index = 0; index <= Lines.GetUpperBound(0); index++)
            {
                item = Lines[index].Trim();
                if (!string.IsNullOrEmpty(item))
                {
                    try
                    {
                        int chromosome = Convert.ToInt32(item);
                        if (chromosome > 0 & chromosome < 26)
                            c.Add(item);

                    }
                    catch (Exception ex)
                    {
                    }

                }
            }

            c.Sort();

            return c;

        }

        static internal ArrayList ListOfGeneNames(string FileContents)
        {
            string[] Lines = FileContents.Split(Convert.ToChar(Constants.vbCr));
            string item = null;
            ArrayList c = new ArrayList();

            for (int index = 0; index <= Lines.GetUpperBound(0); index++)
            {
                item = Lines[index].Trim().ToLower();
                if (!string.IsNullOrEmpty(item))
                {

                    try
                    {
                        c.Add(item);

                    }
                    catch (Exception ex)
                    {
                    }

                }
            }

            return c;

        }

        static internal ArrayList ListOfGeneRegions(string FileContents)
        {
            string[] Lines = FileContents.Split(Convert.ToChar(Constants.vbCr));
            string data = null;
            DNARegion item = default(DNARegion);
            ArrayList c = new ArrayList();

            for (int index = 0; index <= Lines.GetUpperBound(0); index++)
            {
                data = Lines[index].Trim();
                if (!string.IsNullOrEmpty(data))
                {
                    try
                    {
                        item = new DNARegion(data);
                        c.Add(item);
                        item = null;

                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            c.Sort(new DNARegionSort());

            return c;

        }

        static internal byte[] StringToBytes(string Value)
        {

            byte[] OutPut = new byte[Value.Length];
            for (int i = 0; i <= OutPut.GetUpperBound(0); i++)
            {
                OutPut[i] = Convert.ToByte(Value[i]);
            }

            return OutPut;

        }

        static internal string BytesToString(byte[] TheValue)
        {
            System.Text.StringBuilder value = new System.Text.StringBuilder();

            for (int i = 0; i <= TheValue.GetUpperBound(0); i++)
            {
                value.Append(Convert.ToChar(TheValue[i]));
            }

            return value.ToString();

        }

        static internal int GetChromosomeIndex(string FileName)
        {
            if (FileName.LastIndexOf(".") > -1)
                FileName = FileName.Substring(0, FileName.LastIndexOf("."));
            if (FileName.Length > 3)
                FileName = FileName.Substring(3).ToLower();
            
            try
            {
                if (char.IsDigit(FileName[0]) == true)
                {
                    return Convert.ToInt32(FileName);
                }
                else if (FileName == "x")
                {
                    return 23;
                }
                else if (FileName == "y")
                {
                    return 24;
                }
                else if (FileName == "mt")
                {
                    return 26;
                }
                else
                { return Convert.ToInt32(FileName); }
            }
            catch (Exception ex)
            {
                return -1;
            }


        }

        static internal int GetPhred(char Score)
        {

            try
            {
                int value = Convert.ToInt32(Score) - 33;

                return Convert.ToInt32(10 * Math.Log10(1 + Math.Pow(10, (value / 10.0))) / 1);

            }
            catch (Exception ex)
            {
                return int.MinValue;
            }

        }

        static internal string ReverseComplement(string Original)
        {
            char[] final = Original.ToCharArray();
            Original = null;
            Array.Reverse(final);

            for (int i = 0; i <= final.GetUpperBound(0); i++)
            {
                switch (final[i])
                {
                    case 'a':
                    case 'A':
                        final[i] = 'T';
                        break;
                    case 'c':
                    case 'C':
                        final[i] = 'G';
                        break;
                    case 'g':
                    case 'G':
                        final[i] = 'C';
                        break;
                    case 't':
                    case 'T':
                        final[i] = 'A';
                        break;
                    case ' ':
                        final[i] = ' ';
                        break;
                    case '-':
                        final[i] = '-';
                        break;
                    default:
                        final[i] = 'N';
                        break;
                }
            }


            return new string(final);

        }

        static internal string ReverseQualityString(string Original)
        {
            char[] final = Original.ToCharArray();

            Array.Reverse(final);

            return string.Concat(final);

        }
    }
}

