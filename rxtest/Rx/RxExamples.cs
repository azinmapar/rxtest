using System.Reactive.Linq;

namespace rxtest.Rx
{
    public class RxExamples
    {

        public static void RxTicker()
        {
            // a simple ticker using rx
            IObservable<long> ticks = Observable.Timer(
                dueTime: TimeSpan.Zero,
                period: TimeSpan.FromSeconds(1));

            // results will be shown only after we subscribe
            ticks.Subscribe(
                ticks => Console.WriteLine($"ticks: {ticks}"));
        }

        public static void RxCountdown(int seconds)
        {
            Observable
                // every second
                .Timer(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1))
                // showing the countdown
                .Select(currentSeconds => seconds - currentSeconds)
                // count until 0
                .TakeWhile(currentSecond => currentSecond > 0)
                // print coumtdown
                .Subscribe((currentSeconds) =>
                {
                    Console.WriteLine(currentSeconds);
                }, () =>
                // when countdown finished
                {
                    Console.WriteLine("Time's up!");
                });
        }

    }
}
