using UnityEngine;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using TMPro;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public IntPtr file = IntPtr.Zero;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public enum OpenFileNameFlags
{
    OFN_HIDEREADONLY = 0x4,
    OFN_FORCESHOWHIDDEN = 0x10000000,
    OFN_ALLOWMULTISELECT = 0x200,
    OFN_EXPLORER = 0x80000
}

public class FileExplorer : MonoBehaviour
{
    public TextMeshProUGUI _text;

    [StructLayout(LayoutKind.Sequential)]
    public struct UNICODE_STRING
    {
        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;
    }

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    public static bool GetOpenFileName1([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }

    [DllImport("ntdll.dll", CharSet = CharSet.Unicode)]
    static extern void RtlInitUnicodeString(out UNICODE_STRING DestinationString, string SourceString);

    public void OpenFileDialog()
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "All Files\0*.*\0\0";
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.dataPath;
        ofn.title = "Upload Image";
        ofn.defExt = "PNG";
        //ofn.flags = (int)OpenFileNameFlags.OFN_HIDEREADONLY | (int)OpenFileNameFlags.OFN_ALLOWMULTISELECT | (int)OpenFileNameFlags.OFN_EXPLORER;
        //          OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST | OFN_ALLOWMULTISELECT | OFN_NOCHANGEDIR
        ofn.flags = 0x00080000   | 0x00001000        | 0x00000800        | 0x00000200           | 0x00000008;

        const int MAX_FILE_LENGTH = 2048;
        string fileNames = new String(new char[MAX_FILE_LENGTH]);
        ofn.file = Marshal.StringToBSTR(fileNames);
        ofn.maxFile = fileNames.Length;

        if (GetOpenFileName(ofn))
        {
            List<string> selectedFilesList = new List<string>();

            long pointer = (long)ofn.file;
            string file = Marshal.PtrToStringAuto(ofn.file);

            while (file.Length > 0)
            {
                selectedFilesList.Add(file);
                pointer += file.Length * 2 + 2;
                ofn.file = (IntPtr)pointer;
                file = Marshal.PtrToStringAuto(ofn.file);
            }

            _text.text = "";

            foreach (string fileName in selectedFilesList)
            {
                _text.text += fileName + "\n\n";
                Debug.Log($"file: {fileName}");
            }
        }

        /****************************************************************************************/
        // THIS DOESNT WORK IN A BUILD IN UNITY!
        //using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
        //{
        //    openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

        //    // Allow the user to select multiple images.
        //    openFileDialog1.Multiselect = true;
        //    openFileDialog1.Title = "My txt Browser";

        //    DialogResult dr = openFileDialog1.ShowDialog();

        //    if (dr == System.Windows.Forms.DialogResult.OK)
        //    {
        //        // Read the files
        //        foreach (string file in openFileDialog1.FileNames)
        //        {
        //            Debug.Log($"{file}");
        //        }
        //    }
        //}
    }
}
