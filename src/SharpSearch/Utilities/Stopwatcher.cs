using System.Diagnostics;

class Stopwatcher
{
    private static void PrintMessage(float time, string unit, string message)
    {
        Console.WriteLine($"{message} {time}{unit}");
    }

    private static void Report(long elapsedMilliseconds, string message)
    {
        float elapsedTime;
        string elapsedTimeUnit;

        if (elapsedMilliseconds < 1000)
        {
            elapsedTime = elapsedMilliseconds;
            elapsedTimeUnit = "ms";
        }
        else
        {
            elapsedTime = (float)elapsedMilliseconds / 1000;
            elapsedTimeUnit = "s";
        }

        PrintMessage(elapsedTime, elapsedTimeUnit, message);
    }

    public static T Time<T>(Func<T> func, string message)
    {
        var w = Stopwatch.StartNew();
        bool finished = true;
        try
        {
            return func();
        }
        catch (Exception)
        {
            finished = false;
            throw;
        }
        finally
        {
            w.Stop();
            if (finished)
                Report(w.ElapsedMilliseconds, message);
        }
    }

    public static void Time(Action action, string message)
    {
        var w = Stopwatch.StartNew();
        bool finished = true;
        try
        {
            action();
        }
        catch (Exception)
        {
            finished = false;
            throw;
        }
        finally
        {
            if (finished)
                Report(w.ElapsedMilliseconds, message);
        }
    }
}