namespace ServerCore;

// 1. recursive-able?
// Write -> Write (o), Write -> Read (o), Read -> Write (x)
// 2. spinLock policy -> 5000 spin <-> yield
public class RWLock {
  private readonly bool _allowRecursive;

  private const int EMPTY_FLAG = 0x00000000;

  // 7 -> 0 1 1 1 -> 2^2 + 2^1 + 2^0
  private const int WRITE_MASK = 0x7FFF0000;
  private const int READ_MASK = 0x0000FFFF;
  private const int MAX_SPING_COUNT = 5000;

  // [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
  private int _flag = EMPTY_FLAG;
  private int _writeCount = 0;

  public RWLock(bool allowRecursive = true) {
    _allowRecursive = allowRecursive;
  }

  public void WriteLock() {
    if (_allowRecursive) {
      // check the same thread has already acquired WriteLock.
      int lockThreadId = (_flag & WRITE_MASK) >> 16;
      if (lockThreadId == Thread.CurrentThread.ManagedThreadId) {
        _writeCount++;
        return;
      }
    }

    int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
    const int expected = EMPTY_FLAG;

    // acquire after competing when no one acquires writeLock or readLock.
    while (true) {
      for (int i = 0; i < MAX_SPING_COUNT; ++i) {
        // try enter and break on success;
        if (Interlocked.CompareExchange(ref _flag, desired, expected) == EMPTY_FLAG) {
          _writeCount += 1;
          return;
        }

        Thread.Yield();
      }
    }
  }

  public void WriteUnlock() {
    int lockCount = --_writeCount;
    if (lockCount == 0)
      Interlocked.Exchange(ref _flag, EMPTY_FLAG);
  }

  public void ReadLock() {
    if (_allowRecursive) {
      int lockThreadId = (_flag & WRITE_MASK) >> 16;
      if (Thread.CurrentThread.ManagedThreadId == lockThreadId) {
        Interlocked.Increment(ref _flag);
        return;
      }
    }

    // increase 1 when no one acquires WriteLock.
    while (true) {
      for (int i = 0; i < MAX_SPING_COUNT; i++) {
        int expected = _flag & READ_MASK;
        int desired = expected + 1;
        if (Interlocked.CompareExchange(ref _flag, desired, expected) == expected)
          return;

        // if ((_flag & WRITE_MASK) == EMPTY_FLAG) {
        //   _flag += 1;
        //   return;
        // }
      }

      Thread.Yield();
    }
  }

  public void ReadUnlock() {
    Interlocked.Decrement(ref _flag);
  }
}