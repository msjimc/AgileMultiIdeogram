using System;
using System.Collections;
using System.Text;

namespace  MultiIdeogram_CS
{
    public class VCFRegions
    {
	private SeqVariant[] variants = null;
    private bool dataAvaialbe = false;
    public VCFRegions()
	{
		variants = null;
	}

    public string GetData(string fileName, bool IgnoreRSField, int readDepthCutOff,  bool isGVCF, bool VCFGenotypes)
	{
		string result = null;
		variants = null;
		int answer = 0;

        answer = ReadAFile(fileName, IgnoreRSField, readDepthCutOff, isGVCF, VCFGenotypes);
        if (answer < 0)
            {
            dataAvaialbe = false;
            if (answer == -1) { result = "The file " + fileName.Substring(fileName.LastIndexOf('\\') + 1) + " could not be opened is it open in another application"; }
            else if (answer == -2) { result = "Could not read data in " + fileName.Substring(fileName.LastIndexOf('\\') + 1); }
            }
        else { dataAvaialbe = true; }

		return result;

	}

    private int ReadAFile(string thisFile, bool IgnoreRSField, int readDepthCutOff, bool isGVCF, bool VCFGenotypes) 
    {
        gVCFReader fr = null;
        int answer = 1;

        try
        {
            fr = new gVCFReader(thisFile);
            bool formated = false;
            string line = null;
            int counter = 0;
            VCFPharser vp = new VCFPharser();

            while (fr.Peek() > 0 && formated == false)
            {
                line = fr.ReadLine();

                vp.ReadLine(line, readDepthCutOff, isGVCF, VCFGenotypes);
                formated = vp.IsReady;
                line = null;
            }

            VariantBinarysearch vbs = new VariantBinarysearch();

            while (fr.Peek() > 0)
            {
                line = fr.ReadLine();
                if (line.Length > 0)
                {
                        if (IgnoreRSField == false)
                        {
                            if (vp.ReadRSLine(line, readDepthCutOff, isGVCF, VCFGenotypes) == true)
                            {
                                counter += 1;
                            }
                        }
                        else
                        {
                            if (vp.ReadLine(line, readDepthCutOff, isGVCF, VCFGenotypes) == true)
                            {
                                counter += 1;
                            }
                        }
                    }
                line = null;
            }

            fr.Close();

            variants = new SeqVariant[counter];
            fr = new gVCFReader(thisFile);
            counter = 0;

            while (fr.Peek() > 0)
            {
                line = fr.ReadLine();
                if (line.Length > 0)
                {
                        if (IgnoreRSField == false)
                        {
                            if (line.StartsWith("#") != true && vp.ReadRSLine(line, readDepthCutOff, isGVCF, VCFGenotypes) == true)
                            {
                                variants[counter] = new SeqVariant(vp.ChromosomeNumber, vp.Position, vp.ReferenceBase, vp.AlternateBase, vp.ID);
                                variants[counter].AddVariant(vp);
                                counter += 1;
                            }
                        }
                        else
                        {
                            if (line.StartsWith("#") != true && vp.ReadLine(line, readDepthCutOff, isGVCF, VCFGenotypes) == true)
                            {
                                variants[counter] = new SeqVariant(vp.ChromosomeNumber, vp.Position, vp.ReferenceBase, vp.AlternateBase, vp.ID);
                                variants[counter].AddVariant(vp);
                                counter += 1;
                            }
                        }
                }
                line = null;
            }

            Array.Sort(variants, new SeqVariantSort());

        }
        catch (System.IO.IOException ex)
        {
            answer = -1;
        }
        catch (Exception ex)
        {
            answer = -2;
        }
        finally
        {
            if (fr != null)
            {
                fr.Close();
            }
        }
        return answer;
    }

	public SeqVariant[] getVariantArray {
		get { return variants; }
	}

    private bool HasData { get { return dataAvaialbe; } }
}

}
