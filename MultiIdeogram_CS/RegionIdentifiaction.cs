using System;
using System.Collections.Generic;
using System.Text;

namespace  MultiIdeogram_CS
{
    class RegionIdentification
    {
        private List<DNARegion> theRegions = null;
        private string DataFile = null;
        private int RunsCutOff = 80;
        private int ExclusionCutOff = 600;
        private bool HarshGenotyping;
        private int minimumReadDepth =200;
        private int StartRunsCutOff = 100;
        private int StartExclusionCutOff = 600;
        private int iterationCount = 3;
        private SeqSNP[][] c1to25 = null;
        private int MaximumLength = 0;
        private int readDepthCutOff = 5;
        private float fraction = 0.32f;

        private struct AffyIndexes
        {
            public bool OK;
            public int RSIndex;
            public int GenotypeIndex;
            public int ChromosomeIndex;
            public int Position;
        }

        public RegionIdentification(string theFile, bool UseHarshGenotyping, bool IgnoreRSField, bool VCFGenotypes)
        {

            if (theFile.ToLower().Substring(theFile.LastIndexOf(".")).Equals(".xls") == true)
            { RegionIdenificationAffyXLS(theFile); }
            if (theFile.ToLower().Substring(theFile.LastIndexOf(".")).Equals(".txt") == true)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(theFile);
                if (fi.Length > 1000000)
                {
                    StartRunsCutOff = 386;
                    StartExclusionCutOff = 575;
                    RegionIdenificationAffyTXT(theFile);
                }
                else
                { theRegions = RegionIdentificationUserTXT(theFile); }
            }
            else if (theFile.Length > 6 && theFile.ToLower().Substring(theFile.Length - 6).Equals(".g.vcf") == true)
            {
                StartRunsCutOff = 386;
                StartExclusionCutOff = 575;
                readDepthCutOff = 5;
                RegionIdenificationVCF(theFile, readDepthCutOff, UseHarshGenotyping, true, false, VCFGenotypes);
            }
            else if (theFile.Length > 9 && theFile.ToLower().Substring(theFile.Length - 9).Equals(".g.vcf.gz") == true)
            {
                StartRunsCutOff = 386;
                StartExclusionCutOff = 575;
                readDepthCutOff = 5;
                RegionIdenificationVCF(theFile, readDepthCutOff, UseHarshGenotyping, true, false, VCFGenotypes);
            }
            else if (theFile.Length > 9 && theFile.ToLower().Substring(theFile.Length - 7).Equals(".vcf.gz") == true)
            {
                StartRunsCutOff = 386;
                StartExclusionCutOff = 575;
                readDepthCutOff = 5;
                RegionIdenificationVCF(theFile, readDepthCutOff, UseHarshGenotyping, false, IgnoreRSField, VCFGenotypes);
            }
            else if (theFile.ToLower().Substring(theFile.LastIndexOf(".")).Equals(".vcf") == true)
            {
                StartRunsCutOff = 386;
                StartExclusionCutOff = 575;
                readDepthCutOff = 5;
                RegionIdenificationVCF(theFile, readDepthCutOff, UseHarshGenotyping, false, IgnoreRSField, VCFGenotypes);
            }

        }

        private void RegionIdenificationAffyXLS(string theFile)
        {
            try
            {

                c1to25 = new SeqSNP[26][];
                DataFile = theFile;

                if (AddFileAffyXLS() == -1)
                { return; }

                if (SetConstants() == -1)
                { return; }

                for (int index = 0; index <= 4; index++)
                {
                    if (HomozygousRun(false) == -1)
                    { return; }

                    if (CleanUp() == -1)
                    { return; }
                }

                if (HomozygousRun(true) == -1)
                { return; }

                if (GetMaximumLength() == -1)
                { return; }

                if (setClusters() == -1)
                { return; }

                if (SetIBDValues() == -1)
                { return; }

                if (SetConstants() == -1)
                { return; }

                theRegions = WriteNewDescription();

            }
            catch
            { }

        }

        private void RegionIdenificationAffyTXT(string theFile)
        {

            try
            {

                c1to25 = new SeqSNP[26][];
                DataFile = theFile;

                if (AddFileAffyTXT() == -1)
                { return; }

                if (SetConstants() == -1)
                { return; }

                for (int index = 0; index <= 4; index++)
                {
                    if (HomozygousRun(false) == -1)
                    { return; }

                    if (CleanUp() == -1)
                    { return; }
                }

                if (HomozygousRun(true) == -1)
                { return; }

                if (GetMaximumLength() == -1)
                { return; }

                if (setClusters() == -1)
                { return; }

                if (SetIBDValues() == -1)
                { return; }

                if (SetConstants() == -1)
                { return; }

                theRegions = WriteNewDescription();

            }
            catch
            { }

        }

        private void RegionIdenificationVCF(string theFile, int theMinimumReadDepth, bool UseHarshGenotyping, bool isGVCF, bool IgnoreRSField, bool VCFGenotypes)
        {

            try
            {
                c1to25 = new SeqSNP[26][];
                theMinimumReadDepth = 5;
                DataFile = theFile;
                HarshGenotyping = UseHarshGenotyping;
                minimumReadDepth = theMinimumReadDepth;

                if (AddFileVCF(isGVCF, IgnoreRSField, VCFGenotypes) == -1)
                { return; }

                if (SetConstants() == -1)
                { return; }

                for (int index = 0; index <= iterationCount; index++)
                {
                    if (HomozygousRun(false) == -1)
                    { return; }

                    if (CleanUp() == -1)
                    { return; }
                }

                if (HomozygousRun(true) == -1)
                { return; }

                if (GetMaximumLength() == -1)
                { return; }

                if (setClusters() == -1)
                { return; }

                if (SetIBDValues() == -1)
                { return; }

                if (SetConstants() == -1)
                { return; }

                theRegions = WriteNewDescription();

            }
            catch
            { }

        }

        private List<DNARegion> RegionIdentificationUserTXT(string theFile)
        {
            List<DNARegion> answer = null;
            System.IO.StreamReader fs = null;
            try
            {
                List<DNARegion> theseRegions = new List<DNARegion>();
                fs = new System.IO.StreamReader(theFile);
                while (fs.Peek() >0)
                {
                    string[] items = fs.ReadLine().Split('\t');
                    if (items.Length == 3 || items.Length==4)
                    {
                        if (items[0].ToLower().StartsWith("chr"))
                        { items[0] = items[0].Substring(3); }
                        if (char.IsDigit(items[0][0]) == true)
                        {
                            int Chromosome = Convert.ToInt32(items[0]);
                            int startPoint = Convert.ToInt32(items[1]);
                            int endPoint = Convert.ToInt32(items[2]);
                            DNARegion interval = new DNARegion(Chromosome, startPoint, endPoint);
                            theseRegions.Add(interval);
                        }
                    }
                }               
               answer = condenseList(theseRegions);
            }
            catch
            { return answer; }
            finally
            {
                if (fs != null)
                { fs.Close(); }
            }

            return answer;
        }

        public List<DNARegion> TheRegions { get { return theRegions; } }

        private int AddFileAffyXLS()
            {
            int result = -1;
            System.IO.StreamReader fs1 = null;

            try
                {
                AffyIndexes indexes = getIndexes(DataFile);
                if (indexes.OK == false) { return -1; }
                string chrString = null;
                string line = null;
                string[] items = null;

                int[] count = new int[26];
                fs1 = new System.IO.StreamReader(DataFile);
                if (fs1.Peek() > 0) { fs1.ReadLine(); }

                while (fs1.Peek() > 0)
                {
                    line = fs1.ReadLine();
                    items = line.Split('\t');
                    if (items.Length > 3)
                    {
                        if (items[indexes.RSIndex] != "" && items[indexes.GenotypeIndex].ToUpper() != "NOCALL" && Convert.ToSingle(items[indexes.Position]) != 0)
                        {
                            switch (items[indexes.ChromosomeIndex].ToLower())
                            {
                                case "x":
                                    count[23]++;
                                    break;
                                case "y":
                                    count[24]++;
                                    break;
                                case "mt":                                    
                                    break;
                                default:
                                    try
                                    {
                                        chrString = items[indexes.ChromosomeIndex];
                                        count[Convert.ToInt32(chrString)]++;
                                    }
                                    catch
                                    { }
                                    break;
                            }
                        }
                    }
                }

                fs1.Close();
                ReDimArray(count);
                count = new int[26];
                fs1 = new System.IO.StreamReader(DataFile);
                SeqSNP dataSNP = default(SeqSNP);
                int chromosome = 0;
                int positions = 0;

                if (fs1.Peek() >0) {fs1.ReadLine();}

                while (fs1.Peek() > 0)
                {
                    line = fs1.ReadLine();
                    items = line.Split('\t');

                    if (items.Length > 3)
                    {
                        if (items[indexes.RSIndex] != "" && items[indexes.GenotypeIndex].ToUpper() != "NOCALL" && Convert.ToSingle(items[indexes.Position]) != 0)
                        {
                            switch (items[indexes.ChromosomeIndex].ToLower())
                            {
                                case "x":
                                    positions = Convert.ToInt32(items[indexes.Position]);

                                    dataSNP = new SeqSNP(positions, items[indexes.RSIndex], items[indexes.GenotypeIndex]);

                                    AddtoArray(23, dataSNP, count[23]);
                                    count[23] += 1;
                                    break;
                                case "y":
                                    positions = Convert.ToInt32(items[indexes.Position]);

                                    dataSNP = new SeqSNP(positions, items[indexes.RSIndex], items[indexes.GenotypeIndex]);

                                    AddtoArray(24, dataSNP, count[24]);
                                    count[24] += 1;
                                    break;
                                case "mt":
                                    break;
                                default:
                                    try
                                    {
                                        chrString = items[indexes.ChromosomeIndex];

                                        chromosome = Convert.ToInt32(chrString);
                                        positions = Convert.ToInt32(items[indexes.Position]);

                                        dataSNP = new SeqSNP(positions, items[indexes.RSIndex], items[indexes.GenotypeIndex]);

                                        AddtoArray(chromosome, dataSNP, count[chromosome]);
                                        count[chromosome] += 1;
                                    }
                                    catch
                                    { }
                                    break;
                            }
                        }
                        else 
                        { }
                    }
                }

                DataArraySort(count);

                result = 1;
                }
            catch 
                {
                result = -1;
                }
            finally
                {
                if (fs1 != null)
                { fs1.Close(); }
                }

            return result;

            }

        private AffyIndexes getIndexes(string DataFile)
            {
            AffyIndexes result = new AffyIndexes();
            int answer = 1;
            int intStrArray = -1;
            string[] strRow;
            string[] strDataArray = new string[3];

            result.ChromosomeIndex = -1;
            result.GenotypeIndex = -1;
            result.Position = -1;
            result.RSIndex = -1;
            System.IO.StreamReader fs = null;

            try
                {
                fs = new System.IO.StreamReader(DataFile);

                strDataArray[0] = fs.ReadLine();
                strDataArray[1] = fs.ReadLine();
                strDataArray[2] = fs.ReadLine();
                fs.Close();
                strRow = strDataArray[0].Split('\t');

                for (int index = 0; index < strDataArray.Length; index++)
                    {
                    for (intStrArray = 0; intStrArray < strRow.Length; intStrArray++)
                        {
                        if (strRow[intStrArray].Trim().ToUpper().Equals("Chromosome".ToUpper()) == true)
                        { result.ChromosomeIndex = intStrArray; }
                        else if (strRow[intStrArray].Trim().ToUpper().Equals("Physical Position".ToUpper()))
                        { result.Position = intStrArray; }
                        else if (strRow[intStrArray].Trim().ToUpper().Equals("dbSNP RS ID".ToUpper()) == true)
                        { result.RSIndex = intStrArray; }
                        else if (strRow[intStrArray].Trim().ToUpper().IndexOf("CALL") > -1 && (strRow[intStrArray].Trim().ToUpper().IndexOf("ZONE") == -1))
                        { result.GenotypeIndex = intStrArray; }
                        }
                    }

                if (result.ChromosomeIndex == -1) { answer = -1; }
                if (result.Position == -1) { answer = -1; }
                if (result.RSIndex == -1) { answer = -1; }
                if (result.GenotypeIndex == -1) { answer = -1; }

                if (answer == 1)
                    { result.OK = true;}
                }
            catch (Exception ex)
            { answer = -1; }
            finally
            { if (fs != null) { fs.Close(); } }
            
            return result;

            }

        private int AddFileAffyTXT()
            {
            int result = -1;
            System.IO.StreamReader fs1 = null;

            try
                {
                AffyIndexes indexes = getIndexesTXT(DataFile);
                if (indexes.OK == false) { return -1; }
                string chrString = null;
                string line = null;
                string[] items = null;

                int[] count = new int[26];
                fs1 = new System.IO.StreamReader(DataFile);
                if (fs1.Peek() > 0) { fs1.ReadLine(); }

                while (fs1.Peek() > 0)
                {
                    line = fs1.ReadLine();
                    items = line.Split('\t');
                    if (items.Length > 3)
                    {
                        if (items[indexes.ChromosomeIndex].ToLower() != "chromosome"
                            && items[indexes.ChromosomeIndex].ToLower() != "---" 
                            && items[indexes.RSIndex] != "" 
                            && items[indexes.GenotypeIndex].ToUpper() != "NOCALL" 
                            && Convert.ToSingle(items[indexes.Position]) != 0)
                        {
                            switch (items[indexes.ChromosomeIndex].ToLower())
                            {
                                case "x":
                                    count[23]++;
                                    break;
                                case "y":
                                    count[24]++;
                                    break;
                                default:
                                    try
                                    {
                                        chrString = items[indexes.ChromosomeIndex];
                                        if (Char.IsDigit(chrString[0]) == true)
                                        {
                                            count[Convert.ToInt32(chrString)]++;
                                        }
                                    }
                                    catch
                                    { }
                                    break;
                            }
                        }
                    }
                }

                fs1.Close();
                ReDimArray(count);
                count = new int[26];
                fs1 = new System.IO.StreamReader(DataFile);
                SeqSNP dataSNP = default(SeqSNP);
                int chromosome = 0;
                int positions = 0;

                if (fs1.Peek() >0) {fs1.ReadLine();}

                while (fs1.Peek() > 0)
                {
                    line = fs1.ReadLine();
                    items = line.Split('\t');

                    if (items.Length > 3)
                    {
                        if (items[indexes.ChromosomeIndex].ToLower() != "chromosome"
                            && items[indexes.ChromosomeIndex].ToLower() != "---" 
                            && items[indexes.RSIndex] != "" 
                            && items[indexes.GenotypeIndex].ToUpper() != "NOCALL" 
                            && Convert.ToSingle(items[indexes.Position]) != 0)
                        {
                            switch (items[indexes.ChromosomeIndex].ToLower())
                            {
                                case "x":
                                    positions = Convert.ToInt32(items[indexes.Position]);

                                    dataSNP = new SeqSNP(positions, items[indexes.RSIndex], items[indexes.GenotypeIndex]);

                                    AddtoArray(23, dataSNP, count[23]);
                                    count[23] += 1;
                                    break;
                                case "y":
                                    positions = Convert.ToInt32(items[indexes.Position]);

                                    dataSNP = new SeqSNP(positions, items[indexes.RSIndex], items[indexes.GenotypeIndex]);

                                    AddtoArray(24, dataSNP, count[24]);
                                    count[24] += 1;
                                    break;
                                default:
                                    try
                                    {
                                        
                                        chrString = items[indexes.ChromosomeIndex];
                                        if (Char.IsDigit(chrString[0]) == true)
                                        {
                                            chromosome = Convert.ToInt32(chrString);
                                            positions = Convert.ToInt32(items[indexes.Position]);

                                            dataSNP = new SeqSNP(positions, items[indexes.RSIndex], items[indexes.GenotypeIndex]);

                                            AddtoArray(chromosome, dataSNP, count[chromosome]);
                                            count[chromosome] += 1;
                                        }
                                    }
                                    catch
                                    { }
                                    break;
                            }
                        }
                        else 
                        { }
                    }
                }

                DataArraySort(count);

                result = 1;
                }
            catch 
                {
                result = -1;
                }
            finally
                {
                if (fs1 != null)
                { fs1.Close(); }
                }

            return result;

            }

        private AffyIndexes getIndexesTXT(string DataFile)
        {
            AffyIndexes result = new AffyIndexes();
            int answer = 1;
            int intStrArray = -1;
            string[] strRow;
            string strDataArray;

            result.ChromosomeIndex = -1;
            result.GenotypeIndex = -1;
            result.Position = -1;
            result.RSIndex = -1;
            System.IO.StreamReader fs = null;

            try
            {
                fs = new System.IO.StreamReader(DataFile);


                for (int index = 0; index < 10; index++)
                {
                    if (fs.Peek() > 0)
                    {
                        strDataArray = fs.ReadLine();
                        strRow = strDataArray.Split('\t');

                        for (intStrArray = 0; intStrArray < strRow.Length; intStrArray++)
                        {
                            if (strRow[intStrArray].Trim().ToUpper().Equals("Chromosome".ToUpper()) == true)
                            { result.ChromosomeIndex = intStrArray; }
                            else if (strRow[intStrArray].Trim().ToUpper().Equals("Chromosomal Position".ToUpper()))
                            { result.Position = intStrArray; }
                            else if (strRow[intStrArray].Trim().ToUpper().Equals("dbSNP RS ID".ToUpper()) == true)
                            { result.RSIndex = intStrArray; }
                            else if (strRow[intStrArray].Trim().ToUpper().IndexOf("CALL") > -1)
                            { result.GenotypeIndex = intStrArray; }
                        }
                    }
                }

                if (result.ChromosomeIndex == -1) { answer = -1; }
                if (result.Position == -1) { answer = -1; }
                if (result.RSIndex == -1) { answer = -1; }
                if (result.GenotypeIndex == -1) { answer = -1; }

                if (answer == 1)
                { result.OK = true; }
            }
            catch (Exception ex)
            { answer = -1; }
            finally
            { if (fs != null) { fs.Close(); } }

            return result;

        }
        
        private int AddFileVCF(bool isGVCF, bool IgnoreRSField, bool VCFGenotypes)
        {
            int cutOffReadDepth = 99;
            int result = -1;
            
            try
            {
                VCFRegions dataSource = new VCFRegions();

                if (string.IsNullOrEmpty(this.DataFile))
                { throw new Exception(); }
                string answer = dataSource.GetData(this.DataFile, IgnoreRSField, readDepthCutOff, isGVCF, VCFGenotypes); 
                if (string.IsNullOrEmpty(answer) == false)
                { throw new Exception(); }

                SeqVariant[] theVariants = dataSource.getVariantArray;

                SeqSNP dataSNP = default(SeqSNP);

                int[] iCs = new int[26];

                bool Dilute = false;
                if (theVariants.Length > 100000)
                {
                    Dilute = true;
                    for (int index = 0; index < theVariants.Length; index++)
                    {
                        if (theVariants[index].ReadDepth > cutOffReadDepth)
                        { iCs[theVariants[index].Chromosome] += 1; }
                    }
                }
                else
                {
                    for (int index = 0; index < theVariants.Length; index++)
                    { iCs[theVariants[index].Chromosome] += 1; }
                }

                ReDimArray(iCs);
                iCs = new int[26];

                string RS = null;
                foreach (SeqVariant v in theVariants)
                {
                    if (Dilute == false || v.ReadDepth > cutOffReadDepth)
                    {
                        if (v.Chromosome < 23)
                        {
                            RS = v.Name;
                            if (RS.Length < 3)
                            { RS = v.ChromosomeString + ":" + v.Position.ToString(); }
                            else
                            {
                                do
                                {
                                    RS = RS.Replace("RS0", "RS").Trim();
                                } while (!(RS.IndexOf("RS0") == -1));
                            }
                            dataSNP = new SeqSNP(v.Position, RS, v.Genotype(minimumReadDepth, HarshGenotyping));

                            AddtoArray(v.Chromosome, dataSNP, iCs[v.Chromosome]);
                            iCs[v.Chromosome] += 1;
                        }
                    }
                }

                DataArraySort(iCs);

                result = 1;

            }
            catch (Exception ex1)
            {
                result = -1;
            }
            finally
            {
                
            }

            return result;
        }
   
        private void ReDimArray(int[] iCs)
        {
            for (int index = 0; index < iCs.Length; index++)
            {
                c1to25[index] = new SeqSNP[iCs[index]];
            }
        }

        private void DataArraySort(int[] iCs)
        {
            SeqSNPSorter sss = new SeqSNPSorter();
            for (int index = 0; index < iCs.Length; index++)
            {
                Array.Sort(c1to25[index], sss);
            }
        }

        private void AddtoArray(int Chromosome, SeqSNP aSeqSNP, int index)
        {
            c1to25[Chromosome][index] = aSeqSNP;
        }

        private int SetConstants()
        {
            int answer = -1;
            try
            {
                int Number = c1to25[1].Length;

                RunsCutOff = Convert.ToInt32(StartRunsCutOff * Number / 70700f);
                ExclusionCutOff = Convert.ToInt32(StartExclusionCutOff * Number / 70700f);
                //RunsCutOff = 50;            

                answer = 1;
            }
            catch (Exception ex)
            { answer = -1; }

            return answer;
        }

        private int HomozygousRun(bool Clean)
        {
            int Run = 0;
            int answer = 1;

            try
            {
                for (int chromosome = 1; chromosome <= 22; chromosome++)
                {
                    if (Clean == true)
                    {
                        for (int index = 0; index <= c1to25[chromosome].GetUpperBound(0); index++)
                        {
                            c1to25[chromosome][index].ResetHomozygousRuns();
                        }
                    }

                    Run = 0;
                    for (int index = 0; index <= c1to25[chromosome].GetUpperBound(0); index++)
                    {
                         Run = c1to25[chromosome][index].HomozygousRuns(Run);
                    }

                    Run = 0;
                    for (int index = c1to25[chromosome].GetUpperBound(0); index >= 0; index += -1)
                    {
                        Run = c1to25[chromosome][index].MaximumRun(Run);
                    }

                    Run = 0;
                    if (RunsCutOff > 20)
                    {
                        for (int index = 1; index <= c1to25[chromosome].GetUpperBound(0) - 1; index++)
                        {
                            c1to25[chromosome][index].WrongCall(c1to25[chromosome][index - 1].MyRuns, c1to25[chromosome][index + 1].MyRuns, RunsCutOff);
                        }
                    }
                    else
                    {
                        for (int index = 1; index <= c1to25[chromosome].GetUpperBound(0) - 1; index++)
                        {
                            c1to25[chromosome][index].WrongCallHarsh(c1to25[chromosome][index - 1].MyRuns, c1to25[chromosome][index + 1].MyRuns, RunsCutOff, fraction);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                answer = -1;
            }

            return answer;
        }
     
        private int CleanUp()
        {
            int answer = -1;

            try
            {
                SeqSNP[] cleanedSNPs = null;
                int count = 0;
                for (int c = 1; c <= 22; c++)
                {

                    count = 0;
                    for (int index = 0; index <= c1to25[c].GetUpperBound(0); index++)
                    {
                        if (c1to25[c][index].FailedData == false)
                        {
                            count += 1;
                        }
                    }

                    cleanedSNPs = new SeqSNP[count];

                    count = 0;
                    for (int index = 0; index <= c1to25[c].GetUpperBound(0); index++)
                    {
                        if (c1to25[c][index].FailedData == false)
                        {
                            cleanedSNPs[count] = c1to25[c][index];
                            count += 1;
                        }
                    }

                    c1to25[c] = cleanedSNPs;
                    cleanedSNPs = null;

                }

                answer = 1;
            }
            catch (Exception ex)
            {
            }
            return answer;
        }

        private int GetMaximumLength()
        {
            int answer = 0;
            try
            {
                SeqSNP[] cn = null;
                for (int index = 1; index <= 22; index++)
                {
                    cn = c1to25[index];
                    int NumberOfSNPs = cn.GetUpperBound(0);
                     while (cn[NumberOfSNPs] == null)
                    {
                        NumberOfSNPs -= 1;
                        if (NumberOfSNPs < 0)
                            break;
                    }

                    if (cn[NumberOfSNPs].Position > this.MaximumLength)
                    {
                        this.MaximumLength = (int)cn[NumberOfSNPs].Position + 1;
                    }
                }

                answer = 1;
            }
            catch (Exception ex)
            {
                answer = -1;
            }

            return answer;
        }

        private int setClusters()
        {
            SeqSNP[] cN = null;
            int answer = 0;
            float maximumValue = 0;
            float minimumLength = 0;

            if (MaximumLength > 500)
            {
                maximumValue = 400000;
                minimumLength = 1000000;
            }
            else
            {
                maximumValue = 0.4f;
                minimumLength = 1;
            }

            int regionStartIndex = 0;
            double regionStart = 0;
            bool inCluster = false;

            try
            {
                for (int chromosome = 1; chromosome <= 22; chromosome++)
                {
                    cN = c1to25[chromosome];
                    inCluster = false;
                    regionStart = cN[0].Position;
                    regionStartIndex = 0;

                    for (int index = 1; index <= cN.GetUpperBound(0); index++)
                    {
                        if (cN[index].Position - cN[index - 1].Position < maximumValue)
                        {
                            cN[index].InCluster = true;
                            inCluster = true;
                        }
                        else if (inCluster == true)
                        {
                            if (index - regionStartIndex > 5 && cN[index].Position - regionStart < minimumLength)
                            {
                                for (int innnerIndex = regionStartIndex; innnerIndex <= index; innnerIndex++)
                                {
                                    cN[innnerIndex].InCluster = false;
                                }
                            }
                            regionStart = cN[index].Position;
                            regionStartIndex = index;
                            inCluster = false;
                        }
                        else if (inCluster == false)
                        {
                            regionStart = cN[index - 1].Position;
                            regionStartIndex = index - 1;
                        }
                    }

                }

                answer = 1;
            }
            catch (Exception ex)
            {
                answer = -1;
            }

            return answer;

        }

        private int SetIBDValues()
        {
            int answer = -1;
            SeqSNP[] cN = null;

            try
            {
                int SNPsInClusters = 0;
                int SNPsInClustersAndHomozygousRuns = 0;

                for (int chromosome = 1; chromosome <= 22; chromosome++)
                {
                    cN = c1to25[chromosome];
                    for (int index = 1; index <= cN.GetUpperBound(0); index++)
                    {
                        if (cN[index].InCluster == true)
                        {
                            SNPsInClusters += 1;
                            if (cN[index].MyRuns > ExclusionCutOff)
                            {
                                SNPsInClustersAndHomozygousRuns += 1;
                            }
                        }
                    }
                }

                answer = 1;


            }
            catch (Exception ex)
            {
            }

            return answer;

        }

        private List<DNARegion> WriteNewDescription()
        {
            List<DNARegion> answer = null;

            try
            {
                int NumberOfSNPs = 0;
                double ThisChromosomeLength = 0;

                int here = 0;
                int ThisRegion = 0;
                float RunLength = 0;

                double HomoSize = 0;
                double GenoSize = 0;

                List<DNARegion> theseRegions = new List<DNARegion>();

                for (int ThisChromosome = 1; ThisChromosome <= 22; ThisChromosome++)
                {
                    NumberOfSNPs = c1to25[ThisChromosome].GetUpperBound(0);
                    while (c1to25[ThisChromosome][NumberOfSNPs] == null)
                    {
                        NumberOfSNPs -= 1;
                    }
                    ThisChromosomeLength = c1to25[ThisChromosome][NumberOfSNPs].Position;

                    GenoSize += ThisChromosomeLength - c1to25[ThisChromosome][0].Position;
                    int endOfRun = 0;
                    here = 0;
                    DNARegion interval = null;

                    while (here < NumberOfSNPs + 1)
                    {
                        if (c1to25[ThisChromosome][here].MyRuns > ExclusionCutOff)
                        {
                            ThisRegion = here + c1to25[ThisChromosome][here].MyRuns - 1;
                            if (ThisRegion > NumberOfSNPs)
                                ThisRegion = NumberOfSNPs;
                            RunLength = Convert.ToSingle(c1to25[ThisChromosome][Convert.ToInt32(ThisRegion)].Position - Convert.ToSingle(c1to25[ThisChromosome][here].Position));
                            HomoSize += RunLength;

                            for (int index = here; index <= ThisRegion - 1; index++)
                            {
                                if (c1to25[ThisChromosome][index + 1].Position - c1to25[ThisChromosome][index].Position > 5000000)
                                {
                                    HomoSize -= c1to25[ThisChromosome][index + 1].Position - c1to25[ThisChromosome][index].Position;
                                }
                            }

                            endOfRun = here + c1to25[ThisChromosome][here].MyRuns - 1;
                            if (endOfRun > c1to25[ThisChromosome].GetUpperBound(0))
                                endOfRun = c1to25[ThisChromosome].GetUpperBound(0);
                            interval = new DNARegion(ThisChromosome, (int)c1to25[ThisChromosome][here].Position, (int)c1to25[ThisChromosome][endOfRun].Position);
                            theseRegions.Add(interval);
                            here += c1to25[ThisChromosome][here].MyRuns;
                        }
                        else
                        {
                            here += 1;
                        }
                    }

                }

                answer = condenseList(theseRegions);

            }
            catch (Exception ex)
            {
            }

            return answer;

        }

        private List<DNARegion> condenseList(List<DNARegion> ThisList)
        {
            List<DNARegion> answer = null;
            if (ThisList.Count < 2)
            { answer = ThisList; }
            else
            {
                answer = new List<DNARegion>();
                int Place = 0;
                for (int index = 1; ThisList.Count > index; index++)
                {
                    if (ThisList[Place].Chromosome == ThisList[index].Chromosome && ThisList[Place].EndPoint + 500000 > ThisList[index].StartPoint)
                    { ThisList[Place].EndPoint = ThisList[index].EndPoint; }
                    else
                    {
                        answer.Add(ThisList[Place]);
                        Place = index;
                    }
                }
                if (answer.Count > 0)
                {
                    if (ThisList[ThisList.Count - 1].EndPoint != answer[answer.Count - 1].EndPoint)
                    { answer.Add(ThisList[Place]); }
                }
                else { answer.Add(ThisList[Place]); }

            }

            List<DNARegion> filtered = new List<DNARegion>();
            foreach (DNARegion dr in answer)
            { if (dr.Length > 999999) { filtered.Add(dr); } }
            
            return filtered;
        }

    }
}
