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

            // The basic approach here is that every time source produces a value,
            // we actually create a brand new IObservable<int>, which produces exactly two values.
            // It immediately produces the value 1,
            // and then after the specified timespan (2 seconds in these examples) it produces the value -1.

            //  IScheduler is an Rx abstraction for dealing with timing and concurrency.
            //  We need it because we need to be able to generate events after a one second delay,
            //  and that sort of time-driven activity requires a scheduler
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
