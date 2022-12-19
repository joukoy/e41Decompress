using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static e41Decompress.Helpers;


namespace e41Decompress
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        byte[] buf;
        byte[] newBuf;
        uint fsize;
        bool compression = false;
        string filetype = "";

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void TestIntegrity(byte[] Original, byte[] UnCompressed)
        {
            Logger("Testing integrity...");
            Application.DoEvents();
            byte[] compressed = compress(UnCompressed);
            byte[] cmpBuf = AddHeader(compressed);

            if (Original.SequenceEqual(cmpBuf))
            {
                Logger(" [OK]");
            }
            else
            {
                LoggerBold("Warning! integrity test failed. Unknown compression method?");
            }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            try
            {
                Logger("Uncompressing...");
                compression = false;
                newBuf = uncompress(buf);
                if (newBuf == null)
                    return;
                Logger("[OK]");
                TestIntegrity(buf, newBuf);
                string fName = SelectSaveFile();
                if (fName.Length == 0)
                    return;
                saveBin(fName);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private byte[] compress(byte[] inBuf)
        {
            if (radioType4.Checked)
            {
                return CompressLZMA(inBuf);
            }
            uint addr = 0;
            List<byte> singles = new List<byte>();
            List<byte> localBuf = new List<byte>();
            while (addr < inBuf.Length)
            {
                //Search for repeating DWORD blocks:
                byte[] DWarray = new byte[4];
                byte[] nextBlock = new byte[4];
                byte dwCounter = 1;
                byte wCounter = 1;
                byte bCounter = 1;
                uint addr2 = addr;
                byte crawl = 0;
                if (addr == 0x14fc)
                    Debug.WriteLine("");
                if ((addr + 8) < inBuf.Length)
                {
                    Array.Copy(inBuf, addr, DWarray, 0, 4);
                    while ((addr2 + 8) < inBuf.Length && dwCounter < 65)
                    {
                        addr2 += 4;
                        Array.Copy(inBuf, addr2, nextBlock, 0, 4);
                        if (DWarray.SequenceEqual(nextBlock))
                            dwCounter++;
                        else
                            break;
                    }
                }
                // Search for repeating WORDS
                byte[] Warray = new byte[2];
                nextBlock = new byte[2];
                if ((addr + 4) < inBuf.Length)
                {
                    Array.Copy(inBuf, addr, Warray, 0, 2);
                    wCounter = 1;
                    addr2 = addr;
                    while ((addr2 + 4) < inBuf.Length && wCounter < 65)
                    {
                        addr2 += 2;
                        Array.Copy(inBuf, addr2, nextBlock, 0, 2);
                        if (Warray.SequenceEqual(nextBlock))
                            wCounter++;
                        else
                            break;
                    }
                }

                //Search for repeating single bytes
                bCounter = 1;
                addr2 = addr + 1;
                while (addr2 < inBuf.Length && inBuf[addr] == inBuf[addr2] && bCounter < 65)
                {
                    addr2++;
                    bCounter++;
                }

                if (dwCounter == 1 && wCounter == 1 && bCounter < 3)
                {
                    //No patterns, store as single bytes
                    if (singles.Count >= 0x3f)
                    {
                        localBuf.Add((byte)singles.Count);
                        for (int s = 0; s < singles.Count; s++)
                            localBuf.Add(singles[s]);
                        singles = new List<byte>();
                    }
                    singles.Add(inBuf[addr]);
                    addr++;
                }
                else
                {
                    //Pattern found, flush singles buffer
                    if (singles.Count > 0)
                    {
                        localBuf.Add((byte)singles.Count);
                        for (int s = 0; s < singles.Count; s++)
                            localBuf.Add(singles[s]);
                        singles = new List<byte>();
                    }
                    //Select best method:
                    //Debug.WriteLine("DW: " + dwCounter + ", W: " + wCounter + ", B: " + bCounter);
                    int method = 1;

                    if (bCounter > 3 && bCounter < 0x3f)
                        method = 1;
                    else if (wCounter > 1 && wCounter < 0x3f)
                        method = 2;
                    else if (dwCounter > 1)
                        method = 4;
                    else if (wCounter > 1)
                        method = 2;

                    if (method == 4)
                    {
                        if (dwCounter > 63)
                            dwCounter = 63;
                        crawl = (byte)(0xC0 + dwCounter);
                        localBuf.Add(crawl);
                        Debug.WriteLine("Address: " + addr.ToString("X") + ", Found dwords: " + dwCounter.ToString() + ", Crawl: " + crawl.ToString("X") + ", writebuf: " + localBuf.Count);
                        for (int b = 0; b < 4; b++)
                            localBuf.Add(DWarray[b]);
                        addr += (uint)(4 * dwCounter);

                    }
                    else if (method == 2)
                    {
                        if (wCounter > 63)
                            wCounter = 63;
                        crawl = (byte)(0x80 + wCounter);
                        localBuf.Add(crawl);
                        Debug.WriteLine("Address: " + addr.ToString("X") + ", Found words: " + wCounter.ToString() + ", Crawl: " + crawl.ToString("X") + ", writebuf: " + localBuf.Count);
                        localBuf.Add(Warray[0]);
                        localBuf.Add(Warray[1]);
                        addr += (uint)(2 * wCounter);
                    }
                    else //method == 1
                    {
                        if (bCounter > 63)
                            bCounter = 63;
                        crawl = (byte)(0x40 + bCounter);
                        localBuf.Add(crawl);
                        Debug.WriteLine("Address: " + addr.ToString("X") + ", Found bytes: " + bCounter.ToString() + ", Crawl: " + crawl.ToString("X") + ", writebuf: " + localBuf.Count);
                        localBuf.Add(inBuf[addr]);
                        addr += bCounter;

                    }
                }
            }
            return localBuf.ToArray();
        }

        private byte[] uncompress(byte[] inBuf)
        {
            uint addr = 20;
            if (inBuf[0] == 0xFF && inBuf[1] == 0xFF && inBuf[2] == 0xFF && inBuf[3] == 0xFF)
            {
                radioType1.Checked = true;
                filetype = "type1";
                addr = 12;
                Logger("Type 1");
            }
            else if (inBuf[0] == 0x04 && inBuf[1] == 0x01 && inBuf[4] == 0)
            {
                radioType2.Checked = true;
                filetype = "type2";
                addr = 8;
                Logger("Type 2");
            }
            else if (inBuf[0] == 0x04 && inBuf[1] == 0x01 && inBuf[4] == 0xFF)
            {
                radioType3.Checked = true;
                filetype = "type3";
                addr = 20;
                Logger("Type 3");
            }
            else if (inBuf[0] == 0x04 && inBuf[1] == 0x02)
            {
                radioType4.Checked = true;
                filetype = "type4";
                Logger("Type 4 (LZMA)");
                byte[] tmpBuf = new byte[inBuf.Length - 20];
                Array.Copy(inBuf, 20, tmpBuf, 0, tmpBuf.Length);
                return DecompressLZMA(tmpBuf);
            }
            else
            {
                Logger("Unsupprted file, type: " + inBuf[0].ToString("X2") + " " + inBuf[1].ToString("X2") + " " + inBuf[2].ToString("X2") + " " + inBuf[3].ToString("X2")); ;
                return null;
            }
            List<byte> localBuf = new List<byte>();
            while (addr < fsize)
            {
                byte crawl = inBuf[addr];
                addr++;
                if (crawl > 0xBF)
                {
                    int count = crawl - 0xC0;
                    for (int a=0; a < count; a++)
                    {
                        for (int b = 0; b < 4; b++)
                            localBuf.Add(inBuf[addr + b]);
                    }
                    Debug.WriteLine("Addr: " + addr.ToString("X") + ", Crawl: " + crawl.ToString("X") + ", Copying 4 bytes: " + count.ToString() + " times");
                    addr += 4;
                }
                else if( crawl > 0x7F)
                {
                    int count = crawl - 0x80;
                    for (int a = 0; a < count; a++)
                    {
                        localBuf.Add(inBuf[addr]);
                        localBuf.Add(inBuf[addr + 1]);
                    }
                    Debug.WriteLine("Addr: " + addr.ToString("X") + ", Crawl: " + crawl.ToString("X") + ", Copying 2 bytes: " + count.ToString() + " times");
                    addr += 2;
                }
                else if (crawl > 0x3F)
                {
                    int count = crawl - 0x40;
                    byte val = inBuf[addr];
                    for (int a = 0; a < count; a++)
                        localBuf.Add(val);
                    Debug.WriteLine("Addr: " + addr.ToString("X") + ", Crawl: " + crawl.ToString("X") + ", Copying 1 byte: " + count.ToString() + " times");
                    addr++;
                }
                else 
                {
                    for (int a = 0; a < crawl; a++)
                        localBuf.Add(inBuf[addr + a]);
                    Debug.WriteLine("Addr: " + addr.ToString("X") + ", Crawl: " + crawl.ToString("X") + ", Copying: " + crawl.ToString() + " bytes");
                    addr += (uint)(crawl);
                }
                //addr++;
            }
            return localBuf.ToArray();
        }

        public void LoggerBold(string LogText, Boolean NewLine = true)
        {
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
            txtResult.AppendText(LogText);
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
        }

        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }

        private byte[] AddHeader(byte[] inBuf)
        {
            try
            {
                if (inBuf == null)
                    return null;
                byte[] writeBuf;
                int offset = 0;

                byte[] headerType1 = { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                byte[] headerType2 = { 0x04, 0x01, 0x00, 0x00 };
                byte[] headerType3 = { 0x04, 0x01, 0x00, 0x00 };
                byte[] header1Type4 = { 0x04, 0x02, 0x00, 0x00 };
                byte[] header2Type4 = { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x01 };
                //Note for Type4: header2Type4 length = headerType1.length
                byte[] bytesFollow1 = BitConverter.GetBytes((uint)(inBuf.Length + headerType1.Length + 4));
                Array.Reverse(bytesFollow1, 0, 4);
                byte[] bytesFollow2 = BitConverter.GetBytes((uint)(inBuf.Length));
                Array.Reverse(bytesFollow2, 0, 4);

                int headersize = headerType1.Length + 4;
                if (radioType2.Checked)
                    headersize = headerType2.Length + 4;
                else if (radioType3.Checked)
                    headersize = headersize + headerType3.Length + 4;
                else if (radioType4.Checked)
                    headersize = header1Type4.Length + 4 + header2Type4.Length + 4;

                writeBuf = new byte[inBuf.Length + headersize];
                if (radioType1.Checked)
                {
                    Array.Copy(headerType1, 0, writeBuf, offset, headerType1.Length);
                    offset += headerType1.Length;
                    Array.Copy(bytesFollow2, 0, writeBuf, offset, 4);
                    offset += 4;
                }
                else if (radioType2.Checked)
                {
                    offset = 0;
                    Array.Copy(headerType2, 0, writeBuf, offset, headerType2.Length);
                    offset += headerType2.Length;
                    Array.Copy(bytesFollow2, 0, writeBuf, offset, 4);
                    offset += 4;

                }
                else if (radioType3.Checked)
                {
                    Array.Copy(headerType3, 0, writeBuf, 0, headerType3.Length);
                    offset = headerType3.Length;
                    Array.Copy(bytesFollow1, 0, writeBuf, offset, 4);
                    offset += 4;
                    Array.Copy(headerType1, 0, writeBuf, offset, headerType1.Length);
                    offset += headerType1.Length;
                    Array.Copy(bytesFollow2, 0, writeBuf, offset, 4);
                    offset += 4;
                }
                else if (radioType4.Checked)
                {
                    offset = 0;
                    Array.Copy(header1Type4, 0, writeBuf, offset, header1Type4.Length);
                    offset += header1Type4.Length;
                    Array.Copy(bytesFollow1, 0, writeBuf, offset, 4);
                    offset += 4;
                    Array.Copy(header2Type4, 0, writeBuf, offset, header2Type4.Length);
                    offset += header2Type4.Length;
                    Array.Copy(bytesFollow2, 0, writeBuf, offset, 4);
                    offset += 4;
                }
                Array.Copy(inBuf, 0, writeBuf, offset, inBuf.Length);
                return writeBuf;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold(" Error, line " + line + ": " + ex.Message);
            }
            return null;
        }

        void saveBin(string fName)
        {
            try
            {
                Logger("Saving to file: " + fName, false);

                byte[] writeBuf;
                if (compression)
                    writeBuf = AddHeader(newBuf);
                else
                    writeBuf = newBuf;
                WriteBinToFile(fName, writeBuf);
                Logger(" [OK]");
                if (!compression && chkWriteTxt.Checked)
                {
                    string txtFile = Path.Combine(Path.GetDirectoryName(fName), Path.GetFileNameWithoutExtension(fName) + ".txt");
                    Logger("Writing type to file: " + txtFile + "...", false);
                    WriteTextFile(txtFile, filetype);
                    Logger(" [OK]");
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            string fName = SelectSaveFile();
            if (fName.Length == 0)
                return;
            saveBin(fName);
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            try
            {
                Logger(" [OK]");
                Logger("Compressing...");
                compression = true;
                newBuf = compress(buf);
                Logger("[OK]");
                string fName = SelectSaveFile();
                if (fName.Length == 0)
                    return;
                saveBin(fName);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnMultiCompress_Click(object sender, EventArgs e)
        {
            try
            {
                compression = true;
                frmFileSelection frmF = new frmFileSelection();
                frmF.btnOK.Text = "Compress!";
                frmF.LoadFiles(Application.StartupPath);
                if (frmF.ShowDialog() != DialogResult.OK)
                    return;
                for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                {
                    string fName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                    fsize = (uint)new FileInfo(fName).Length;
                    Logger("Reading file: " + fName, false);
                    buf = ReadBin(fName, 0, fsize);
                    Logger(" [OK]");
                    Logger("Compressing...");
                    newBuf = compress(buf);
                    Logger("[OK]");
                    string fPath = Path.GetDirectoryName(fName);
                    string baseName = Path.GetFileNameWithoutExtension(fName);
                    string saveFname = Path.Combine(fPath, baseName + txtExtension.Text);
                    saveBin(saveFname);
                }
                Logger("Done");
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnMultiExtract_Click(object sender, EventArgs e)
        {
            try
            {
                frmFileSelection frmF = new frmFileSelection();
                frmF.btnOK.Text = "Extract!";
                frmF.LoadFiles(Application.StartupPath);
                if (frmF.ShowDialog() != DialogResult.OK)
                    return;
                compression = false;
                for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                {
                    string fName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                    fsize = (uint)new FileInfo(fName).Length;
                    Logger("Reading file: " + fName, false);
                    buf = ReadBin(fName, 0, fsize);
                    Logger(" [OK]");
                    Logger("Uncompressing...");
                    newBuf = uncompress(buf);
                    Logger("[OK]");
                    TestIntegrity(buf, newBuf);
                    string fPath = Path.GetDirectoryName(fName);
                    string baseName = Path.GetFileNameWithoutExtension(fName);
                    string saveFname = Path.Combine(fPath, baseName + txtExtension.Text);
                    saveBin(saveFname);
                }
                Logger("Done");
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.Show(this);
        }



        public byte[] CompressLZMA(byte[] toCompress)
        {
            try
            {
                string tmpFile1 = Path.Combine(Path.GetTempPath(), "e41decompress-lzma1.tmp");
                string tmpFile2 = Path.Combine(Path.GetTempPath(), "e41decompress-lzma2.tmp");
                WriteBinToFile(tmpFile1, toCompress);
                Process process = new Process();
                // Configure the process using the StartInfo properties.
                string lzmaexe = Path.Combine(Application.StartupPath, "lzma.exe");
                if (!File.Exists(lzmaexe))
                {
                    LoggerBold("File missing: " + lzmaexe);
                    return null;
                }
                process.StartInfo.FileName = lzmaexe;
                process.StartInfo.Arguments = "e \"" + tmpFile1 + "\" \"" + tmpFile2 + "\" -d12 -fb32";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //process.StartInfo.UseShellExecute = false;

                process.Start();
                process.WaitForExit();
                fsize = (uint)new FileInfo(tmpFile2).Length;
                byte[] readBuf = ReadBin(tmpFile2, 0, fsize);
                File.Delete(tmpFile1);
                File.Delete(tmpFile2);
                return readBuf;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold(" Error, line " + line + ": " + ex.Message);
            }
            return null;
        }


        public byte[] CompressLZMA_OLD(byte[] toCompress)
        {

            int dictionary = 4096; //-d12 = 2^12, default: 128 * 1024;
            bool eos = false;

/*            CoderPropID[] propIDs =
                    {
                    CoderPropID.DictionarySize,
                    CoderPropID.PosStateBits,
                    CoderPropID.LitContextBits,
                    CoderPropID.LitPosBits,
                    CoderPropID.NumFastBytes,
                    CoderPropID.MatchFinder,
                    CoderPropID.EndMarker                    
                };

            object[] properties =
                    {
                    (System.Int32)dictionary,
                    (System.Int32)2,
                    (System.Int32)3,
                    (System.Int32)0,
                    (System.Int32)32, //fb32
                    "BT4",
                    eos
                };
*/
            CoderPropID[] propIDs =
            {
                    CoderPropID.DictionarySize,
                    CoderPropID.NumFastBytes
                };

            object[] properties =
                    {
                    (System.Int32)dictionary,
                    (System.Int32)32
                };

            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();

            using (MemoryStream input = new MemoryStream(toCompress))
            using (MemoryStream output = new MemoryStream())
            {
                coder.SetCoderProperties(propIDs, properties);
                coder.WriteCoderProperties(output);

                //Debug.WriteLine(BitConverter.ToString(output.ToArray()));

                output.Write(BitConverter.GetBytes(input.Length), 0, 8);

                coder.Code(input, output, input.Length, -1, null);
                return output.ToArray();
            }
        }

        public byte[] DecompressLZMA(byte[] toDecompress)
        {
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();

            using (MemoryStream input = new MemoryStream(toDecompress))
            using (MemoryStream output = new MemoryStream())
            {

                // Read the decoder properties
                byte[] properties = new byte[5];
                input.Read(properties, 0, 5);


                // Read in the decompress file size.
                byte[] fileLengthBytes = new byte[8];
                input.Read(fileLengthBytes, 0, 8);
                long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                coder.SetDecoderProperties(properties);
                coder.Code(input, output, input.Length, fileLength, null);

                return output.ToArray();
            }
        }

        private void btnReadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fName = SelectFile();
                if (fName.Length == 0)
                    return;
                fsize = (uint)new FileInfo(fName).Length;
                Logger("Reading file: " + fName, false);
                buf = ReadBin(fName, 0, fsize);
                Logger(" [OK]");
                string txtFile = Path.Combine(Path.GetDirectoryName(fName), Path.GetFileNameWithoutExtension(fName) + ".txt");
                if (File.Exists(txtFile))
                {
                    filetype = ReadTextFile(txtFile);
                    if (filetype.Length > 4)
                    {
                        Logger("Reading type from file: " + txtFile + ": ", false);
                        switch (filetype.ToLower().Substring(0, 5))
                        {
                            case "type1":
                                radioType1.Checked = true;
                                break;
                            case "type2":
                                radioType2.Checked = true;
                                break;
                            case "type3":
                                radioType3.Checked = true;
                                break;
                            case "type4":
                                radioType4.Checked = true;
                                break;
                        }
                        Logger(filetype);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }
    }
}

