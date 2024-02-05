using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
static class Constants
{

    public const int IndexFile = 0;
    public const int IndexFlag = 1;
    public const int IndexChromosomeNumber = 2;
    public const int IndexPosition = 3;
    public const int IndexCIGAR = 5;
    public const int IndexRead = 9;

    public const int IndexQuality = 10;
    //Constants for aggregating the data
    public const int Readlength = 100;

    public const int PhredCutoff = 20;
    //Constants for CCDS
    public const int Flanking = 50;
    public const int StartOfORF = Flanking;

    public const int EndOfORF = (Flanking * 2) + 1;
    public static char[] CIGARFlags = { 'M', 'I', 'D', 'N', 'S', 'H', 'P', '=', 'X' };
    public const char vbCr = '\r';
    public const char vbLf = '\n';
    public const string vbCrLf = "\r\n";
    public const string vbTab = "\t";

    public const int MoreThan = 62;
}

