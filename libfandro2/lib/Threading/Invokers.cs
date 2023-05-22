using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using libfandro2.lib.WinAPI;

namespace libfandro2.lib.Threading {
    public delegate void BooleanInvoker(bool aboolean);
    public delegate void StringInvoker(string text);
    public delegate void FloatInvoker(float floating);
    public delegate void FileSystemInfoInvoker(FileSystemInfo file, long position);
    public delegate void FileFindSystemInfoInvoker(FileInfo file, long position);
    public delegate void FileProgressbarProgressStatus(long progress);
}
