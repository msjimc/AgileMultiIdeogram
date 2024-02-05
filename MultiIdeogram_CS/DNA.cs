
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace  MultiIdeogram_CS
{
    static class DNA
    {
        static internal System.Text.StringBuilder ChromosomeAsSequence;

        static internal byte[] ChromosomeAsBinary;
        static internal int GetSequenceData(string FileName, long Filelength)
        {

            System.IO.StreamReader FileData = null;

            try
            {
                System.IO.FileInfo fl = new System.IO.FileInfo(FileName);

                ChromosomeAsBinary = null;
                GC.Collect();
                ChromosomeAsBinary = new byte[Convert.ToInt32(Filelength) + 1];

                int index = 0;
                int Value = 0;
                FileData = new System.IO.StreamReader(FileName);
                if ((char)FileData.Peek() == '>')
                    FileData.ReadLine();

                while (FileData.Peek() > 0)
                {
                    Value = FileData.Read();
                    if (char.IsLetter((char)(Value)) == true)
                    {
                        if (Value > 96)
                            Value -= 32;
                        ChromosomeAsBinary[index] = Convert.ToByte(Value);
                        index += 1;
                    }
                }

                return 1;

            }
            catch 
            {
                return -1;
            }
            finally
            {
                try
                {
                    FileData.Close();

                }
                catch (Exception ex)
                {
                }
            }

        }

        static internal int GetSequenceData(string FileName)
        {

            System.IO.StreamReader FileData = null;

            try
            {
                System.IO.FileInfo fl = new System.IO.FileInfo(FileName);
                ChromosomeAsSequence = null;
                GC.Collect();
                ChromosomeAsSequence = new System.Text.StringBuilder();
                ChromosomeAsSequence.EnsureCapacity(Convert.ToInt32(fl.Length));

                FileData = new System.IO.StreamReader(FileName);
                if (FileData.Peek() == Constants.MoreThan)
                    FileData.ReadLine();

                while (FileData.Peek() > 0)
                {
                    ChromosomeAsSequence.Append(FileData.ReadLine());
                }

                return 1;

            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                try
                {
                    FileData.Close();

                }
                catch (Exception ex)
                {
                }
            }

        }

        static internal int GetSequenceData(System.IO.StreamReader FileData, string FileName)
        {
            try
            {
                System.IO.FileInfo fl = new System.IO.FileInfo(FileName);
                ChromosomeAsSequence = null;
                GC.Collect();
                ChromosomeAsSequence = new System.Text.StringBuilder();
                ChromosomeAsSequence.EnsureCapacity(Convert.ToInt32(fl.Length));

                if (FileData.Peek() == Constants.MoreThan)
                    FileData.ReadLine();

                while (FileData.Peek() > 0)
                {
                    ChromosomeAsSequence.Append(FileData.ReadLine());
                }

                return 1;

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
                return 0;
            }

        }

        static internal int GetPhred(byte Score)
        {

            try
            {
                int value = Score;                
                return Convert.ToInt32(10 * Math.Log10(1 + Math.Pow(10, (value / 10.0))) / 1);

            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        static internal string ReverseComplement(string Original)
        {
            char[] final = Original.ToCharArray();

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
                    case '-':
                    case 's':
                    case 'W':
                    case 'S':
                    case '|':
                        break;
                    case 'r':
                    case 'R':
                        final[i] = 'Y';
                        break;
                    case 'Y':
                    case 'y':
                        final[i] = 'R';
                        break;
                    case 'k':
                    case 'K':
                        final[i] = 'M';
                        break;
                    case 'm':
                    case 'M':
                        final[i] = 'K';
                        break;
                    default:
                        final[i] = 'N';
                        break;
                }
            }

            return string.Concat(final);

        }
    }
}
 