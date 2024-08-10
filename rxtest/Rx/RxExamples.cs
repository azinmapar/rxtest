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

    }
}
