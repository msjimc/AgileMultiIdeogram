using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiIdeogram_CS
{
    public partial class SingleView : Form
    {
        private int[] chromosomelength = { 249250621, 243199373, 198022430, 191154276, 180915260, 171115067, 159138663, 146364022, 141213431, 135534747, 135006516, 133851895, 115169878, 107349540, 102531392, 90354753, 81195210, 78077248, 59128983, 63025520, 48129895, 51304566, 155270560, 59373566 };

        private int fatness = 1;
        private int gap = 1;

        private Bitmap gbtm = null;
        private DNARegion Gene = null;
        private CytoGenetic[][] CytoBands = null;
        private string[] Files = null;
        private string MultiText = null;
        private string[] FileNames = null;
        private bool[] Affecteds = null;
        private List<DNARegion>[] SNPRegions = null;
        private List<DNARegion> MinimumSNPRegions = null;
        private bool showDashedLine = true;
        private bool scaleToChromosomeOne = false;
        private Color AffectedColour = Color.Blue;
        private Color UnaffectedColour = Color.Red;
        private Color CommonRegionsCommon = Color.LightBlue;
        private enum ExportThis { Long, Short, Common }

        private int currentChromosome = 1;

        public SingleView(string[] theFiles, string[] theFileNames, bool[] theAffecteds, string multiText)
        {
            InitializeComponent();

            MultiText = multiText;
            Files = theFiles;
            FileNames = theFileNames;
            Affecteds = theAffecteds;
            SNPRegions = new List<DNARegion>[Files.Length];
        }
        
        private void SingleView_Load(object sender, EventArgs e)
        {
            CytoBands = new CytoGenetic[26][];
            GetBands();
            GetCytogeneticData();
            PlaceImage(Draw(1), true);
        }

        public void GetData(bool IgnoreRSField, bool VCFGenotypes)
        {
            getRegions(IgnoreRSField, VCFGenotypes);
            MinimumSNPRegions = CreateMinimumArray(SNPRegions, Affecteds);
            cboChromosome.SelectedIndex = 0;
        }

        private Dictionary<string, List<DNARegion>> getRegionsFromMultiTextFile()
        {
            Dictionary<string, List<DNARegion>> data = new Dictionary<string, List<DNARegion>>();

            System.IO.StreamReader fr = null;
            try
            {
                fr = new System.IO.StreamReader(MultiText);
                string line = null;
                string key = null;
                List<DNARegion> regions = new List<DNARegion>();
                while (fr.Peek() > 0)
                {
                    line = fr.ReadLine();
                    if (line.Contains(":") != true && line.Contains("\t") != true)
                    {
                        if (string.IsNullOrEmpty(key) == false)
                        {
                            data.Add(key, regions);
                            regions = new List<DNARegion>();
                        }
                        key = line.Trim();
                    }
                    else
                    {
                        string[] items = line.Split('\t');
                        if (items.Length > 3 && line.ToLower().StartsWith("chro") == false)
                        {
                            
                            try
                            {
                                string chrStr = items[0].ToLower().Replace("chr", "");
                                DNARegion dr = new DNARegion(Convert.ToInt32(chrStr), Convert.ToInt32(items[1]), Convert.ToInt32(items[2]));
                                regions.Add(dr);
                            }
                            catch { }
                        }
                    }
                }
                if (string.IsNullOrEmpty(key) == false)
                {
                    data.Add(key, regions);
                }

                fr.Close();

            }
            finally
            { if (fr != null) { fr.Close(); } }

            return data;
        }

        private List<DNARegion> CreateMinimumArray(List<DNARegion>[] intervals, bool[] affected)
        {
            List<DNARegion> answer = null;
            if (intervals.Length == 1)
            {
                answer = intervals[0];
            }
            else
            {
                answer = new List<DNARegion>();
                DNARegion thisRegion = null;
                bool stopNow = false;
                bool addThisRegion = false;
                int count = 0;
                foreach (bool b in affected)
                { if (b == true) { count++; } }
                int[] indexes = new int[count];
                int[] place = new int[count];

                count = 0;
                for (int index = 0; index < affected.Length; index++)
                {
                    if (affected[index] == true)
                    { indexes[count++] = index; }
                }
                if (count == 0) { stopNow = true; }

                while (stopNow == false)
                {
                    thisRegion = new DNARegion(intervals[indexes[0]][place[0]]);
                    int index = 0;

                    addThisRegion = true;
                    index = 1;
                    while (addThisRegion == true && index < indexes.Length)
                    {

                        if (intervals[indexes[index]][place[index]].Chromosome == thisRegion.Chromosome)
                        {
                            if (intervals[indexes[index]][place[index]].StartPoint > thisRegion.EndPoint ||
                                thisRegion.StartPoint >= intervals[indexes[index]][place[index]].EndPoint)
                            {
                                if (intervals[indexes[index]][place[index]].StartPoint > thisRegion.EndPoint)
                                {
                                    place[0]++;
                                    addThisRegion = false;
                                }
                                else if (thisRegion.StartPoint >= intervals[indexes[index]][place[index]].EndPoint)
                                {
                                    place[index]++;
                                    addThisRegion = false;
                                }
                            }
                            else
                            {
                                if (thisRegion.StartPoint < intervals[indexes[index]][place[index]].StartPoint)
                                { thisRegion.adjustStart(intervals[indexes[index]][place[index]].StartPoint); }
                                if (thisRegion.EndPoint > intervals[indexes[index]][place[index]].EndPoint)
                                { thisRegion.AdjustEnd(intervals[indexes[index]][place[index]].EndPoint); }
                            }
                        }
                        else if (intervals[indexes[index]][place[index]].Chromosome < thisRegion.Chromosome)
                        {
                            place[index]++;
                            addThisRegion = false;
                        }
                        else if (intervals[indexes[index]][place[index]].Chromosome > thisRegion.Chromosome)
                        {
                            place[0]++;
                            addThisRegion = false;
                        }

                        index++;
                    }

                    if (addThisRegion == true)
                    {
                        int smallest = int.MaxValue;
                        int thisPlace = 0;
                        answer.Add(thisRegion);

                        for (int test = 0; test < indexes.Length; test++)
                        {
                            if (intervals[indexes[test]][place[test]].EndPoint < smallest)
                            {
                                thisPlace = test;
                                smallest = intervals[indexes[test]][place[test]].EndPoint;
                            }
                        }

                        place[thisPlace]++;
                    }

                    for (int test = 0; test < indexes.Length; test++)
                    { if (place[test] >= intervals[indexes[test]].Count) { stopNow = true; } }

                }

            }

            return answer;

        }

        private List<string> StandardCytoBands = new List<string>();

        private void GetBands()
        {
            StandardCytoBands.Add("chr1" + '\t' + "0" + '\t' + "2300000" + '\t' + "p36.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "2300000" + '\t' + "5400000" + '\t' + "p36.32" + '\t' + "gpos25");
            StandardCytoBands.Add("chr1" + '\t' + "5400000" + '\t' + "7200000" + '\t' + "p36.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "7200000" + '\t' + "9200000" + '\t' + "p36.23" + '\t' + "gpos25");
            StandardCytoBands.Add("chr1" + '\t' + "9200000" + '\t' + "12700000" + '\t' + "p36.22" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "12700000" + '\t' + "16200000" + '\t' + "p36.21" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "16200000" + '\t' + "20400000" + '\t' + "p36.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "20400000" + '\t' + "23900000" + '\t' + "p36.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr1" + '\t' + "23900000" + '\t' + "28000000" + '\t' + "p36.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "28000000" + '\t' + "30200000" + '\t' + "p35.3" + '\t' + "gpos25");
            StandardCytoBands.Add("chr1" + '\t' + "30200000" + '\t' + "32400000" + '\t' + "p35.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "32400000" + '\t' + "34600000" + '\t' + "p35.1" + '\t' + "gpos25");
            StandardCytoBands.Add("chr1" + '\t' + "34600000" + '\t' + "40100000" + '\t' + "p34.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "40100000" + '\t' + "44100000" + '\t' + "p34.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr1" + '\t' + "44100000" + '\t' + "46800000" + '\t' + "p34.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "46800000" + '\t' + "50700000" + '\t' + "p33" + '\t' + "gpos75");
            StandardCytoBands.Add("chr1" + '\t' + "50700000" + '\t' + "56100000" + '\t' + "p32.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "56100000" + '\t' + "59000000" + '\t' + "p32.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "59000000" + '\t' + "61300000" + '\t' + "p32.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "61300000" + '\t' + "68900000" + '\t' + "p31.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "68900000" + '\t' + "69700000" + '\t' + "p31.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "69700000" + '\t' + "84900000" + '\t' + "p31.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr1" + '\t' + "84900000" + '\t' + "88400000" + '\t' + "p22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "88400000" + '\t' + "92000000" + '\t' + "p22.2" + '\t' + "gpos75");
            StandardCytoBands.Add("chr1" + '\t' + "92000000" + '\t' + "94700000" + '\t' + "p22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "94700000" + '\t' + "99700000" + '\t' + "p21.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr1" + '\t' + "99700000" + '\t' + "102200000" + '\t' + "p21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "102200000" + '\t' + "107200000" + '\t' + "p21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr1" + '\t' + "107200000" + '\t' + "111800000" + '\t' + "p13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "111800000" + '\t' + "116100000" + '\t' + "p13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "116100000" + '\t' + "117800000" + '\t' + "p13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "117800000" + '\t' + "120600000" + '\t' + "p12" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "120600000" + '\t' + "121500000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "121500000" + '\t' + "125000000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr1" + '\t' + "125000000" + '\t' + "128900000" + '\t' + "q11" + '\t' + "acen");
            StandardCytoBands.Add("chr1" + '\t' + "128900000" + '\t' + "142600000" + '\t' + "q12" + '\t' + "gvar");
            StandardCytoBands.Add("chr1" + '\t' + "142600000" + '\t' + "147000000" + '\t' + "q21.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "147000000" + '\t' + "150300000" + '\t' + "q21.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "150300000" + '\t' + "155000000" + '\t' + "q21.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "155000000" + '\t' + "156500000" + '\t' + "q22" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "156500000" + '\t' + "159100000" + '\t' + "q23.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "159100000" + '\t' + "160500000" + '\t' + "q23.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "160500000" + '\t' + "165500000" + '\t' + "q23.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "165500000" + '\t' + "167200000" + '\t' + "q24.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "167200000" + '\t' + "170900000" + '\t' + "q24.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "170900000" + '\t' + "172900000" + '\t' + "q24.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr1" + '\t' + "172900000" + '\t' + "176000000" + '\t' + "q25.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "176000000" + '\t' + "180300000" + '\t' + "q25.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "180300000" + '\t' + "185800000" + '\t' + "q25.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "185800000" + '\t' + "190800000" + '\t' + "q31.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr1" + '\t' + "190800000" + '\t' + "193800000" + '\t' + "q31.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "193800000" + '\t' + "198700000" + '\t' + "q31.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr1" + '\t' + "198700000" + '\t' + "207200000" + '\t' + "q32.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "207200000" + '\t' + "211500000" + '\t' + "q32.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr1" + '\t' + "211500000" + '\t' + "214500000" + '\t' + "q32.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "214500000" + '\t' + "224100000" + '\t' + "q41" + '\t' + "gpos100");
            StandardCytoBands.Add("chr1" + '\t' + "224100000" + '\t' + "224600000" + '\t' + "q42.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "224600000" + '\t' + "227000000" + '\t' + "q42.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr1" + '\t' + "227000000" + '\t' + "230700000" + '\t' + "q42.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "230700000" + '\t' + "234700000" + '\t' + "q42.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr1" + '\t' + "234700000" + '\t' + "236600000" + '\t' + "q42.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr1" + '\t' + "236600000" + '\t' + "243700000" + '\t' + "q43" + '\t' + "gpos75");
            StandardCytoBands.Add("chr1" + '\t' + "243700000" + '\t' + "249250621" + '\t' + "q44" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "0" + '\t' + "3000000" + '\t' + "p15.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "3000000" + '\t' + "3800000" + '\t' + "p15.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr10" + '\t' + "3800000" + '\t' + "6600000" + '\t' + "p15.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "6600000" + '\t' + "12200000" + '\t' + "p14" + '\t' + "gpos75");
            StandardCytoBands.Add("chr10" + '\t' + "12200000" + '\t' + "17300000" + '\t' + "p13" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "17300000" + '\t' + "18600000" + '\t' + "p12.33" + '\t' + "gpos75");
            StandardCytoBands.Add("chr10" + '\t' + "18600000" + '\t' + "18700000" + '\t' + "p12.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "18700000" + '\t' + "22600000" + '\t' + "p12.31" + '\t' + "gpos75");
            StandardCytoBands.Add("chr10" + '\t' + "22600000" + '\t' + "24600000" + '\t' + "p12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "24600000" + '\t' + "29600000" + '\t' + "p12.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr10" + '\t' + "29600000" + '\t' + "31300000" + '\t' + "p11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "31300000" + '\t' + "34400000" + '\t' + "p11.22" + '\t' + "gpos25");
            StandardCytoBands.Add("chr10" + '\t' + "34400000" + '\t' + "38000000" + '\t' + "p11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "38000000" + '\t' + "40200000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr10" + '\t' + "40200000" + '\t' + "42300000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr10" + '\t' + "42300000" + '\t' + "46100000" + '\t' + "q11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "46100000" + '\t' + "49900000" + '\t' + "q11.22" + '\t' + "gpos25");
            StandardCytoBands.Add("chr10" + '\t' + "49900000" + '\t' + "52900000" + '\t' + "q11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "52900000" + '\t' + "61200000" + '\t' + "q21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr10" + '\t' + "61200000" + '\t' + "64500000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "64500000" + '\t' + "70600000" + '\t' + "q21.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr10" + '\t' + "70600000" + '\t' + "74900000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "74900000" + '\t' + "77700000" + '\t' + "q22.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr10" + '\t' + "77700000" + '\t' + "82000000" + '\t' + "q22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "82000000" + '\t' + "87900000" + '\t' + "q23.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr10" + '\t' + "87900000" + '\t' + "89500000" + '\t' + "q23.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "89500000" + '\t' + "92900000" + '\t' + "q23.31" + '\t' + "gpos75");
            StandardCytoBands.Add("chr10" + '\t' + "92900000" + '\t' + "94100000" + '\t' + "q23.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "94100000" + '\t' + "97000000" + '\t' + "q23.33" + '\t' + "gpos50");
            StandardCytoBands.Add("chr10" + '\t' + "97000000" + '\t' + "99300000" + '\t' + "q24.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "99300000" + '\t' + "101900000" + '\t' + "q24.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr10" + '\t' + "101900000" + '\t' + "103000000" + '\t' + "q24.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "103000000" + '\t' + "104900000" + '\t' + "q24.32" + '\t' + "gpos25");
            StandardCytoBands.Add("chr10" + '\t' + "104900000" + '\t' + "105800000" + '\t' + "q24.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "105800000" + '\t' + "111900000" + '\t' + "q25.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr10" + '\t' + "111900000" + '\t' + "114900000" + '\t' + "q25.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "114900000" + '\t' + "119100000" + '\t' + "q25.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr10" + '\t' + "119100000" + '\t' + "121700000" + '\t' + "q26.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "121700000" + '\t' + "123100000" + '\t' + "q26.12" + '\t' + "gpos50");
            StandardCytoBands.Add("chr10" + '\t' + "123100000" + '\t' + "127500000" + '\t' + "q26.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr10" + '\t' + "127500000" + '\t' + "130600000" + '\t' + "q26.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr10" + '\t' + "130600000" + '\t' + "135534747" + '\t' + "q26.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "0" + '\t' + "2800000" + '\t' + "p15.5" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "2800000" + '\t' + "10700000" + '\t' + "p15.4" + '\t' + "gpos50");
            StandardCytoBands.Add("chr11" + '\t' + "10700000" + '\t' + "12700000" + '\t' + "p15.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "12700000" + '\t' + "16200000" + '\t' + "p15.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr11" + '\t' + "16200000" + '\t' + "21700000" + '\t' + "p15.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "21700000" + '\t' + "26100000" + '\t' + "p14.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr11" + '\t' + "26100000" + '\t' + "27200000" + '\t' + "p14.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "27200000" + '\t' + "31000000" + '\t' + "p14.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr11" + '\t' + "31000000" + '\t' + "36400000" + '\t' + "p13" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "36400000" + '\t' + "43500000" + '\t' + "p12" + '\t' + "gpos100");
            StandardCytoBands.Add("chr11" + '\t' + "43500000" + '\t' + "48800000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "48800000" + '\t' + "51600000" + '\t' + "p11.12" + '\t' + "gpos75");
            StandardCytoBands.Add("chr11" + '\t' + "51600000" + '\t' + "53700000" + '\t' + "p11.11" + '\t' + "acen");
            StandardCytoBands.Add("chr11" + '\t' + "53700000" + '\t' + "55700000" + '\t' + "q11" + '\t' + "acen");
            StandardCytoBands.Add("chr11" + '\t' + "55700000" + '\t' + "59900000" + '\t' + "q12.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr11" + '\t' + "59900000" + '\t' + "61700000" + '\t' + "q12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "61700000" + '\t' + "63400000" + '\t' + "q12.3" + '\t' + "gpos25");
            StandardCytoBands.Add("chr11" + '\t' + "63400000" + '\t' + "65900000" + '\t' + "q13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "65900000" + '\t' + "68400000" + '\t' + "q13.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr11" + '\t' + "68400000" + '\t' + "70400000" + '\t' + "q13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "70400000" + '\t' + "75200000" + '\t' + "q13.4" + '\t' + "gpos50");
            StandardCytoBands.Add("chr11" + '\t' + "75200000" + '\t' + "77100000" + '\t' + "q13.5" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "77100000" + '\t' + "85600000" + '\t' + "q14.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr11" + '\t' + "85600000" + '\t' + "88300000" + '\t' + "q14.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "88300000" + '\t' + "92800000" + '\t' + "q14.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr11" + '\t' + "92800000" + '\t' + "97200000" + '\t' + "q21" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "97200000" + '\t' + "102100000" + '\t' + "q22.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr11" + '\t' + "102100000" + '\t' + "102900000" + '\t' + "q22.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "102900000" + '\t' + "110400000" + '\t' + "q22.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr11" + '\t' + "110400000" + '\t' + "112500000" + '\t' + "q23.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "112500000" + '\t' + "114500000" + '\t' + "q23.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr11" + '\t' + "114500000" + '\t' + "121200000" + '\t' + "q23.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "121200000" + '\t' + "123900000" + '\t' + "q24.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr11" + '\t' + "123900000" + '\t' + "127800000" + '\t' + "q24.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr11" + '\t' + "127800000" + '\t' + "130800000" + '\t' + "q24.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr11" + '\t' + "130800000" + '\t' + "135006516" + '\t' + "q25" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "0" + '\t' + "3300000" + '\t' + "p13.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "3300000" + '\t' + "5400000" + '\t' + "p13.32" + '\t' + "gpos25");
            StandardCytoBands.Add("chr12" + '\t' + "5400000" + '\t' + "10100000" + '\t' + "p13.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "10100000" + '\t' + "12800000" + '\t' + "p13.2" + '\t' + "gpos75");
            StandardCytoBands.Add("chr12" + '\t' + "12800000" + '\t' + "14800000" + '\t' + "p13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "14800000" + '\t' + "20000000" + '\t' + "p12.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr12" + '\t' + "20000000" + '\t' + "21300000" + '\t' + "p12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "21300000" + '\t' + "26500000" + '\t' + "p12.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr12" + '\t' + "26500000" + '\t' + "27800000" + '\t' + "p11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "27800000" + '\t' + "30700000" + '\t' + "p11.22" + '\t' + "gpos50");
            StandardCytoBands.Add("chr12" + '\t' + "30700000" + '\t' + "33300000" + '\t' + "p11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "33300000" + '\t' + "35800000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr12" + '\t' + "35800000" + '\t' + "38200000" + '\t' + "q11" + '\t' + "acen");
            StandardCytoBands.Add("chr12" + '\t' + "38200000" + '\t' + "46400000" + '\t' + "q12" + '\t' + "gpos100");
            StandardCytoBands.Add("chr12" + '\t' + "46400000" + '\t' + "49100000" + '\t' + "q13.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "49100000" + '\t' + "51500000" + '\t' + "q13.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr12" + '\t' + "51500000" + '\t' + "54900000" + '\t' + "q13.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "54900000" + '\t' + "56600000" + '\t' + "q13.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr12" + '\t' + "56600000" + '\t' + "58100000" + '\t' + "q13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "58100000" + '\t' + "63100000" + '\t' + "q14.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr12" + '\t' + "63100000" + '\t' + "65100000" + '\t' + "q14.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "65100000" + '\t' + "67700000" + '\t' + "q14.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr12" + '\t' + "67700000" + '\t' + "71500000" + '\t' + "q15" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "71500000" + '\t' + "75700000" + '\t' + "q21.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr12" + '\t' + "75700000" + '\t' + "80300000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "80300000" + '\t' + "86700000" + '\t' + "q21.31" + '\t' + "gpos100");
            StandardCytoBands.Add("chr12" + '\t' + "86700000" + '\t' + "89000000" + '\t' + "q21.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "89000000" + '\t' + "92600000" + '\t' + "q21.33" + '\t' + "gpos100");
            StandardCytoBands.Add("chr12" + '\t' + "92600000" + '\t' + "96200000" + '\t' + "q22" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "96200000" + '\t' + "101600000" + '\t' + "q23.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr12" + '\t' + "101600000" + '\t' + "103800000" + '\t' + "q23.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "103800000" + '\t' + "109000000" + '\t' + "q23.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr12" + '\t' + "109000000" + '\t' + "111700000" + '\t' + "q24.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "111700000" + '\t' + "112300000" + '\t' + "q24.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr12" + '\t' + "112300000" + '\t' + "114300000" + '\t' + "q24.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "114300000" + '\t' + "116800000" + '\t' + "q24.21" + '\t' + "gpos50");
            StandardCytoBands.Add("chr12" + '\t' + "116800000" + '\t' + "118100000" + '\t' + "q24.22" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "118100000" + '\t' + "120700000" + '\t' + "q24.23" + '\t' + "gpos50");
            StandardCytoBands.Add("chr12" + '\t' + "120700000" + '\t' + "125900000" + '\t' + "q24.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr12" + '\t' + "125900000" + '\t' + "129300000" + '\t' + "q24.32" + '\t' + "gpos50");
            StandardCytoBands.Add("chr12" + '\t' + "129300000" + '\t' + "133851895" + '\t' + "q24.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "0" + '\t' + "4500000" + '\t' + "p13" + '\t' + "gvar");
            StandardCytoBands.Add("chr13" + '\t' + "4500000" + '\t' + "10000000" + '\t' + "p12" + '\t' + "stalk");
            StandardCytoBands.Add("chr13" + '\t' + "10000000" + '\t' + "16300000" + '\t' + "p11.2" + '\t' + "gvar");
            StandardCytoBands.Add("chr13" + '\t' + "16300000" + '\t' + "17900000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr13" + '\t' + "17900000" + '\t' + "19500000" + '\t' + "q11" + '\t' + "acen");
            StandardCytoBands.Add("chr13" + '\t' + "19500000" + '\t' + "23300000" + '\t' + "q12.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "23300000" + '\t' + "25500000" + '\t' + "q12.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr13" + '\t' + "25500000" + '\t' + "27800000" + '\t' + "q12.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "27800000" + '\t' + "28900000" + '\t' + "q12.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr13" + '\t' + "28900000" + '\t' + "32200000" + '\t' + "q12.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "32200000" + '\t' + "34000000" + '\t' + "q13.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr13" + '\t' + "34000000" + '\t' + "35500000" + '\t' + "q13.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "35500000" + '\t' + "40100000" + '\t' + "q13.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr13" + '\t' + "40100000" + '\t' + "45200000" + '\t' + "q14.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "45200000" + '\t' + "45800000" + '\t' + "q14.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr13" + '\t' + "45800000" + '\t' + "47300000" + '\t' + "q14.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "47300000" + '\t' + "50900000" + '\t' + "q14.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr13" + '\t' + "50900000" + '\t' + "55300000" + '\t' + "q14.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "55300000" + '\t' + "59600000" + '\t' + "q21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr13" + '\t' + "59600000" + '\t' + "62300000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "62300000" + '\t' + "65700000" + '\t' + "q21.31" + '\t' + "gpos75");
            StandardCytoBands.Add("chr13" + '\t' + "65700000" + '\t' + "68600000" + '\t' + "q21.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "68600000" + '\t' + "73300000" + '\t' + "q21.33" + '\t' + "gpos100");
            StandardCytoBands.Add("chr13" + '\t' + "73300000" + '\t' + "75400000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "75400000" + '\t' + "77200000" + '\t' + "q22.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr13" + '\t' + "77200000" + '\t' + "79000000" + '\t' + "q22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "79000000" + '\t' + "87700000" + '\t' + "q31.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr13" + '\t' + "87700000" + '\t' + "90000000" + '\t' + "q31.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "90000000" + '\t' + "95000000" + '\t' + "q31.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr13" + '\t' + "95000000" + '\t' + "98200000" + '\t' + "q32.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "98200000" + '\t' + "99300000" + '\t' + "q32.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr13" + '\t' + "99300000" + '\t' + "101700000" + '\t' + "q32.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "101700000" + '\t' + "104800000" + '\t' + "q33.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr13" + '\t' + "104800000" + '\t' + "107000000" + '\t' + "q33.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr13" + '\t' + "107000000" + '\t' + "110300000" + '\t' + "q33.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr13" + '\t' + "110300000" + '\t' + "115169878" + '\t' + "q34" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "0" + '\t' + "3700000" + '\t' + "p13" + '\t' + "gvar");
            StandardCytoBands.Add("chr14" + '\t' + "3700000" + '\t' + "8100000" + '\t' + "p12" + '\t' + "stalk");
            StandardCytoBands.Add("chr14" + '\t' + "8100000" + '\t' + "16100000" + '\t' + "p11.2" + '\t' + "gvar");
            StandardCytoBands.Add("chr14" + '\t' + "16100000" + '\t' + "17600000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr14" + '\t' + "17600000" + '\t' + "19100000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr14" + '\t' + "19100000" + '\t' + "24600000" + '\t' + "q11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "24600000" + '\t' + "33300000" + '\t' + "q12" + '\t' + "gpos100");
            StandardCytoBands.Add("chr14" + '\t' + "33300000" + '\t' + "35300000" + '\t' + "q13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "35300000" + '\t' + "36600000" + '\t' + "q13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr14" + '\t' + "36600000" + '\t' + "37800000" + '\t' + "q13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "37800000" + '\t' + "43500000" + '\t' + "q21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr14" + '\t' + "43500000" + '\t' + "47200000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "47200000" + '\t' + "50900000" + '\t' + "q21.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr14" + '\t' + "50900000" + '\t' + "54100000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "54100000" + '\t' + "55500000" + '\t' + "q22.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr14" + '\t' + "55500000" + '\t' + "58100000" + '\t' + "q22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "58100000" + '\t' + "62100000" + '\t' + "q23.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr14" + '\t' + "62100000" + '\t' + "64800000" + '\t' + "q23.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "64800000" + '\t' + "67900000" + '\t' + "q23.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr14" + '\t' + "67900000" + '\t' + "70200000" + '\t' + "q24.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "70200000" + '\t' + "73800000" + '\t' + "q24.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr14" + '\t' + "73800000" + '\t' + "79300000" + '\t' + "q24.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "79300000" + '\t' + "83600000" + '\t' + "q31.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr14" + '\t' + "83600000" + '\t' + "84900000" + '\t' + "q31.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "84900000" + '\t' + "89800000" + '\t' + "q31.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr14" + '\t' + "89800000" + '\t' + "91900000" + '\t' + "q32.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "91900000" + '\t' + "94700000" + '\t' + "q32.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr14" + '\t' + "94700000" + '\t' + "96300000" + '\t' + "q32.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "96300000" + '\t' + "101400000" + '\t' + "q32.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr14" + '\t' + "101400000" + '\t' + "103200000" + '\t' + "q32.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr14" + '\t' + "103200000" + '\t' + "104000000" + '\t' + "q32.32" + '\t' + "gpos50");
            StandardCytoBands.Add("chr14" + '\t' + "104000000" + '\t' + "107349540" + '\t' + "q32.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "0" + '\t' + "3900000" + '\t' + "p13" + '\t' + "gvar");
            StandardCytoBands.Add("chr15" + '\t' + "3900000" + '\t' + "8700000" + '\t' + "p12" + '\t' + "stalk");
            StandardCytoBands.Add("chr15" + '\t' + "8700000" + '\t' + "15800000" + '\t' + "p11.2" + '\t' + "gvar");
            StandardCytoBands.Add("chr15" + '\t' + "15800000" + '\t' + "19000000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr15" + '\t' + "19000000" + '\t' + "20700000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr15" + '\t' + "20700000" + '\t' + "25700000" + '\t' + "q11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "25700000" + '\t' + "28100000" + '\t' + "q12" + '\t' + "gpos50");
            StandardCytoBands.Add("chr15" + '\t' + "28100000" + '\t' + "30300000" + '\t' + "q13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "30300000" + '\t' + "31200000" + '\t' + "q13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr15" + '\t' + "31200000" + '\t' + "33600000" + '\t' + "q13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "33600000" + '\t' + "40100000" + '\t' + "q14" + '\t' + "gpos75");
            StandardCytoBands.Add("chr15" + '\t' + "40100000" + '\t' + "42800000" + '\t' + "q15.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "42800000" + '\t' + "43600000" + '\t' + "q15.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr15" + '\t' + "43600000" + '\t' + "44800000" + '\t' + "q15.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "44800000" + '\t' + "49500000" + '\t' + "q21.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr15" + '\t' + "49500000" + '\t' + "52900000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "52900000" + '\t' + "59100000" + '\t' + "q21.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr15" + '\t' + "59100000" + '\t' + "59300000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "59300000" + '\t' + "63700000" + '\t' + "q22.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr15" + '\t' + "63700000" + '\t' + "67200000" + '\t' + "q22.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "67200000" + '\t' + "67300000" + '\t' + "q22.32" + '\t' + "gpos25");
            StandardCytoBands.Add("chr15" + '\t' + "67300000" + '\t' + "67500000" + '\t' + "q22.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "67500000" + '\t' + "72700000" + '\t' + "q23" + '\t' + "gpos25");
            StandardCytoBands.Add("chr15" + '\t' + "72700000" + '\t' + "75200000" + '\t' + "q24.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "75200000" + '\t' + "76600000" + '\t' + "q24.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr15" + '\t' + "76600000" + '\t' + "78300000" + '\t' + "q24.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "78300000" + '\t' + "81700000" + '\t' + "q25.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr15" + '\t' + "81700000" + '\t' + "85200000" + '\t' + "q25.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "85200000" + '\t' + "89100000" + '\t' + "q25.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr15" + '\t' + "89100000" + '\t' + "94300000" + '\t' + "q26.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr15" + '\t' + "94300000" + '\t' + "98500000" + '\t' + "q26.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr15" + '\t' + "98500000" + '\t' + "102531392" + '\t' + "q26.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "0" + '\t' + "7900000" + '\t' + "p13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "7900000" + '\t' + "10500000" + '\t' + "p13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr16" + '\t' + "10500000" + '\t' + "12600000" + '\t' + "p13.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "12600000" + '\t' + "14800000" + '\t' + "p13.12" + '\t' + "gpos50");
            StandardCytoBands.Add("chr16" + '\t' + "14800000" + '\t' + "16800000" + '\t' + "p13.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "16800000" + '\t' + "21200000" + '\t' + "p12.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr16" + '\t' + "21200000" + '\t' + "24200000" + '\t' + "p12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "24200000" + '\t' + "28100000" + '\t' + "p12.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr16" + '\t' + "28100000" + '\t' + "34600000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "34600000" + '\t' + "36600000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr16" + '\t' + "36600000" + '\t' + "38600000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr16" + '\t' + "38600000" + '\t' + "47000000" + '\t' + "q11.2" + '\t' + "gvar");
            StandardCytoBands.Add("chr16" + '\t' + "47000000" + '\t' + "52600000" + '\t' + "q12.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "52600000" + '\t' + "56700000" + '\t' + "q12.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr16" + '\t' + "56700000" + '\t' + "57400000" + '\t' + "q13" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "57400000" + '\t' + "66700000" + '\t' + "q21" + '\t' + "gpos100");
            StandardCytoBands.Add("chr16" + '\t' + "66700000" + '\t' + "70800000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "70800000" + '\t' + "72900000" + '\t' + "q22.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr16" + '\t' + "72900000" + '\t' + "74100000" + '\t' + "q22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "74100000" + '\t' + "79200000" + '\t' + "q23.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr16" + '\t' + "79200000" + '\t' + "81700000" + '\t' + "q23.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "81700000" + '\t' + "84200000" + '\t' + "q23.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr16" + '\t' + "84200000" + '\t' + "87100000" + '\t' + "q24.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr16" + '\t' + "87100000" + '\t' + "88700000" + '\t' + "q24.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr16" + '\t' + "88700000" + '\t' + "90354753" + '\t' + "q24.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "0" + '\t' + "3300000" + '\t' + "p13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "3300000" + '\t' + "6500000" + '\t' + "p13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr17" + '\t' + "6500000" + '\t' + "10700000" + '\t' + "p13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "10700000" + '\t' + "16000000" + '\t' + "p12" + '\t' + "gpos75");
            StandardCytoBands.Add("chr17" + '\t' + "16000000" + '\t' + "22200000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "22200000" + '\t' + "24000000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr17" + '\t' + "24000000" + '\t' + "25800000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr17" + '\t' + "25800000" + '\t' + "31800000" + '\t' + "q11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "31800000" + '\t' + "38100000" + '\t' + "q12" + '\t' + "gpos50");
            StandardCytoBands.Add("chr17" + '\t' + "38100000" + '\t' + "38400000" + '\t' + "q21.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "38400000" + '\t' + "40900000" + '\t' + "q21.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr17" + '\t' + "40900000" + '\t' + "44900000" + '\t' + "q21.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "44900000" + '\t' + "47400000" + '\t' + "q21.32" + '\t' + "gpos25");
            StandardCytoBands.Add("chr17" + '\t' + "47400000" + '\t' + "50200000" + '\t' + "q21.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "50200000" + '\t' + "57600000" + '\t' + "q22" + '\t' + "gpos75");
            StandardCytoBands.Add("chr17" + '\t' + "57600000" + '\t' + "58300000" + '\t' + "q23.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "58300000" + '\t' + "61100000" + '\t' + "q23.2" + '\t' + "gpos75");
            StandardCytoBands.Add("chr17" + '\t' + "61100000" + '\t' + "62600000" + '\t' + "q23.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "62600000" + '\t' + "64200000" + '\t' + "q24.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr17" + '\t' + "64200000" + '\t' + "67100000" + '\t' + "q24.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "67100000" + '\t' + "70900000" + '\t' + "q24.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr17" + '\t' + "70900000" + '\t' + "74800000" + '\t' + "q25.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr17" + '\t' + "74800000" + '\t' + "75300000" + '\t' + "q25.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr17" + '\t' + "75300000" + '\t' + "81195210" + '\t' + "q25.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "0" + '\t' + "2900000" + '\t' + "p11.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "2900000" + '\t' + "7100000" + '\t' + "p11.31" + '\t' + "gpos50");
            StandardCytoBands.Add("chr18" + '\t' + "7100000" + '\t' + "8500000" + '\t' + "p11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "8500000" + '\t' + "10900000" + '\t' + "p11.22" + '\t' + "gpos25");
            StandardCytoBands.Add("chr18" + '\t' + "10900000" + '\t' + "15400000" + '\t' + "p11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "15400000" + '\t' + "17200000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr18" + '\t' + "17200000" + '\t' + "19000000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr18" + '\t' + "19000000" + '\t' + "25000000" + '\t' + "q11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "25000000" + '\t' + "32700000" + '\t' + "q12.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr18" + '\t' + "32700000" + '\t' + "37200000" + '\t' + "q12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "37200000" + '\t' + "43500000" + '\t' + "q12.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr18" + '\t' + "43500000" + '\t' + "48200000" + '\t' + "q21.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "48200000" + '\t' + "53800000" + '\t' + "q21.2" + '\t' + "gpos75");
            StandardCytoBands.Add("chr18" + '\t' + "53800000" + '\t' + "56200000" + '\t' + "q21.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "56200000" + '\t' + "59000000" + '\t' + "q21.32" + '\t' + "gpos50");
            StandardCytoBands.Add("chr18" + '\t' + "59000000" + '\t' + "61600000" + '\t' + "q21.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "61600000" + '\t' + "66800000" + '\t' + "q22.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr18" + '\t' + "66800000" + '\t' + "68700000" + '\t' + "q22.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr18" + '\t' + "68700000" + '\t' + "73100000" + '\t' + "q22.3" + '\t' + "gpos25");
            StandardCytoBands.Add("chr18" + '\t' + "73100000" + '\t' + "78077248" + '\t' + "q23" + '\t' + "gneg");
            StandardCytoBands.Add("chr19" + '\t' + "0" + '\t' + "6900000" + '\t' + "p13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr19" + '\t' + "6900000" + '\t' + "13900000" + '\t' + "p13.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr19" + '\t' + "13900000" + '\t' + "14000000" + '\t' + "p13.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr19" + '\t' + "14000000" + '\t' + "16300000" + '\t' + "p13.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr19" + '\t' + "16300000" + '\t' + "20000000" + '\t' + "p13.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr19" + '\t' + "20000000" + '\t' + "24400000" + '\t' + "p12" + '\t' + "gvar");
            StandardCytoBands.Add("chr19" + '\t' + "24400000" + '\t' + "26500000" + '\t' + "p11" + '\t' + "acen");
            StandardCytoBands.Add("chr19" + '\t' + "26500000" + '\t' + "28600000" + '\t' + "q11" + '\t' + "acen");
            StandardCytoBands.Add("chr19" + '\t' + "28600000" + '\t' + "32400000" + '\t' + "q12" + '\t' + "gvar");
            StandardCytoBands.Add("chr19" + '\t' + "32400000" + '\t' + "35500000" + '\t' + "q13.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr19" + '\t' + "35500000" + '\t' + "38300000" + '\t' + "q13.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr19" + '\t' + "38300000" + '\t' + "38700000" + '\t' + "q13.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr19" + '\t' + "38700000" + '\t' + "43400000" + '\t' + "q13.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr19" + '\t' + "43400000" + '\t' + "45200000" + '\t' + "q13.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr19" + '\t' + "45200000" + '\t' + "48000000" + '\t' + "q13.32" + '\t' + "gpos25");
            StandardCytoBands.Add("chr19" + '\t' + "48000000" + '\t' + "51400000" + '\t' + "q13.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr19" + '\t' + "51400000" + '\t' + "53600000" + '\t' + "q13.41" + '\t' + "gpos25");
            StandardCytoBands.Add("chr19" + '\t' + "53600000" + '\t' + "56300000" + '\t' + "q13.42" + '\t' + "gneg");
            StandardCytoBands.Add("chr19" + '\t' + "56300000" + '\t' + "59128983" + '\t' + "q13.43" + '\t' + "gpos25");
            StandardCytoBands.Add("chr2" + '\t' + "0" + '\t' + "4400000" + '\t' + "p25.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "4400000" + '\t' + "7100000" + '\t' + "p25.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "7100000" + '\t' + "12200000" + '\t' + "p25.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "12200000" + '\t' + "16700000" + '\t' + "p24.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr2" + '\t' + "16700000" + '\t' + "19200000" + '\t' + "p24.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "19200000" + '\t' + "24000000" + '\t' + "p24.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr2" + '\t' + "24000000" + '\t' + "27900000" + '\t' + "p23.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "27900000" + '\t' + "30000000" + '\t' + "p23.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr2" + '\t' + "30000000" + '\t' + "32100000" + '\t' + "p23.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "32100000" + '\t' + "36600000" + '\t' + "p22.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr2" + '\t' + "36600000" + '\t' + "38600000" + '\t' + "p22.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "38600000" + '\t' + "41800000" + '\t' + "p22.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "41800000" + '\t' + "47800000" + '\t' + "p21" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "47800000" + '\t' + "52900000" + '\t' + "p16.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr2" + '\t' + "52900000" + '\t' + "55000000" + '\t' + "p16.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "55000000" + '\t' + "61300000" + '\t' + "p16.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr2" + '\t' + "61300000" + '\t' + "64100000" + '\t' + "p15" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "64100000" + '\t' + "68600000" + '\t' + "p14" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "68600000" + '\t' + "71500000" + '\t' + "p13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "71500000" + '\t' + "73500000" + '\t' + "p13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "73500000" + '\t' + "75000000" + '\t' + "p13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "75000000" + '\t' + "83300000" + '\t' + "p12" + '\t' + "gpos100");
            StandardCytoBands.Add("chr2" + '\t' + "83300000" + '\t' + "90500000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "90500000" + '\t' + "93300000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr2" + '\t' + "93300000" + '\t' + "96800000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr2" + '\t' + "96800000" + '\t' + "102700000" + '\t' + "q11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "102700000" + '\t' + "106000000" + '\t' + "q12.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "106000000" + '\t' + "107500000" + '\t' + "q12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "107500000" + '\t' + "110200000" + '\t' + "q12.3" + '\t' + "gpos25");
            StandardCytoBands.Add("chr2" + '\t' + "110200000" + '\t' + "114400000" + '\t' + "q13" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "114400000" + '\t' + "118800000" + '\t' + "q14.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "118800000" + '\t' + "122400000" + '\t' + "q14.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "122400000" + '\t' + "129900000" + '\t' + "q14.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "129900000" + '\t' + "132500000" + '\t' + "q21.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "132500000" + '\t' + "135100000" + '\t' + "q21.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr2" + '\t' + "135100000" + '\t' + "136800000" + '\t' + "q21.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "136800000" + '\t' + "142200000" + '\t' + "q22.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr2" + '\t' + "142200000" + '\t' + "144100000" + '\t' + "q22.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "144100000" + '\t' + "148700000" + '\t' + "q22.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr2" + '\t' + "148700000" + '\t' + "149900000" + '\t' + "q23.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "149900000" + '\t' + "150500000" + '\t' + "q23.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr2" + '\t' + "150500000" + '\t' + "154900000" + '\t' + "q23.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "154900000" + '\t' + "159800000" + '\t' + "q24.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr2" + '\t' + "159800000" + '\t' + "163700000" + '\t' + "q24.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "163700000" + '\t' + "169700000" + '\t' + "q24.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr2" + '\t' + "169700000" + '\t' + "178000000" + '\t' + "q31.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "178000000" + '\t' + "180600000" + '\t' + "q31.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "180600000" + '\t' + "183000000" + '\t' + "q31.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "183000000" + '\t' + "189400000" + '\t' + "q32.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr2" + '\t' + "189400000" + '\t' + "191900000" + '\t' + "q32.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "191900000" + '\t' + "197400000" + '\t' + "q32.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr2" + '\t' + "197400000" + '\t' + "203300000" + '\t' + "q33.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "203300000" + '\t' + "204900000" + '\t' + "q33.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "204900000" + '\t' + "209000000" + '\t' + "q33.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "209000000" + '\t' + "215300000" + '\t' + "q34" + '\t' + "gpos100");
            StandardCytoBands.Add("chr2" + '\t' + "215300000" + '\t' + "221500000" + '\t' + "q35" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "221500000" + '\t' + "225200000" + '\t' + "q36.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr2" + '\t' + "225200000" + '\t' + "226100000" + '\t' + "q36.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "226100000" + '\t' + "231000000" + '\t' + "q36.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr2" + '\t' + "231000000" + '\t' + "235600000" + '\t' + "q37.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr2" + '\t' + "235600000" + '\t' + "237300000" + '\t' + "q37.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr2" + '\t' + "237300000" + '\t' + "243199373" + '\t' + "q37.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "0" + '\t' + "5100000" + '\t' + "p13" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "5100000" + '\t' + "9200000" + '\t' + "p12.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr20" + '\t' + "9200000" + '\t' + "12100000" + '\t' + "p12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "12100000" + '\t' + "17900000" + '\t' + "p12.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr20" + '\t' + "17900000" + '\t' + "21300000" + '\t' + "p11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "21300000" + '\t' + "22300000" + '\t' + "p11.22" + '\t' + "gpos25");
            StandardCytoBands.Add("chr20" + '\t' + "22300000" + '\t' + "25600000" + '\t' + "p11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "25600000" + '\t' + "27500000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr20" + '\t' + "27500000" + '\t' + "29400000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr20" + '\t' + "29400000" + '\t' + "32100000" + '\t' + "q11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "32100000" + '\t' + "34400000" + '\t' + "q11.22" + '\t' + "gpos25");
            StandardCytoBands.Add("chr20" + '\t' + "34400000" + '\t' + "37600000" + '\t' + "q11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "37600000" + '\t' + "41700000" + '\t' + "q12" + '\t' + "gpos75");
            StandardCytoBands.Add("chr20" + '\t' + "41700000" + '\t' + "42100000" + '\t' + "q13.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "42100000" + '\t' + "46400000" + '\t' + "q13.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr20" + '\t' + "46400000" + '\t' + "49800000" + '\t' + "q13.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "49800000" + '\t' + "55000000" + '\t' + "q13.2" + '\t' + "gpos75");
            StandardCytoBands.Add("chr20" + '\t' + "55000000" + '\t' + "56500000" + '\t' + "q13.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr20" + '\t' + "56500000" + '\t' + "58400000" + '\t' + "q13.32" + '\t' + "gpos50");
            StandardCytoBands.Add("chr20" + '\t' + "58400000" + '\t' + "63025520" + '\t' + "q13.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr21" + '\t' + "0" + '\t' + "2800000" + '\t' + "p13" + '\t' + "gvar");
            StandardCytoBands.Add("chr21" + '\t' + "2800000" + '\t' + "6800000" + '\t' + "p12" + '\t' + "stalk");
            StandardCytoBands.Add("chr21" + '\t' + "6800000" + '\t' + "10900000" + '\t' + "p11.2" + '\t' + "gvar");
            StandardCytoBands.Add("chr21" + '\t' + "10900000" + '\t' + "13200000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr21" + '\t' + "13200000" + '\t' + "14300000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr21" + '\t' + "14300000" + '\t' + "16400000" + '\t' + "q11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr21" + '\t' + "16400000" + '\t' + "24000000" + '\t' + "q21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr21" + '\t' + "24000000" + '\t' + "26800000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr21" + '\t' + "26800000" + '\t' + "31500000" + '\t' + "q21.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr21" + '\t' + "31500000" + '\t' + "35800000" + '\t' + "q22.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr21" + '\t' + "35800000" + '\t' + "37800000" + '\t' + "q22.12" + '\t' + "gpos50");
            StandardCytoBands.Add("chr21" + '\t' + "37800000" + '\t' + "39700000" + '\t' + "q22.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr21" + '\t' + "39700000" + '\t' + "42600000" + '\t' + "q22.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr21" + '\t' + "42600000" + '\t' + "48129895" + '\t' + "q22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr22" + '\t' + "0" + '\t' + "3800000" + '\t' + "p13" + '\t' + "gvar");
            StandardCytoBands.Add("chr22" + '\t' + "3800000" + '\t' + "8300000" + '\t' + "p12" + '\t' + "stalk");
            StandardCytoBands.Add("chr22" + '\t' + "8300000" + '\t' + "12200000" + '\t' + "p11.2" + '\t' + "gvar");
            StandardCytoBands.Add("chr22" + '\t' + "12200000" + '\t' + "14700000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr22" + '\t' + "14700000" + '\t' + "17900000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr22" + '\t' + "17900000" + '\t' + "22200000" + '\t' + "q11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr22" + '\t' + "22200000" + '\t' + "23500000" + '\t' + "q11.22" + '\t' + "gpos25");
            StandardCytoBands.Add("chr22" + '\t' + "23500000" + '\t' + "25900000" + '\t' + "q11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr22" + '\t' + "25900000" + '\t' + "29600000" + '\t' + "q12.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr22" + '\t' + "29600000" + '\t' + "32200000" + '\t' + "q12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr22" + '\t' + "32200000" + '\t' + "37600000" + '\t' + "q12.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr22" + '\t' + "37600000" + '\t' + "41000000" + '\t' + "q13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr22" + '\t' + "41000000" + '\t' + "44200000" + '\t' + "q13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr22" + '\t' + "44200000" + '\t' + "48400000" + '\t' + "q13.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr22" + '\t' + "48400000" + '\t' + "49400000" + '\t' + "q13.32" + '\t' + "gpos50");
            StandardCytoBands.Add("chr22" + '\t' + "49400000" + '\t' + "51304566" + '\t' + "q13.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "0" + '\t' + "2800000" + '\t' + "p26.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr3" + '\t' + "2800000" + '\t' + "4000000" + '\t' + "p26.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "4000000" + '\t' + "8700000" + '\t' + "p26.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr3" + '\t' + "8700000" + '\t' + "11800000" + '\t' + "p25.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "11800000" + '\t' + "13300000" + '\t' + "p25.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr3" + '\t' + "13300000" + '\t' + "16400000" + '\t' + "p25.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "16400000" + '\t' + "23900000" + '\t' + "p24.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr3" + '\t' + "23900000" + '\t' + "26400000" + '\t' + "p24.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "26400000" + '\t' + "30900000" + '\t' + "p24.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "30900000" + '\t' + "32100000" + '\t' + "p23" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "32100000" + '\t' + "36500000" + '\t' + "p22.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr3" + '\t' + "36500000" + '\t' + "39400000" + '\t' + "p22.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "39400000" + '\t' + "43700000" + '\t' + "p22.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "43700000" + '\t' + "44100000" + '\t' + "p21.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "44100000" + '\t' + "44200000" + '\t' + "p21.32" + '\t' + "gpos50");
            StandardCytoBands.Add("chr3" + '\t' + "44200000" + '\t' + "50600000" + '\t' + "p21.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "50600000" + '\t' + "52300000" + '\t' + "p21.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr3" + '\t' + "52300000" + '\t' + "54400000" + '\t' + "p21.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "54400000" + '\t' + "58600000" + '\t' + "p14.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr3" + '\t' + "58600000" + '\t' + "63700000" + '\t' + "p14.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "63700000" + '\t' + "69800000" + '\t' + "p14.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr3" + '\t' + "69800000" + '\t' + "74200000" + '\t' + "p13" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "74200000" + '\t' + "79800000" + '\t' + "p12.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "79800000" + '\t' + "83500000" + '\t' + "p12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "83500000" + '\t' + "87200000" + '\t' + "p12.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "87200000" + '\t' + "87900000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "87900000" + '\t' + "91000000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr3" + '\t' + "91000000" + '\t' + "93900000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr3" + '\t' + "93900000" + '\t' + "98300000" + '\t' + "q11.2" + '\t' + "gvar");
            StandardCytoBands.Add("chr3" + '\t' + "98300000" + '\t' + "100000000" + '\t' + "q12.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "100000000" + '\t' + "100900000" + '\t' + "q12.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr3" + '\t' + "100900000" + '\t' + "102800000" + '\t' + "q12.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "102800000" + '\t' + "106200000" + '\t' + "q13.11" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "106200000" + '\t' + "107900000" + '\t' + "q13.12" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "107900000" + '\t' + "111300000" + '\t' + "q13.13" + '\t' + "gpos50");
            StandardCytoBands.Add("chr3" + '\t' + "111300000" + '\t' + "113500000" + '\t' + "q13.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "113500000" + '\t' + "117300000" + '\t' + "q13.31" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "117300000" + '\t' + "119000000" + '\t' + "q13.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "119000000" + '\t' + "121900000" + '\t' + "q13.33" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "121900000" + '\t' + "123800000" + '\t' + "q21.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "123800000" + '\t' + "125800000" + '\t' + "q21.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr3" + '\t' + "125800000" + '\t' + "129200000" + '\t' + "q21.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "129200000" + '\t' + "133700000" + '\t' + "q22.1" + '\t' + "gpos25");
            StandardCytoBands.Add("chr3" + '\t' + "133700000" + '\t' + "135700000" + '\t' + "q22.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "135700000" + '\t' + "138700000" + '\t' + "q22.3" + '\t' + "gpos25");
            StandardCytoBands.Add("chr3" + '\t' + "138700000" + '\t' + "142800000" + '\t' + "q23" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "142800000" + '\t' + "148900000" + '\t' + "q24" + '\t' + "gpos100");
            StandardCytoBands.Add("chr3" + '\t' + "148900000" + '\t' + "152100000" + '\t' + "q25.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "152100000" + '\t' + "155000000" + '\t' + "q25.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr3" + '\t' + "155000000" + '\t' + "157000000" + '\t' + "q25.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "157000000" + '\t' + "159000000" + '\t' + "q25.32" + '\t' + "gpos50");
            StandardCytoBands.Add("chr3" + '\t' + "159000000" + '\t' + "160700000" + '\t' + "q25.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "160700000" + '\t' + "167600000" + '\t' + "q26.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr3" + '\t' + "167600000" + '\t' + "170900000" + '\t' + "q26.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "170900000" + '\t' + "175700000" + '\t' + "q26.31" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "175700000" + '\t' + "179000000" + '\t' + "q26.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "179000000" + '\t' + "182700000" + '\t' + "q26.33" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "182700000" + '\t' + "184500000" + '\t' + "q27.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "184500000" + '\t' + "186000000" + '\t' + "q27.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr3" + '\t' + "186000000" + '\t' + "187900000" + '\t' + "q27.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr3" + '\t' + "187900000" + '\t' + "192300000" + '\t' + "q28" + '\t' + "gpos75");
            StandardCytoBands.Add("chr3" + '\t' + "192300000" + '\t' + "198022430" + '\t' + "q29" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "0" + '\t' + "4500000" + '\t' + "p16.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "4500000" + '\t' + "6000000" + '\t' + "p16.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr4" + '\t' + "6000000" + '\t' + "11300000" + '\t' + "p16.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "11300000" + '\t' + "15200000" + '\t' + "p15.33" + '\t' + "gpos50");
            StandardCytoBands.Add("chr4" + '\t' + "15200000" + '\t' + "17800000" + '\t' + "p15.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "17800000" + '\t' + "21300000" + '\t' + "p15.31" + '\t' + "gpos75");
            StandardCytoBands.Add("chr4" + '\t' + "21300000" + '\t' + "27700000" + '\t' + "p15.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "27700000" + '\t' + "35800000" + '\t' + "p15.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr4" + '\t' + "35800000" + '\t' + "41200000" + '\t' + "p14" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "41200000" + '\t' + "44600000" + '\t' + "p13" + '\t' + "gpos50");
            StandardCytoBands.Add("chr4" + '\t' + "44600000" + '\t' + "48200000" + '\t' + "p12" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "48200000" + '\t' + "50400000" + '\t' + "p11" + '\t' + "acen");
            StandardCytoBands.Add("chr4" + '\t' + "50400000" + '\t' + "52700000" + '\t' + "q11" + '\t' + "acen");
            StandardCytoBands.Add("chr4" + '\t' + "52700000" + '\t' + "59500000" + '\t' + "q12" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "59500000" + '\t' + "66600000" + '\t' + "q13.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr4" + '\t' + "66600000" + '\t' + "70500000" + '\t' + "q13.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "70500000" + '\t' + "76300000" + '\t' + "q13.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr4" + '\t' + "76300000" + '\t' + "78900000" + '\t' + "q21.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "78900000" + '\t' + "82400000" + '\t' + "q21.21" + '\t' + "gpos50");
            StandardCytoBands.Add("chr4" + '\t' + "82400000" + '\t' + "84100000" + '\t' + "q21.22" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "84100000" + '\t' + "86900000" + '\t' + "q21.23" + '\t' + "gpos25");
            StandardCytoBands.Add("chr4" + '\t' + "86900000" + '\t' + "88000000" + '\t' + "q21.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "88000000" + '\t' + "93700000" + '\t' + "q22.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr4" + '\t' + "93700000" + '\t' + "95100000" + '\t' + "q22.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "95100000" + '\t' + "98800000" + '\t' + "q22.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr4" + '\t' + "98800000" + '\t' + "101100000" + '\t' + "q23" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "101100000" + '\t' + "107700000" + '\t' + "q24" + '\t' + "gpos50");
            StandardCytoBands.Add("chr4" + '\t' + "107700000" + '\t' + "114100000" + '\t' + "q25" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "114100000" + '\t' + "120800000" + '\t' + "q26" + '\t' + "gpos75");
            StandardCytoBands.Add("chr4" + '\t' + "120800000" + '\t' + "123800000" + '\t' + "q27" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "123800000" + '\t' + "128800000" + '\t' + "q28.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr4" + '\t' + "128800000" + '\t' + "131100000" + '\t' + "q28.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "131100000" + '\t' + "139500000" + '\t' + "q28.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr4" + '\t' + "139500000" + '\t' + "141500000" + '\t' + "q31.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "141500000" + '\t' + "146800000" + '\t' + "q31.21" + '\t' + "gpos25");
            StandardCytoBands.Add("chr4" + '\t' + "146800000" + '\t' + "148500000" + '\t' + "q31.22" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "148500000" + '\t' + "151100000" + '\t' + "q31.23" + '\t' + "gpos25");
            StandardCytoBands.Add("chr4" + '\t' + "151100000" + '\t' + "155600000" + '\t' + "q31.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "155600000" + '\t' + "161800000" + '\t' + "q32.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr4" + '\t' + "161800000" + '\t' + "164500000" + '\t' + "q32.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "164500000" + '\t' + "170100000" + '\t' + "q32.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr4" + '\t' + "170100000" + '\t' + "171900000" + '\t' + "q33" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "171900000" + '\t' + "176300000" + '\t' + "q34.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr4" + '\t' + "176300000" + '\t' + "177500000" + '\t' + "q34.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "177500000" + '\t' + "183200000" + '\t' + "q34.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr4" + '\t' + "183200000" + '\t' + "187100000" + '\t' + "q35.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr4" + '\t' + "187100000" + '\t' + "191154276" + '\t' + "q35.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr5" + '\t' + "0" + '\t' + "4500000" + '\t' + "p15.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "4500000" + '\t' + "6300000" + '\t' + "p15.32" + '\t' + "gpos25");
            StandardCytoBands.Add("chr5" + '\t' + "6300000" + '\t' + "9800000" + '\t' + "p15.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "9800000" + '\t' + "15000000" + '\t' + "p15.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr5" + '\t' + "15000000" + '\t' + "18400000" + '\t' + "p15.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "18400000" + '\t' + "23300000" + '\t' + "p14.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr5" + '\t' + "23300000" + '\t' + "24600000" + '\t' + "p14.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "24600000" + '\t' + "28900000" + '\t' + "p14.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr5" + '\t' + "28900000" + '\t' + "33800000" + '\t' + "p13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "33800000" + '\t' + "38400000" + '\t' + "p13.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr5" + '\t' + "38400000" + '\t' + "42500000" + '\t' + "p13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "42500000" + '\t' + "46100000" + '\t' + "p12" + '\t' + "gpos50");
            StandardCytoBands.Add("chr5" + '\t' + "46100000" + '\t' + "48400000" + '\t' + "p11" + '\t' + "acen");
            StandardCytoBands.Add("chr5" + '\t' + "48400000" + '\t' + "50700000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr5" + '\t' + "50700000" + '\t' + "58900000" + '\t' + "q11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "58900000" + '\t' + "62900000" + '\t' + "q12.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr5" + '\t' + "62900000" + '\t' + "63200000" + '\t' + "q12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "63200000" + '\t' + "66700000" + '\t' + "q12.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr5" + '\t' + "66700000" + '\t' + "68400000" + '\t' + "q13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "68400000" + '\t' + "73300000" + '\t' + "q13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr5" + '\t' + "73300000" + '\t' + "76900000" + '\t' + "q13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "76900000" + '\t' + "81400000" + '\t' + "q14.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr5" + '\t' + "81400000" + '\t' + "82800000" + '\t' + "q14.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "82800000" + '\t' + "92300000" + '\t' + "q14.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr5" + '\t' + "92300000" + '\t' + "98200000" + '\t' + "q15" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "98200000" + '\t' + "102800000" + '\t' + "q21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr5" + '\t' + "102800000" + '\t' + "104500000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "104500000" + '\t' + "109600000" + '\t' + "q21.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr5" + '\t' + "109600000" + '\t' + "111500000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "111500000" + '\t' + "113100000" + '\t' + "q22.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr5" + '\t' + "113100000" + '\t' + "115200000" + '\t' + "q22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "115200000" + '\t' + "121400000" + '\t' + "q23.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr5" + '\t' + "121400000" + '\t' + "127300000" + '\t' + "q23.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "127300000" + '\t' + "130600000" + '\t' + "q23.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr5" + '\t' + "130600000" + '\t' + "136200000" + '\t' + "q31.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "136200000" + '\t' + "139500000" + '\t' + "q31.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr5" + '\t' + "139500000" + '\t' + "144500000" + '\t' + "q31.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "144500000" + '\t' + "149800000" + '\t' + "q32" + '\t' + "gpos75");
            StandardCytoBands.Add("chr5" + '\t' + "149800000" + '\t' + "152700000" + '\t' + "q33.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "152700000" + '\t' + "155700000" + '\t' + "q33.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr5" + '\t' + "155700000" + '\t' + "159900000" + '\t' + "q33.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "159900000" + '\t' + "168500000" + '\t' + "q34" + '\t' + "gpos100");
            StandardCytoBands.Add("chr5" + '\t' + "168500000" + '\t' + "172800000" + '\t' + "q35.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr5" + '\t' + "172800000" + '\t' + "176600000" + '\t' + "q35.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr5" + '\t' + "176600000" + '\t' + "180915260" + '\t' + "q35.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "0" + '\t' + "2300000" + '\t' + "p25.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "2300000" + '\t' + "4200000" + '\t' + "p25.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr6" + '\t' + "4200000" + '\t' + "7100000" + '\t' + "p25.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "7100000" + '\t' + "10600000" + '\t' + "p24.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr6" + '\t' + "10600000" + '\t' + "11600000" + '\t' + "p24.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "11600000" + '\t' + "13400000" + '\t' + "p24.1" + '\t' + "gpos25");
            StandardCytoBands.Add("chr6" + '\t' + "13400000" + '\t' + "15200000" + '\t' + "p23" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "15200000" + '\t' + "25200000" + '\t' + "p22.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr6" + '\t' + "25200000" + '\t' + "27000000" + '\t' + "p22.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "27000000" + '\t' + "30400000" + '\t' + "p22.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr6" + '\t' + "30400000" + '\t' + "32100000" + '\t' + "p21.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "32100000" + '\t' + "33500000" + '\t' + "p21.32" + '\t' + "gpos25");
            StandardCytoBands.Add("chr6" + '\t' + "33500000" + '\t' + "36600000" + '\t' + "p21.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "36600000" + '\t' + "40500000" + '\t' + "p21.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr6" + '\t' + "40500000" + '\t' + "46200000" + '\t' + "p21.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "46200000" + '\t' + "51800000" + '\t' + "p12.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr6" + '\t' + "51800000" + '\t' + "52900000" + '\t' + "p12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "52900000" + '\t' + "57000000" + '\t' + "p12.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr6" + '\t' + "57000000" + '\t' + "58700000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "58700000" + '\t' + "61000000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr6" + '\t' + "61000000" + '\t' + "63300000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr6" + '\t' + "63300000" + '\t' + "63400000" + '\t' + "q11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "63400000" + '\t' + "70000000" + '\t' + "q12" + '\t' + "gpos100");
            StandardCytoBands.Add("chr6" + '\t' + "70000000" + '\t' + "75900000" + '\t' + "q13" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "75900000" + '\t' + "83900000" + '\t' + "q14.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr6" + '\t' + "83900000" + '\t' + "84900000" + '\t' + "q14.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "84900000" + '\t' + "88000000" + '\t' + "q14.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr6" + '\t' + "88000000" + '\t' + "93100000" + '\t' + "q15" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "93100000" + '\t' + "99500000" + '\t' + "q16.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr6" + '\t' + "99500000" + '\t' + "100600000" + '\t' + "q16.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "100600000" + '\t' + "105500000" + '\t' + "q16.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr6" + '\t' + "105500000" + '\t' + "114600000" + '\t' + "q21" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "114600000" + '\t' + "118300000" + '\t' + "q22.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr6" + '\t' + "118300000" + '\t' + "118500000" + '\t' + "q22.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "118500000" + '\t' + "126100000" + '\t' + "q22.31" + '\t' + "gpos100");
            StandardCytoBands.Add("chr6" + '\t' + "126100000" + '\t' + "127100000" + '\t' + "q22.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "127100000" + '\t' + "130300000" + '\t' + "q22.33" + '\t' + "gpos75");
            StandardCytoBands.Add("chr6" + '\t' + "130300000" + '\t' + "131200000" + '\t' + "q23.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "131200000" + '\t' + "135200000" + '\t' + "q23.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr6" + '\t' + "135200000" + '\t' + "139000000" + '\t' + "q23.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "139000000" + '\t' + "142800000" + '\t' + "q24.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr6" + '\t' + "142800000" + '\t' + "145600000" + '\t' + "q24.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "145600000" + '\t' + "149000000" + '\t' + "q24.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr6" + '\t' + "149000000" + '\t' + "152500000" + '\t' + "q25.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "152500000" + '\t' + "155500000" + '\t' + "q25.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr6" + '\t' + "155500000" + '\t' + "161000000" + '\t' + "q25.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr6" + '\t' + "161000000" + '\t' + "164500000" + '\t' + "q26" + '\t' + "gpos50");
            StandardCytoBands.Add("chr6" + '\t' + "164500000" + '\t' + "171115067" + '\t' + "q27" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "0" + '\t' + "2800000" + '\t' + "p22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "2800000" + '\t' + "4500000" + '\t' + "p22.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr7" + '\t' + "4500000" + '\t' + "7300000" + '\t' + "p22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "7300000" + '\t' + "13800000" + '\t' + "p21.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr7" + '\t' + "13800000" + '\t' + "16500000" + '\t' + "p21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "16500000" + '\t' + "20900000" + '\t' + "p21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr7" + '\t' + "20900000" + '\t' + "25500000" + '\t' + "p15.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "25500000" + '\t' + "28000000" + '\t' + "p15.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr7" + '\t' + "28000000" + '\t' + "28800000" + '\t' + "p15.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "28800000" + '\t' + "35000000" + '\t' + "p14.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "35000000" + '\t' + "37200000" + '\t' + "p14.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "37200000" + '\t' + "43300000" + '\t' + "p14.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "43300000" + '\t' + "45400000" + '\t' + "p13" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "45400000" + '\t' + "49000000" + '\t' + "p12.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "49000000" + '\t' + "50500000" + '\t' + "p12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "50500000" + '\t' + "54000000" + '\t' + "p12.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "54000000" + '\t' + "58000000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "58000000" + '\t' + "59900000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr7" + '\t' + "59900000" + '\t' + "61700000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr7" + '\t' + "61700000" + '\t' + "67000000" + '\t' + "q11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "67000000" + '\t' + "72200000" + '\t' + "q11.22" + '\t' + "gpos50");
            StandardCytoBands.Add("chr7" + '\t' + "72200000" + '\t' + "77500000" + '\t' + "q11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "77500000" + '\t' + "86400000" + '\t' + "q21.11" + '\t' + "gpos100");
            StandardCytoBands.Add("chr7" + '\t' + "86400000" + '\t' + "88200000" + '\t' + "q21.12" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "88200000" + '\t' + "91100000" + '\t' + "q21.13" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "91100000" + '\t' + "92800000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "92800000" + '\t' + "98000000" + '\t' + "q21.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "98000000" + '\t' + "103800000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "103800000" + '\t' + "104500000" + '\t' + "q22.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr7" + '\t' + "104500000" + '\t' + "107400000" + '\t' + "q22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "107400000" + '\t' + "114600000" + '\t' + "q31.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "114600000" + '\t' + "117400000" + '\t' + "q31.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "117400000" + '\t' + "121100000" + '\t' + "q31.31" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "121100000" + '\t' + "123800000" + '\t' + "q31.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "123800000" + '\t' + "127100000" + '\t' + "q31.33" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "127100000" + '\t' + "129200000" + '\t' + "q32.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "129200000" + '\t' + "130400000" + '\t' + "q32.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr7" + '\t' + "130400000" + '\t' + "132600000" + '\t' + "q32.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "132600000" + '\t' + "138200000" + '\t' + "q33" + '\t' + "gpos50");
            StandardCytoBands.Add("chr7" + '\t' + "138200000" + '\t' + "143100000" + '\t' + "q34" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "143100000" + '\t' + "147900000" + '\t' + "q35" + '\t' + "gpos75");
            StandardCytoBands.Add("chr7" + '\t' + "147900000" + '\t' + "152600000" + '\t' + "q36.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr7" + '\t' + "152600000" + '\t' + "155100000" + '\t' + "q36.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr7" + '\t' + "155100000" + '\t' + "159138663" + '\t' + "q36.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "0" + '\t' + "2200000" + '\t' + "p23.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "2200000" + '\t' + "6200000" + '\t' + "p23.2" + '\t' + "gpos75");
            StandardCytoBands.Add("chr8" + '\t' + "6200000" + '\t' + "12700000" + '\t' + "p23.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "12700000" + '\t' + "19000000" + '\t' + "p22" + '\t' + "gpos100");
            StandardCytoBands.Add("chr8" + '\t' + "19000000" + '\t' + "23300000" + '\t' + "p21.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "23300000" + '\t' + "27400000" + '\t' + "p21.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr8" + '\t' + "27400000" + '\t' + "28800000" + '\t' + "p21.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "28800000" + '\t' + "36500000" + '\t' + "p12" + '\t' + "gpos75");
            StandardCytoBands.Add("chr8" + '\t' + "36500000" + '\t' + "38300000" + '\t' + "p11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "38300000" + '\t' + "39700000" + '\t' + "p11.22" + '\t' + "gpos25");
            StandardCytoBands.Add("chr8" + '\t' + "39700000" + '\t' + "43100000" + '\t' + "p11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "43100000" + '\t' + "45600000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr8" + '\t' + "45600000" + '\t' + "48100000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr8" + '\t' + "48100000" + '\t' + "52200000" + '\t' + "q11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "52200000" + '\t' + "52600000" + '\t' + "q11.22" + '\t' + "gpos75");
            StandardCytoBands.Add("chr8" + '\t' + "52600000" + '\t' + "55500000" + '\t' + "q11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "55500000" + '\t' + "61600000" + '\t' + "q12.1" + '\t' + "gpos50");
            StandardCytoBands.Add("chr8" + '\t' + "61600000" + '\t' + "62200000" + '\t' + "q12.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "62200000" + '\t' + "66000000" + '\t' + "q12.3" + '\t' + "gpos50");
            StandardCytoBands.Add("chr8" + '\t' + "66000000" + '\t' + "68000000" + '\t' + "q13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "68000000" + '\t' + "70500000" + '\t' + "q13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chr8" + '\t' + "70500000" + '\t' + "73900000" + '\t' + "q13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "73900000" + '\t' + "78300000" + '\t' + "q21.11" + '\t' + "gpos100");
            StandardCytoBands.Add("chr8" + '\t' + "78300000" + '\t' + "80100000" + '\t' + "q21.12" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "80100000" + '\t' + "84600000" + '\t' + "q21.13" + '\t' + "gpos75");
            StandardCytoBands.Add("chr8" + '\t' + "84600000" + '\t' + "86900000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "86900000" + '\t' + "93300000" + '\t' + "q21.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr8" + '\t' + "93300000" + '\t' + "99000000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "99000000" + '\t' + "101600000" + '\t' + "q22.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr8" + '\t' + "101600000" + '\t' + "106200000" + '\t' + "q22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "106200000" + '\t' + "110500000" + '\t' + "q23.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr8" + '\t' + "110500000" + '\t' + "112100000" + '\t' + "q23.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "112100000" + '\t' + "117700000" + '\t' + "q23.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr8" + '\t' + "117700000" + '\t' + "119200000" + '\t' + "q24.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "119200000" + '\t' + "122500000" + '\t' + "q24.12" + '\t' + "gpos50");
            StandardCytoBands.Add("chr8" + '\t' + "122500000" + '\t' + "127300000" + '\t' + "q24.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "127300000" + '\t' + "131500000" + '\t' + "q24.21" + '\t' + "gpos50");
            StandardCytoBands.Add("chr8" + '\t' + "131500000" + '\t' + "136400000" + '\t' + "q24.22" + '\t' + "gneg");
            StandardCytoBands.Add("chr8" + '\t' + "136400000" + '\t' + "139900000" + '\t' + "q24.23" + '\t' + "gpos75");
            StandardCytoBands.Add("chr8" + '\t' + "139900000" + '\t' + "146364022" + '\t' + "q24.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "0" + '\t' + "2200000" + '\t' + "p24.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "2200000" + '\t' + "4600000" + '\t' + "p24.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "4600000" + '\t' + "9000000" + '\t' + "p24.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "9000000" + '\t' + "14200000" + '\t' + "p23" + '\t' + "gpos75");
            StandardCytoBands.Add("chr9" + '\t' + "14200000" + '\t' + "16600000" + '\t' + "p22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "16600000" + '\t' + "18500000" + '\t' + "p22.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "18500000" + '\t' + "19900000" + '\t' + "p22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "19900000" + '\t' + "25600000" + '\t' + "p21.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chr9" + '\t' + "25600000" + '\t' + "28000000" + '\t' + "p21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "28000000" + '\t' + "33200000" + '\t' + "p21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr9" + '\t' + "33200000" + '\t' + "36300000" + '\t' + "p13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "36300000" + '\t' + "38400000" + '\t' + "p13.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "38400000" + '\t' + "41000000" + '\t' + "p13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "41000000" + '\t' + "43600000" + '\t' + "p12" + '\t' + "gpos50");
            StandardCytoBands.Add("chr9" + '\t' + "43600000" + '\t' + "47300000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "47300000" + '\t' + "49000000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chr9" + '\t' + "49000000" + '\t' + "50700000" + '\t' + "q11" + '\t' + "acen");
            StandardCytoBands.Add("chr9" + '\t' + "50700000" + '\t' + "65900000" + '\t' + "q12" + '\t' + "gvar");
            StandardCytoBands.Add("chr9" + '\t' + "65900000" + '\t' + "68700000" + '\t' + "q13" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "68700000" + '\t' + "72200000" + '\t' + "q21.11" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "72200000" + '\t' + "74000000" + '\t' + "q21.12" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "74000000" + '\t' + "79200000" + '\t' + "q21.13" + '\t' + "gpos50");
            StandardCytoBands.Add("chr9" + '\t' + "79200000" + '\t' + "81100000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "81100000" + '\t' + "84100000" + '\t' + "q21.31" + '\t' + "gpos50");
            StandardCytoBands.Add("chr9" + '\t' + "84100000" + '\t' + "86900000" + '\t' + "q21.32" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "86900000" + '\t' + "90400000" + '\t' + "q21.33" + '\t' + "gpos50");
            StandardCytoBands.Add("chr9" + '\t' + "90400000" + '\t' + "91800000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "91800000" + '\t' + "93900000" + '\t' + "q22.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "93900000" + '\t' + "96600000" + '\t' + "q22.31" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "96600000" + '\t' + "99300000" + '\t' + "q22.32" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "99300000" + '\t' + "102600000" + '\t' + "q22.33" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "102600000" + '\t' + "108200000" + '\t' + "q31.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chr9" + '\t' + "108200000" + '\t' + "111300000" + '\t' + "q31.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "111300000" + '\t' + "114900000" + '\t' + "q31.3" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "114900000" + '\t' + "117700000" + '\t' + "q32" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "117700000" + '\t' + "122500000" + '\t' + "q33.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chr9" + '\t' + "122500000" + '\t' + "125800000" + '\t' + "q33.2" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "125800000" + '\t' + "130300000" + '\t' + "q33.3" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "130300000" + '\t' + "133500000" + '\t' + "q34.11" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "133500000" + '\t' + "134000000" + '\t' + "q34.12" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "134000000" + '\t' + "135900000" + '\t' + "q34.13" + '\t' + "gneg");
            StandardCytoBands.Add("chr9" + '\t' + "135900000" + '\t' + "137400000" + '\t' + "q34.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chr9" + '\t' + "137400000" + '\t' + "141213431" + '\t' + "q34.3" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "0" + '\t' + "4300000" + '\t' + "p22.33" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "4300000" + '\t' + "6000000" + '\t' + "p22.32" + '\t' + "gpos50");
            StandardCytoBands.Add("chrX" + '\t' + "6000000" + '\t' + "9500000" + '\t' + "p22.31" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "9500000" + '\t' + "17100000" + '\t' + "p22.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chrX" + '\t' + "17100000" + '\t' + "19300000" + '\t' + "p22.13" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "19300000" + '\t' + "21900000" + '\t' + "p22.12" + '\t' + "gpos50");
            StandardCytoBands.Add("chrX" + '\t' + "21900000" + '\t' + "24900000" + '\t' + "p22.11" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "24900000" + '\t' + "29300000" + '\t' + "p21.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chrX" + '\t' + "29300000" + '\t' + "31500000" + '\t' + "p21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "31500000" + '\t' + "37600000" + '\t' + "p21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chrX" + '\t' + "37600000" + '\t' + "42400000" + '\t' + "p11.4" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "42400000" + '\t' + "46400000" + '\t' + "p11.3" + '\t' + "gpos75");
            StandardCytoBands.Add("chrX" + '\t' + "46400000" + '\t' + "49800000" + '\t' + "p11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "49800000" + '\t' + "54800000" + '\t' + "p11.22" + '\t' + "gpos25");
            StandardCytoBands.Add("chrX" + '\t' + "54800000" + '\t' + "58100000" + '\t' + "p11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "58100000" + '\t' + "60600000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chrX" + '\t' + "60600000" + '\t' + "63000000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chrX" + '\t' + "63000000" + '\t' + "64600000" + '\t' + "q11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "64600000" + '\t' + "67800000" + '\t' + "q12" + '\t' + "gpos50");
            StandardCytoBands.Add("chrX" + '\t' + "67800000" + '\t' + "71800000" + '\t' + "q13.1" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "71800000" + '\t' + "73900000" + '\t' + "q13.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chrX" + '\t' + "73900000" + '\t' + "76000000" + '\t' + "q13.3" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "76000000" + '\t' + "84600000" + '\t' + "q21.1" + '\t' + "gpos100");
            StandardCytoBands.Add("chrX" + '\t' + "84600000" + '\t' + "86200000" + '\t' + "q21.2" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "86200000" + '\t' + "91800000" + '\t' + "q21.31" + '\t' + "gpos100");
            StandardCytoBands.Add("chrX" + '\t' + "91800000" + '\t' + "93500000" + '\t' + "q21.32" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "93500000" + '\t' + "98300000" + '\t' + "q21.33" + '\t' + "gpos75");
            StandardCytoBands.Add("chrX" + '\t' + "98300000" + '\t' + "102600000" + '\t' + "q22.1" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "102600000" + '\t' + "103700000" + '\t' + "q22.2" + '\t' + "gpos50");
            StandardCytoBands.Add("chrX" + '\t' + "103700000" + '\t' + "108700000" + '\t' + "q22.3" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "108700000" + '\t' + "116500000" + '\t' + "q23" + '\t' + "gpos75");
            StandardCytoBands.Add("chrX" + '\t' + "116500000" + '\t' + "120900000" + '\t' + "q24" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "120900000" + '\t' + "128700000" + '\t' + "q25" + '\t' + "gpos100");
            StandardCytoBands.Add("chrX" + '\t' + "128700000" + '\t' + "130400000" + '\t' + "q26.1" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "130400000" + '\t' + "133600000" + '\t' + "q26.2" + '\t' + "gpos25");
            StandardCytoBands.Add("chrX" + '\t' + "133600000" + '\t' + "138000000" + '\t' + "q26.3" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "138000000" + '\t' + "140300000" + '\t' + "q27.1" + '\t' + "gpos75");
            StandardCytoBands.Add("chrX" + '\t' + "140300000" + '\t' + "142100000" + '\t' + "q27.2" + '\t' + "gneg");
            StandardCytoBands.Add("chrX" + '\t' + "142100000" + '\t' + "147100000" + '\t' + "q27.3" + '\t' + "gpos100");
            StandardCytoBands.Add("chrX" + '\t' + "147100000" + '\t' + "155270560" + '\t' + "q28" + '\t' + "gneg");
            StandardCytoBands.Add("chrY" + '\t' + "0" + '\t' + "2500000" + '\t' + "p11.32" + '\t' + "gneg");
            StandardCytoBands.Add("chrY" + '\t' + "2500000" + '\t' + "3000000" + '\t' + "p11.31" + '\t' + "gpos50");
            StandardCytoBands.Add("chrY" + '\t' + "3000000" + '\t' + "11600000" + '\t' + "p11.2" + '\t' + "gneg");
            StandardCytoBands.Add("chrY" + '\t' + "11600000" + '\t' + "12500000" + '\t' + "p11.1" + '\t' + "acen");
            StandardCytoBands.Add("chrY" + '\t' + "12500000" + '\t' + "13400000" + '\t' + "q11.1" + '\t' + "acen");
            StandardCytoBands.Add("chrY" + '\t' + "13400000" + '\t' + "15100000" + '\t' + "q11.21" + '\t' + "gneg");
            StandardCytoBands.Add("chrY" + '\t' + "15100000" + '\t' + "19800000" + '\t' + "q11.221" + '\t' + "gpos50");
            StandardCytoBands.Add("chrY" + '\t' + "19800000" + '\t' + "22100000" + '\t' + "q11.222" + '\t' + "gneg");
            StandardCytoBands.Add("chrY" + '\t' + "22100000" + '\t' + "26200000" + '\t' + "q11.223" + '\t' + "gpos50");
            StandardCytoBands.Add("chrY" + '\t' + "26200000" + '\t' + "28800000" + '\t' + "q11.23" + '\t' + "gneg");
            StandardCytoBands.Add("chrY" + '\t' + "28800000" + '\t' + "59373566" + '\t' + "q12" + '\t' + "gvar");

        }

        private void GetCytogeneticData()
        {
            try
            {
                int[] count = new int[26];
                String[] items = null;


                foreach (string line in StandardCytoBands)
                {
                    items = null;
                    items = line.Split('\t');
                    items[0] = items[0].Substring(3);

                    if (items.GetUpperBound(0) >= 3)
                    {
                        try
                        {
                            switch (items[0].ToLower())
                            {
                                case "x":
                                    count[23]++;
                                    break;
                                case "y":
                                    count[24]++;
                                    break;
                                case "m":
                                    count[25]++;
                                    break;
                                default:
                                    count[Convert.ToInt32(items[0])]++;
                                    break;
                            }
                        }
                        catch
                        { }
                    }
                }

                for (int i = 1; i < 26; i++)
                {
                    CytoBands[i] = null;
                    CytoBands[i] = new CytoGenetic[count[i]];
                }

                count = new int[25];
                int number = 0;

                foreach (string line in StandardCytoBands)
                {
                    items = null;
                    items = line.Split('\t');
                    items[0] = items[0].Substring(3);
                    if (items.GetUpperBound(0) >= 3)
                    {
                        try
                        {
                            switch (items[0].ToLower())
                            {
                                case "x":
                                    CytoBands[23][count[23]] = new CytoGenetic(items);
                                    count[23] += 1;
                                    break;
                                case "y":
                                    CytoBands[24][count[24]] = new CytoGenetic(items);
                                    count[24] += 1;
                                    break;
                                case "m":
                                    CytoBands[25][count[25]] = new CytoGenetic(items);
                                    count[25] += 1;
                                    break;
                                default:
                                    number = Convert.ToInt32(items[0]);
                                    CytoBands[number][count[number]] = new CytoGenetic(items);
                                    count[number] += 1;
                                    break;
                            }
                        }
                        catch { }
                    }
                }
                SortTheCytogeneticPlaceData();
            }
            catch
            { }
        }

        private void SortTheCytogeneticPlaceData()
        {
            for (int i = 1; i < 26; i++)
            { if (CytoBands[i] != null && CytoBands[i].Length > 1) { Array.Sort(CytoBands[i], new CytoSort()); } }
        }

        private void getRegions(bool IgnoreRSField, bool VCFGenotypes)
        {
            Dictionary<string, List<DNARegion>> data = null;
            if (System.IO.File.Exists(MultiText) == true)
            { data = getRegionsFromMultiTextFile(); }

            string theTitle = Text;
            int index1 = 0;
            foreach (string aFile in Files)
            {
                try
                {
                    Text = aFile.Substring(aFile.LastIndexOf("\\") + 1);
                    Application.DoEvents();
                    if (System.IO.File.Exists(aFile) == true)
                    {
                        RegionIdentification ri = new RegionIdentification(aFile, false, IgnoreRSField, VCFGenotypes);
                        SNPRegions[index1] = ri.TheRegions;

                        ri = null;
                    }
                    else
                    {
                        string key = aFile.Substring(aFile.LastIndexOf('\\') + 1);
                        if (data.ContainsKey(key) == true)
                        { SNPRegions[index1] = data[key]; }
                    }

                    PlaceImage(Draw(1), true);
                    Application.DoEvents();
                    Text = theTitle;
                    index1++;
                }
                catch (Exception ex)
                { }
                finally
                {
                    Text = theTitle;
                }
            }
        }

        private void PlaceImage(Bitmap btm, bool resetScrollbar)
        {
            if (resetScrollbar == true)
            { modifyScrollBarValues(btm.Height); }
            if (btm.Height < p1.Height)
            { p1.Image = btm; }
            else
            {
                Rectangle area;
                int h = btm.Height - p1.Height - vScrollBar1.Value;
                if (h < 0)
                { area = new Rectangle(0, vScrollBar1.Value + h, 752, p1.Height); }
                else
                { area = new Rectangle(0, vScrollBar1.Value, 752, p1.Height); }
                Bitmap btmShow = btm.Clone(area, btm.PixelFormat);

                p1.Image = btmShow;
            }
        }

        private Bitmap Draw(int multiple)
        {
            int twenty = 20 * multiple;
            int ten = 10 * multiple;
            int twentyfive = 25 * multiple;
            int forty = 40 * multiple;
            int two = 2 * multiple;
            int seventeen = 17 * multiple;
            int thirteen = 13 * multiple;
            int fifteen = 15 * multiple;
            int one = 1 * multiple;
            int eighteen = 18 * multiple;
            int four = 4 * multiple;
            int twentytwo = 22 * multiple;
            int TheHeight = two * fatness;
            int gapScaled = two * gap;
            int bit = (SNPRegions.Length * TheHeight + gapScaled) - ten;
            if (bit < 0) { bit = 0; }
            int step = twentyfive + bit + four;           
                       

            int offset = step + forty + (gap * SNPRegions.Length);

            Size currentSize = new Size(p1.Width * multiple, offset);
            Bitmap btm = new Bitmap(currentSize.Width, currentSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            if (multiple == 4)
            { btm.SetResolution(300.0F, 300.0F); }
            
            Graphics g = Graphics.FromImage(btm);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.White);
            if (multiple == 1)
            { g.DrawRectangle(Pens.Black, 0, 0, btm.Width - 1, btm.Height - 1); }
            double widthScale;
            if (scaleToChromosomeOne == true)
            { widthScale = (Single)((Single)chromosomelength[currentChromosome - 1] / (currentSize.Width - forty)); }
            else
            { widthScale = (Single)((Single)chromosomelength[0] / ((currentSize.Width - forty))); }
                offset = 0;
            Font f = null;
            f = new Font(System.Drawing.FontFamily.GenericSansSerif, 8, FontStyle.Bold);

            Brush Homozygousbrush = new SolidBrush(Color.Pink);
            SolidBrush colourAffected = new SolidBrush(AffectedColour);
            SolidBrush colourUnaffected = new SolidBrush(UnaffectedColour); ;
            SolidBrush colourCommon = new SolidBrush(CommonRegionsCommon); ;

            //Draw chromosome
            int cl = Convert.ToInt32(chromosomelength[currentChromosome - 1] / widthScale);
            g.DrawString(currentChromosome.ToString(), f, Brushes.Black, two, offset + seventeen);
            if (scaleToChromosomeOne == true)
            { DrawCytobandsImage(g, currentChromosome, 0, chromosomelength[currentChromosome - 1], twenty + offset, (float)widthScale, multiple); }
            else
            { DrawCytobandsImage(g, currentChromosome, 0, 247249719, twenty + offset, (float)widthScale, multiple); }
                       
            //Draw common regions
            int startHeight = thirteen; offset = 0;
            if (MinimumSNPRegions != null)
            {
                foreach (DNARegion r in MinimumSNPRegions)
                {
                    if (r.Chromosome == currentChromosome)
                    {
                        int drawFrom = startHeight;
                        g.FillRectangle(colourCommon, (Single)(r.StartPoint / widthScale) + twenty, drawFrom + eighteen, (Single)(r.Length / widthScale), (SNPRegions.Length * (gap + TheHeight)) ) ;// step - fifteen);
                    }
                }
                startHeight += one;
                offset += step;
            }

            //Draw variant
            if (object.ReferenceEquals(Gene, null) != true  && Gene.Chromosome==currentChromosome)
            {
                int drawFrom = startHeight;
                g.FillRectangle(colourAffected, (Single)(Gene.StartPoint / widthScale) + twenty, twenty + drawFrom, one, step - ten);// fifteen);
            }

            //Draw individual's regions
            startHeight = thirteen;
            offset = 0;
            float[] dashValues = { 1, 10 };
            Pen dotdash = new Pen(Color.Black, one);
            dotdash.DashPattern = dashValues;
            for (int i = 0; i < SNPRegions.Length; i++)
            {
                if (SNPRegions[i] != null)
                {
                    
                    int drawFrom = startHeight;
                    if (showDashedLine == true)
                    { g.DrawLine(dotdash, (float)twenty, (float)(twenty + drawFrom + ((float)TheHeight / 2)), (float)(currentSize.Width - twenty), (float)(twenty + drawFrom + ((float)TheHeight / 2))); }
                    
                    foreach (DNARegion r in SNPRegions[i])
                    {
                        drawFrom = startHeight;
                        if (r.Chromosome == currentChromosome)
                        {
                            if (Affecteds[i] == true)
                            { g.FillRectangle(colourAffected, (Single)(r.StartPoint / widthScale) + twenty, twenty + drawFrom, (Single)(r.Length / widthScale), TheHeight); }
                            else
                            { g.FillRectangle(colourUnaffected, (Single)(r.StartPoint / widthScale) + twenty, twenty + drawFrom, (Single)(r.Length / widthScale), TheHeight); }
                            if (TheHeight > 5)
                            { g.DrawRectangle(Pens.Black, (Single)(r.StartPoint / widthScale) + twenty, twenty + drawFrom, (Single)(r.Length / widthScale), TheHeight); }
                        }
                    }
                    startHeight += TheHeight + gap;
                    offset += step;                     
                }
            }
            //Draw X-axis at bottom of image
            int increment = (int)(250000000 / 1000000);
            int axisStep = 10;
            if (scaleToChromosomeOne==true)
            {
                increment = (int)(chromosomelength[currentChromosome - 1] / 1000000);
                if (increment < 126) { axisStep = 5; }
            }
            else
            {
                increment = (int)(chromosomelength[0] / 1000000);
                axisStep = 10;
            }
            if (increment >= 300) { increment /= 10; }
            offset = step + four + (gap * SNPRegions.Length);
            Single value = (Single)twenty;
            if (value <= currentSize.Width - twenty)
            {
                g.DrawLine(Pens.Black, value, offset + fifteen, value, offset + eighteen);
                g.DrawString("0", f, Brushes.Black, value - (one * 6), offset + twentytwo);
            }
            for (int index = axisStep; index < increment + 1; index += axisStep)
            {
                value = (Single)(twenty + ((currentSize.Width - forty) * ((Single)index / increment)));
                if (value <= currentSize.Width - 20)
                {
                    g.DrawLine(Pens.Black, value, offset + fifteen, value, offset + eighteen);
                    g.DrawString(index.ToString(), f, Brushes.Black, value - (one * 6), offset + twentytwo);
                }
            }
            g.DrawLine(Pens.Black, twenty, offset + fifteen, currentSize.Width - twenty, offset + fifteen);

            if (multiple == 1)
            {
                gbtm = btm;
            }
            return btm;
        }

        private void DrawCytobandsImage(Graphics g, int chromosome, int StartPoint, int EndPoint, int height, Single scaleHorizontal, int multiple)
        {
            Point offset = new Point(StartPoint, 0);         
            bool odd = false;
            if (CytoBands[chromosome] != null)
            {
                offset.Y = height;
                for (int index = 0; index < CytoBands[chromosome].Length; index++)
                {
                    odd = !odd;
                    CytoBands[chromosome][index].Draw(g, scaleHorizontal, offset, odd, multiple);
                }
            }
        }
        
        private void modifyScrollBarValues(int height)
        {
            int scrolmax = height + 10 - p1.Height;
            if (scrolmax < 0)
            {
                vScrollBar1.Value = 0;
                vScrollBar1.Maximum = 0;
            }
            else
            {
                vScrollBar1.Value = 0;
                vScrollBar1.Maximum = scrolmax;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            PlaceImage(gbtm, false);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentChromosome = cboChromosome.SelectedIndex + 1;
            Text = "Chromosome:" + currentChromosome.ToString();
            PlaceImage(Draw(1), true);
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = FileAccessClass.FileString(FileAccessClass.FileJob.SaveAs, "Save image as", "*.tif|*.tif");
            if (file.Equals("cancel") == true) { return; }
            Bitmap savebtm = Draw(4);
            savebtm.Save(file);
        }

        private void exportIntervalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportIntervals(ExportThis.Long);
        }

        private void exportIntervalsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            exportIntervals(ExportThis.Short);
        }

        private void exportCommonIntervalsOnlyshortFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportIntervals(ExportThis.Common);
        }

        private void exportIntervals(ExportThis ExportValue)
        {
            string filename = FileAccessClass.FileString(FileAccessClass.FileJob.SaveAs, "Save the autozygous intervals", "Tab delimited (*.txt|*.txt");
            if (filename.Equals("Cancel") == true || string.IsNullOrEmpty(filename) == true) { return; }

            System.IO.StreamWriter fw = null;
            try
            {

                fw = new System.IO.StreamWriter(filename);

                for (int index = 0; index < FileNames.Length; index++)
                {

                    if (ExportValue != ExportThis.Common)
                    { fw.WriteLine(FileNames[index]); }
                    if (SNPRegions[index] != null)
                    {
                        if (ExportValue == ExportThis.Short)
                        {
                            foreach (DNARegion line in SNPRegions[index])
                            { fw.WriteLine(line.ToStringTerse()); }
                        }
                        else if (ExportValue == ExportThis.Long)
                        {
                            fw.WriteLine("Chromosome\tStart\tEnd\tLength");
                            foreach (DNARegion line in SNPRegions[index])
                            { fw.WriteLine(line.ToString()); }
                        }
                    }
                    else { fw.WriteLine("No data"); }
                }


                if (MinimumSNPRegions != null)
                {
                    if (ExportValue != ExportThis.Common)
                    { fw.WriteLine("Common regions among affected patients:"); }
                    if (ExportValue == ExportThis.Short)
                    {
                        foreach (DNARegion line in MinimumSNPRegions)
                        { fw.WriteLine(line.ToStringTerse()); }
                    }
                    else if (ExportValue == ExportThis.Long)
                    {
                        fw.WriteLine("Chromosome\tStart\tEnd\tLength");
                        foreach (DNARegion line in MinimumSNPRegions)
                        { fw.WriteLine(line.ToString()); }
                    }
                    else if (ExportValue == ExportThis.Common)
                    {
                        foreach (DNARegion line in MinimumSNPRegions)
                        { fw.WriteLine(line.ToStringTerse()); }
                    }
                }
                else { fw.WriteLine("No common regions among affected patients"); }

            }
            catch
            { }
            finally
            {
                if (fw != null) { fw.Close(); }

            }
        }

        private void affectedAutozygousRegionColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = AffectedColour;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                AffectedColour = colorDialog1.Color;
                PlaceImage(Draw(1), false);
            }
        }

        private void commonAffectedAutozygousRegionColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = CommonRegionsCommon;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                CommonRegionsCommon = colorDialog1.Color;
                PlaceImage(Draw(1), false);
            }
        }

        private void unaffectedAutozygousRegionColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = UnaffectedColour;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                UnaffectedColour = colorDialog1.Color;
                PlaceImage(Draw(1), false);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AffectedColour = Color.Blue;
            UnaffectedColour = Color.Red;
            CommonRegionsCommon = Color.LightBlue;
            Gene = null;
            gap = 1;
            fatness = 1;
            PlaceImage(Draw(1), false);
        }

        private void addVariantPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenePosition gp = new GenePosition(Gene);
            if (gp.ShowDialog() == DialogResult.OK)
            {
                Gene = gp.getGene();
                PlaceImage(Draw(1), false);
            }
        }
        
        private void makeGapBetweenAutozygouseRegionsNarrowerToolStripMenuItem_Click(object sender, EventArgs e)
        {   
            gap--;
            if (gap == 0)
            { gap = 1; }
            PlaceImage(Draw(1), true);
            
            
        }

        private void makeGapBetweenAutozygouseRegionsBiggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gap++;            
            PlaceImage(Draw(1), true);            
        }

        private void makeAutozygousRegionsThinnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fatness--;
            if (fatness == 0)
            { fatness = 1;  }
            PlaceImage(Draw(1), true);
        }

        private void makeAutozygousRegionsDeeperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fatness++;
            PlaceImage(Draw(1), true);
        }

        private void hideDottedLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem s = (ToolStripMenuItem)sender;
            if (showDashedLine == true)
            {
                showDashedLine = false;
                s.Text = "Show dotted lines";
            }
            else
            {
                showDashedLine = true;
                s.Text = "Hide dotted lines";
            }
            PlaceImage(Draw(1), false);
        }

        private void scaleToChromosome1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scaleToChromosomeOne = !scaleToChromosomeOne;
            PlaceImage(Draw(1), false);
        }
    }
}
