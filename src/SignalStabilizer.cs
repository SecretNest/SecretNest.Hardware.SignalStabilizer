using System;
using System.Threading;

namespace SecretNest.Hardware
{
    public class SignalStabilizer<T> : IDisposable
    {
        private bool _disposedValue;

        private DateTime _effectedTime;
        private readonly TimeSpan _waitingTime;

        private Thread _thread;
        private AutoResetEvent _valueChanged;
        private bool _needQuit;

        public T Value => _currentValue;
        private T _nextValue, _currentValue;

        private readonly ManualResetEventSlim _started;

        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;

        public SignalStabilizer(int waitingTimeMilliseconds, T initial = default, ThreadPriority threadPriority = ThreadPriority.AboveNormal)
            : this(new TimeSpan(0, 0, 0, 0, waitingTimeMilliseconds), initial, threadPriority)
        {
        }

        public SignalStabilizer(TimeSpan waitingTime, T initial = default, ThreadPriority threadPriority = ThreadPriority.AboveNormal)
        {
            _currentValue = initial;
            _started = new ManualResetEventSlim();
            _waitingTime = waitingTime;
            _valueChanged = new AutoResetEvent(false);
            _needQuit = false;
            _thread = new Thread(Notifier) { Priority = threadPriority};
            _thread.Start();
            _started.Wait();
            _thread.IsBackground = true;
            _started.Dispose();
            _started = null;
        }

        private void Notifier()
        {
            _started.Set();
            while (true)
            {
                _valueChanged.WaitOne();
                if (_needQuit) return;

                while (true)
                {
                    var waiting = _effectedTime - DateTime.Now;
                    if (waiting > TimeSpan.Zero)
                    {
                        Thread.Sleep(waiting);
                    }
                    else if (_needQuit)
                    {
                        return;
                    }
                    else
                    {
                        lock (_thread)
                        {
                            if (_effectedTime > DateTime.Now)
                            {
                                continue;
                            }

                            if (!_nextValue.Equals(_currentValue))
                            {
                                _currentValue = _nextValue;
                                ValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(_currentValue));
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void SetValue(T value)
        {
            lock (_thread)
            {
                _effectedTime = DateTime.Now + _waitingTime;
                _nextValue = value;
                _valueChanged.Set();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _needQuit = true;
                    _valueChanged.Set();
                    _thread.Join();
                    _thread = null;
                    _valueChanged.Dispose();
                    _valueChanged = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
