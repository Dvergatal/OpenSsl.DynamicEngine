namespace System.Security.Cryptography.OpenSsl
{
    public class DynamicEngineHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
    {
        public DynamicEngineHandle() : base(false)
        {
        }

        protected override bool ReleaseHandle()
        {
            if (IsInvalid) return false;

            SafeNativeMethods.ENGINE_free(this);
            SetHandleAsInvalid();
            return true;
        }
    }
}
