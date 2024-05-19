using System;

public interface IHasProgress {
    public event EventHandler<ProgressChangedEventArgs> OnProgressChanged;
    public class ProgressChangedEventArgs : EventArgs {
        public int Progress;
        public float ProgressNormalized;
    }
}