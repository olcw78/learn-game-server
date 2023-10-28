namespace ServerCore;

public class EventLock {
  private readonly AutoResetEvent _available = new(true);

  public void Acquire() {
    _available.WaitOne(); // try to enter.
  }

  public void Release() {
    _available.Set();
  }
}