using rxtest.Models;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace rxtest.Rx
{
    static public class RxExamples
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
                // print countdown
                .Subscribe((currentSeconds) =>
                {
                    Console.WriteLine(currentSeconds);
                }, () =>
                // when countdown finished
                {
                    Console.WriteLine("Time's up!");
                });
        }

        public static void RxCatFact()
        {
            CatFactQuery catFactQuery = new CatFactQuery();

            Observable
                .FromAsync(() => catFactQuery.Execute())
                .ValidateCatFactLength(70)
                .Retry(3)
                .Catch(Observable.Return(new CatFact() { Content = "CatFact CatFact" }))
                .Subscribe((catfact) =>
                {
                    Console.WriteLine(catfact.Content);
                });
           
        }

        public static IObservable<CatFact> ValidateCatFactLength(this IObservable<CatFact> observable, int maxLength)
        {
            return observable.Select(catfact =>
             {
                 if (catfact.Content.Length > maxLength)
                 {
                     return Observable.Throw<CatFact>(new Exception("CatFact was too long"));
                 }

                 return Observable.Return(catfact);


             })
                .Switch();
        }

        public static void RxPrintOne()
        {
            var o = Observable.Create<int>(o =>
            {
                o.OnNext(1);
                o.OnCompleted();
                return Disposable.Create(() => Console.WriteLine("Disposed"));
            }
            
            );

            o.Subscribe(
                next => Console.WriteLine(next),
                error => Console.WriteLine(error),
                () => Console.WriteLine("Finished")
                );
        }

        public static void RxRangeCounter()
        {
            var throwsAtTwo = Observable.Range(0, 4)
                .Select(i =>
                {
                    if (i == 2)
                        throw new Exception("i is 2");
                    return i;
                });
                
                Func<Exception, IObservable<int>> errorHandler = ex =>
                {
                    Console.WriteLine("Exception: " + ex.Message);

                    if (ex is NullReferenceException)
                        return Observable.Range(9, 2);
                    else
                        throw ex;
                };

            throwsAtTwo
                .Catch(errorHandler)
                .Retry(2)
                .Subscribe(Console.WriteLine);
        } 

        public static async void RxRangeCounter2()
        {
            var throwsAtTwo = Observable.Range(0, 4)
                .Select(i =>
                {
                    if (i == 2)
                        throw new Exception("i is 2");
                    return i;
                });

            Func<Exception, IObservable<int>> errorHandler = ex =>
            {
                Console.WriteLine("Exception: " + ex.Message);

                if (ex is NullReferenceException)
                    return Observable.Range(9, 2);
                else
                    throw ex;
            };

            var signal = Observable.Timer(DateTimeOffset.Now.AddSeconds(1));

            //RetryWhen does exactly what retry does except that we need a trigger observable
            //that is signaling when to retry again
            // Retrywhen catches the exception and waits until the signal emits any value

            throwsAtTwo
                .RetryWhen(_ => signal)
                .Subscribe(Console.WriteLine);

            await Task.Delay(1000);
        }

        public static async void RxHotObservable()
        {

            // used for when we have an external source emitting events
            var source = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1)).Replay(3);

            // after replay we should use connect for the observer that is the result of replay be initialized

            source.Connect();

            await Task.Delay(8000); 

            // connect already starts the subscription and observers can lose notifications until they subscribe

            var observer1 = source.Subscribe(Console.WriteLine);

            var observer2 = source.Subscribe(Console.WriteLine);

        }

         
    }


}
