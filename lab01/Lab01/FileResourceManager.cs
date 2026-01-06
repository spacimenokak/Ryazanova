using System;
using System.IO;
using System.Text;

namespace Lab01
{
    public class FileResourceManager : IDisposable
    {
        private FileStream _fileStream;
        private StreamWriter _writer;
        private StreamReader _reader;
        private bool _disposed;
        private readonly string _filePath;
        private readonly object _syncRoot = new object();

        public FileResourceManager(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public void OpenForWriting()
        {
            ThrowIfDisposed();

            lock (_syncRoot)
            {
                CloseStreams();

                _fileStream = new FileStream(
                    _filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Read
                );

                _writer = new StreamWriter(_fileStream, Encoding.UTF8);
            }
        }

        public void OpenForReading()
        {
            ThrowIfDisposed();

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("File not found", _filePath);
            }

            lock (_syncRoot)
            {
                CloseStreams();

                _fileStream = new FileStream(
                    _filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite
                );

                _reader = new StreamReader(_fileStream, Encoding.UTF8);
            }
        }

        public void WriteLine(string text)
        {
            ThrowIfDisposed();

            if (_writer == null)
            {
                throw new InvalidOperationException("File is not opened for writing");
            }

            lock (_syncRoot)
            {
                _writer.WriteLine(text);
                _writer.Flush();
            }
        }

        public string ReadAllText()
        {
            ThrowIfDisposed();

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("File not found", _filePath);
            }

            lock (_syncRoot)
            {
                using var fs = new FileStream(
                    _filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite
                );
                using var reader = new StreamReader(fs, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        public void AppendText(string text)
        {
            ThrowIfDisposed();

            lock (_syncRoot)
            {
                using var fs = new FileStream(
                    _filePath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.Read
                );
                using var writer = new StreamWriter(fs, Encoding.UTF8);
                writer.Write(text);
            }
        }

        public FileInfo GetFileInfo()
        {
            ThrowIfDisposed();
            return new FileInfo(_filePath);
        }

        private void CloseStreams()
        {
            _writer?.Dispose();
            _reader?.Dispose();
            _fileStream?.Dispose();

            _writer = null;
            _reader = null;
            _fileStream = null;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(FileResourceManager));
            }
        }

        // Реализация IDisposable (паттерн Dispose)
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // освобождаем управляемые ресурсы
                CloseStreams();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileResourceManager()
        {
            Dispose(false);
        }
    }
}
