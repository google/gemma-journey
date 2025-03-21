using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
namespace GemmaCpp
{
    public class GemmaException : Exception
    {
        public GemmaException(string message) : base(message) { }
    }

    public class Gemma : IDisposable
    {
        private IntPtr _context;
        private bool _disposed;

        // Optional: Allow setting DLL path
        public static string DllPath { get; set; } = "gemma.dll";

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        static Gemma()
        {
            // Load DLL from specified path
            if (LoadLibrary(DllPath) == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Failed to load {DllPath}. Error: {Marshal.GetLastWin32Error()}");
            }
        }

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GemmaCreate(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string tokenizerPath,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string modelType,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string weightsPath,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string weightType,
            int maxLength);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GemmaDestroy(IntPtr context);

        // Delegate type for token callbacks
        public delegate bool TokenCallback(string token);

        // Keep delegate alive for duration of calls
        private GCHandle _callbackHandle;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool GemmaTokenCallback(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string text,
            IntPtr userData);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GemmaGenerate(
            IntPtr context,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string prompt,
            [Out] byte[] output,
            int maxLength,
            GemmaTokenCallback callback,
            IntPtr userData);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GemmaGenerateMultimodal(
            IntPtr context,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string prompt,
            IntPtr image,
            [MarshalAs(UnmanagedType.LPUTF8Str)] StringBuilder output,
            int maxLength,
            GemmaTokenCallback callback,
            IntPtr userData);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GemmaCountTokens(
            IntPtr context,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string text);

        // Configuration function imports
        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GemmaSetMultiturn(IntPtr context, int value);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GemmaSetTemperature(IntPtr context, float value);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GemmaSetTopK(IntPtr context, int value);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GemmaSetDeterministic(IntPtr context, int value);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GemmaResetContext(IntPtr context);

        // Context management function imports
        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GemmaCreateContext(
            IntPtr context,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string contextName);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GemmaSwitchContext(
            IntPtr context,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string contextName);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GemmaDeleteContext(
            IntPtr context,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string contextName);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GemmaHasContext(
            IntPtr context,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string contextName);

        // Native callback delegate type
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GemmaLogCallback(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string message,
            IntPtr userData);

        [DllImport("gemma", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GemmaSetLogCallback(
            IntPtr context,
            GemmaLogCallback callback,
            IntPtr userData);

        private GCHandle _logCallbackHandle;
        private bool _loggingEnabled = false;

        public Gemma(string tokenizerPath, string modelType, string weightsPath, string weightType, int maxLength = 8192)
        {
            _context = GemmaCreate(tokenizerPath, modelType, weightsPath, weightType, maxLength);
            if (_context == IntPtr.Zero)
            {
                throw new GemmaException("Failed to create Gemma context");
            }
        }

        // Enable debug logging
        public void EnableLogging(bool enable = true)
        {
            if (enable && !_loggingEnabled)
            {
                GemmaLogCallback logCallback = (message, _) =>
                {
                    Debug.WriteLine($"Gemma: {message}");
                };
                _logCallbackHandle = GCHandle.Alloc(logCallback);
                GemmaSetLogCallback(_context, logCallback, IntPtr.Zero);
                _loggingEnabled = true;
            }
            else if (!enable && _loggingEnabled)
            {
                if (_logCallbackHandle.IsAllocated)
                    _logCallbackHandle.Free();
                GemmaSetLogCallback(_context, null, IntPtr.Zero);
                _loggingEnabled = false;
            }
        }

        // Configuration methods
        public void SetMultiturn(bool enable)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            GemmaSetMultiturn(_context, enable ? 1 : 0);
            Debug.WriteLine($"Gemma: Set multiturn to {(enable ? "enabled" : "disabled")}");
        }

        public void SetTemperature(float temperature)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            GemmaSetTemperature(_context, temperature);
            Debug.WriteLine($"Gemma: Set temperature to {temperature}");
        }

        public void SetTopK(int topK)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            GemmaSetTopK(_context, topK);
            Debug.WriteLine($"Gemma: Set topK to {topK}");
        }

        public void SetDeterministic(bool deterministic)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            GemmaSetDeterministic(_context, deterministic ? 1 : 0);
            Debug.WriteLine($"Gemma: Set deterministic to {(deterministic ? "true" : "false")}");
        }

        public void ResetContext()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            GemmaResetContext(_context);
            Debug.WriteLine("Gemma: Reset context");
        }

        // Context management methods
        public bool CreateContext(string contextName)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            bool result = GemmaCreateContext(_context, contextName) != 0;
            Debug.WriteLine($"Gemma: Create context '{contextName}' - {(result ? "succeeded" : "failed")}");
            return result;
        }

        public bool SwitchContext(string contextName)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            bool result = GemmaSwitchContext(_context, contextName) != 0;
            Debug.WriteLine($"Gemma: Switch to context '{contextName}' - {(result ? "succeeded" : "failed")}");
            return result;
        }

        public bool DeleteContext(string contextName)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            bool result = GemmaDeleteContext(_context, contextName) != 0;
            Debug.WriteLine($"Gemma: Delete context '{contextName}' - {(result ? "succeeded" : "failed")}");
            return result;
        }

        public bool HasContext(string contextName)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            bool result = GemmaHasContext(_context, contextName) != 0;
            Debug.WriteLine($"Gemma: Has context '{contextName}' - {result}");
            return result;
        }

        public int CountTokens(string prompt)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");
            int count = GemmaCountTokens(_context, prompt);
            return count;
        }

        public string Generate(string prompt, int maxLength = 4096)
        {
            return Generate(prompt, null, maxLength);
        }

        public string Generate(string prompt, TokenCallback callback, int maxLength = 4096)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            var outputBuffer = new byte[maxLength * 4];  // Allow for worst case UTF-8 size
            GemmaTokenCallback nativeCallback = null;

            // Track token count for debugging
            int tokenCount = 0;

            if (callback != null)
            {
                nativeCallback = (text, _) =>
                {
                    tokenCount++;
                    // Log token for debugging
                    Debug.WriteLine($"Token {tokenCount}: '{text}'");

                    // Pass token to user callback
                    return callback(text);
                };
                _callbackHandle = GCHandle.Alloc(nativeCallback);
            }

            try
            {
                int length = GemmaGenerate(_context, prompt, outputBuffer, maxLength,
                    nativeCallback, IntPtr.Zero);

                if (length < 0)
                    throw new GemmaException("Generation failed");

                Debug.WriteLine($"Generation complete: {tokenCount} tokens processed, result length: {length}");

                // Convert the byte buffer to a string using UTF-8 encoding
                string result = Encoding.UTF8.GetString(outputBuffer, 0, length);
                return result;
            }
            finally
            {
                if (_callbackHandle.IsAllocated)
                    _callbackHandle.Free();
            }
        }

        public string GenerateMultimodal(string prompt, float[] imageData, int imageWidth, int imageHeight, int maxLength = 4096)
        {
            return GenerateMultimodal(prompt, imageData, imageWidth, imageHeight, null, maxLength);
        }

        public string GenerateMultimodal(string prompt, float[] imageData, int imageWidth, int imageHeight, TokenCallback callback, int maxLength = 4096)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Gemma));

            if (_context == IntPtr.Zero)
                throw new GemmaException("Gemma context is invalid");

            if (imageData == null || imageData.Length == 0)
                throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));

            if (imageWidth <= 0 || imageHeight <= 0)
                throw new ArgumentException("Image dimensions must be positive");

            if (imageData.Length < imageWidth * imageHeight * 3)
                throw new ArgumentException("Image data array is too small for the specified dimensions");

            var output = new StringBuilder(maxLength);
            GemmaTokenCallback nativeCallback = null;

            if (callback != null)
            {
                nativeCallback = (text, _) => callback(text);
                _callbackHandle = GCHandle.Alloc(nativeCallback);
            }

            // Pin the image data so it doesn't move during the native call
            GCHandle imageHandle = GCHandle.Alloc(imageData, GCHandleType.Pinned);

            try
            {
                IntPtr imagePtr = imageHandle.AddrOfPinnedObject();

                int length = GemmaGenerateMultimodal(_context, prompt, imagePtr, output, maxLength,
                    nativeCallback, IntPtr.Zero);

                if (length < 0)
                    throw new GemmaException("Multimodal generation failed");

                return output.ToString();
            }
            finally
            {
                imageHandle.Free();

                if (_callbackHandle.IsAllocated)
                    _callbackHandle.Free();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_context != IntPtr.Zero)
                {
                    GemmaDestroy(_context);
                    _context = IntPtr.Zero;
                }
                if (_logCallbackHandle.IsAllocated)
                    _logCallbackHandle.Free();
                _disposed = true;
            }
        }

        ~Gemma()
        {
            Dispose();
        }
    }
}