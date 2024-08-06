using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace rxtest.Rx
{
    static class RxExt
    {

        //  once we return to a state of inactivity, we will want to process everything that just happened,
        //  so this operator will produce a list containing every value that the source reported in its most recent flurry of activity.
        public static IObservable<IList<T>> Quiescent<T>(
            this IObservable<T> src,
            TimeSpan minimumInactivityPeriod,
            IScheduler scheduler)
        {
            IObservable<int> onoffs =
                from _ in src
                from delta in
                   Observable.Return(1, scheduler)
                             .Concat(Observable.Return(-1, scheduler)
                                               .Delay(minimumInactivityPeriod, scheduler))
                select delta;
            IObservable<int> outstanding = onoffs.Scan(0, (total, delta) => total + delta);
            IObservable<int> zeroCrossings = outstanding.Where(total => total == 0);
            return src.Buffer(zeroCrossings);
        }
    }

}
