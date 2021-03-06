using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecretNest.Hardware
{
    public abstract class SignalStabilizerBase : IDisposable
    {
        private bool _disposedValue;

        private DateTime _effectedTime;
        private readonly TimeSpan _waitingTime;

        private Thread _thread;
        private AutoResetEvent _valueChanged;
        private bool _needQuit;

        protected SignalStabilizerBase(int waitingTimeMilliseconds) : this(new TimeSpan(0, 0, 0, 0, waitingTimeMilliseconds))
        {
        }

        protected SignalStabilizerBase(TimeSpan waitingTime)
        {
            _waitingTime = waitingTime;
            _valueChanged = new AutoResetEvent(false);
            _needQuit = false;
            _thread = new Thread(Notifier);
            _thread.Start();
        }

        public abstract object ValueGeneric { get; }
        public abstract void SetValueGeneric(object value);

        private protected void BeforeValueUpdating()
        {
            _effectedTime = DateTime.Now + _waitingTime;
        }

        private protected void AfterValueUpdating()
        {
            _valueChanged.Set();
        }

        public event EventHandler<ValueChangedGenericEventArgs> ValueChangedGeneric;

        private protected void OnValueChangedGeneric(object value)
        {
            ValueChangedGeneric?.Invoke(this, new ValueChangedGenericEventArgs(value));
        }

        private void Notifier()
        {
            while (true)
            {
                _valueChanged.WaitOne();
                if (_needQuit) return;

                while (true)
                {
                    var waiting = _effectedTime - DateTime.Now;
                    if (waiting > TimeSpan.Zero)
                    {
                        Task.Delay(waiting).Wait();
                    }
                    else if (!IsNextChanged())
                    {
                        break;
                    }
                }

                AnnounceValueIfChanged();
            }
        }

        private protected abstract bool IsNextChanged();

        private protected abstract void AnnounceValueIfChanged();

        protected virtual void Dispose(bool disposing)
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
