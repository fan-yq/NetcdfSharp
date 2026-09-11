// NetcdfSharp.java.lang.StackTraceElement shim.
namespace NetcdfSharp.java.lang
{
    /// <summary>An element in a stack trace. Minimal subset used for logging.</summary>
    public class StackTraceElement
    {
        public string className;
        public string methodName;
        public string fileName;
        public int lineNumber;

        public StackTraceElement(string className, string methodName, string fileName, int lineNumber)
        {
            this.className = className;
            this.methodName = methodName;
            this.fileName = fileName;
            this.lineNumber = lineNumber;
        }

        public virtual string getClassName() => className;
        public virtual string getMethodName() => methodName;
        public virtual string getFileName() => fileName;
        public virtual int getLineNumber() => lineNumber;

        public override string ToString()
        {
            return className + "." + methodName + "(" + fileName + ":" + lineNumber + ")";
        }
    }
}
