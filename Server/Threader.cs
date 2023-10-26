namespace Server;

public static class Threader
{
    private static int _x = 0;
    private static int _y = 0;
    private static int _r1 = 0;
    private static int _r2 = 0;

    private static void Thread1()
    {
        _y = 1;
        _r1 = _x;
    }

    private static void Thread2()
    {
        _x = 1;
        _r2 = _y;
    }

    public static void Run()
    {
        int count = 0;
        while (true)
        {
            ++count;
            _x = _y = _r1 = _r2 = 0;

            var t1 = new Task(Thread1);
            var t2 = new Task(Thread2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            if (_r1 == 0 && _r2 == 00)
                break;
        }

        Console.WriteLine($"{count} 에 빠져나옴.");
    }
}