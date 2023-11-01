// using System.Diagnostics;
// using ServerCore;
//
// Stopwatch sw = Stopwatch.StartNew();
// EventLock _lock = new();
// bool _bLockTaken = false;
// SpinLock _spinLock = new();
//
// int num = 0;
//
// Task t1 = new(Thread1);
// Task t2 = new(Thread2);
// t1.Start();
// t2.Start();
//
// Task.WaitAll(t1, t2);
//
// Console.WriteLine(num);
// sw.Stop();
//
// Console.WriteLine(sw.ElapsedMilliseconds + "ms");
//
// ReaderWriterLockSlim _rwLock = new();
// return;
//
//
// void Thread1() {
//   for (int i = 0; i < 100_000; i++) {
//     try {
//       // _lock.Acquire();
//       _spinLock.Enter(ref _bLockTaken);
//       num++;
//     }
//     finally {
//       // _lock.Release();
//
//       if (_bLockTaken)
//         _spinLock.Exit();
//     }
//   }
// }
//
// void Thread2() {
//   for (int i = 0; i < 100_000; i++) {
//     try {
//       // _lock.Acquire();
//       _spinLock.Enter(ref _bLockTaken);
//       num--;
//     }
//     finally {
//       // _lock.Release();
//       if (_bLockTaken)
//         _spinLock.Exit();
//     }
//   }
// }

// Reward GetRewardById(int id) {
//   try {
//     _rwLock.EnterReadLock();
//
//     return null;
//   }
//   finally {
//     _rwLock.ExitReadLock();
//   }
// }
//
// void AddReward(Reward reward) {
//   try {
//     _rwLock.EnterWriteLock();
//   }
//   finally {
//     _rwLock.ExitWriteLock();
//   }
// }
//
// class Reward {
// }

// using ServerCore;

// int cnt = 0;
// var rwLock = new RWLock();
// Task t1 = new(() => {
//   for (int i = 0; i < 100_000; i++) {
//     rwLock.WriteLock();
//     cnt++;
//     rwLock.WriteUnlock();
//   }
// });
// Task t2 = new(() => {
//   for (int i = 0; i < 100_000; i++) {
//     rwLock.WriteLock();
//     rwLock.WriteLock();
//     cnt--;
//     rwLock.WriteUnlock();
//     rwLock.WriteUnlock();
//   }
// });

// t1.Start();
// t2.Start();
// Task.WaitAll(t1, t2);

// Console.Write(cnt);

ServerCore.Server server = new();