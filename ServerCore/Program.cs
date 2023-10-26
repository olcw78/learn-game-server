
using ServerCore;

MySpinLock _spinLock = new MySpinLock();

int num = 0;

Task t1 = new(Thread1);
Task t2 = new(Thread2);
t1.Start();
t2.Start();

Task.WaitAll(t1, t2);

Console.WriteLine($"t1: {t1}, t2: {t2}, same? {t1 == t2}");
void Thread1()
{
    for (int i = 0; i < 100_000; i++)
    {
        _spinLock.Acquire();
        num++;
        _spinLock.Release();
    }
}

void Thread2()
{
    for (int i = 0; i < 100_000; i++)
    {
        _spinLock.Acquire();
        num--;
        _spinLock.Release();
    }
}