namespace ServerCore;

public sealed class MySpinLock
{
    private volatile int _locked;
    private const int UNLOCKED = 0;
    private const int LOCKED = 1;

    public void Acquire()
    {
        int expected = UNLOCKED;
        int desired = LOCKED;

        while (true)
        {
            if (Interlocked.CompareExchange(ref _locked, desired, expected) == UNLOCKED)
                break;
            
            // Thread.Sleep(1); // 무조건 휴식 => 무조건 1ms 정도 쉬고 시퍼!>!
            // Thread.Sleep(0); // 자신보다 우선순위가 낮은 애들한테는 양보 X. 우선순위가 나보다 같거나 높은 쓰레드가 없으면 다시 본인한테 옴.
            Thread.Yield(); // 관대한 양보. 조건 X. 지금 실행가능한 쓰레드에게 양보.
        }
    }

    public void Release()
    {
        _locked = UNLOCKED;
    }
}