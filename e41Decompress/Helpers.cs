using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace e41Decompress
{
    public static class Helpers
    {
        public static string SelectFile(string Title = "Select file", string Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*")
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = Title;
            fdlg.Filter = Filter;
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;

            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                return fdlg.FileName;
            }
            return "";

        }
        public static string SelectSaveFile(string Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*", string defaultFileName = "")
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = Filter;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Save to file";
            saveFileDialog.FileName = defaultFileName;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            else
                return "";

        }

        public static string SelectFolder(string Title, string defaultFolder = "")
        {
            string folderPath = "";
            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            folderBrowser.InitialDirectory = defaultFolder;
            // Always default to Folder Selection.
            folderBrowser.Title = Title;
            folderBrowser.FileName = "Folder Selection";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                folderPath = Path.GetDirectoryName(folderBrowser.FileName);
            }
            return folderPath;
        }

        public static byte[] ReadBin(string FileName, uint FileOffset, uint Length)
        {

            byte[] buf = new byte[Length];

            long offset = 0;
            long remaining = Length;

            using (BinaryReader freader = new BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                freader.BaseStream.Seek(FileOffset, 0);
                while (remaining > 0)
                {
                    int read = freader.Read(buf, (int)offset, (int)remaining);
                    if (read <= 0)
                        throw new EndOfStreamException
                            (String.Format("End of stream reached with {0} bytes left to read", remaining));
                    remaining -= read;
                    offset += read;
                }
                freader.Close();
            }
            return buf;
        }
        public static void WriteBinToFile(string FileName, byte[] Buf)
        {

            using (FileStream stream = new FileStream(FileName, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(Buf);
                    writer.Close();
                }
            }
        }
        public static uint BEToUint32(byte[] buf, uint offset)
        {
            //Shift first byte 24 bits left, second 16bits left...
            return (uint)((buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3]);
        }

        public static UInt16 BEToUint16(byte[] buf, uint offset)
        {
            return (UInt16)((buf[offset] << 8) | buf[offset + 1]);
        }

    }
}
