using FastGenericNew;

class Dog
{
    public string Sound { get; set; }
    public Dog()
    {

    }
}

class Cat
{
    public string Sound { get; set; }
    public string Color { get; set; }
    public int Lives { get; set; }
}

class Girl
{
    public string Name { get; set; }
    public string FavorityManga { get; set; }
}

class Boy
{
    public string Birthday { get; set; }
    public string FavorityDisneyMovie { get; set; }
}

class Place
{
    public double Spent { get; set; }
    public string Label { get; set; }
}

class Program
{
    public const int ITERATIONS = 60000;

    public static List<Place> places = new List<Place>();

    public static void Main(string[] args)
    {
        Console.WriteLine($"Net  Version: {Environment.Version}");
        Console.WriteLine();
        Console.WriteLine("Initialization = (one line before benchmark) FastNew.CreateInstance<T> (same T of the test)");
        Console.WriteLine("NoInit         = Initialization 'included' in benchmark");
        Console.WriteLine("AfterInit      = Initialization 'not included' in benchmark");
        Console.WriteLine("Common         = Activator.CreateInstance");
        Console.WriteLine();
        Console.WriteLine($"Testing CreateInstance for {ITERATIONS.ToString("#,##")} iterations");

        var threadList = new List<Thread>() {
            TestCommonGenericNew<Dog>(),
            TestFastGenericNew<Dog>(),
            TestFastGenericNewNoInit<Cat>(),
            TestCommonGenericNew<Cat>(),
            TestFastGenericNewNoInit<Dog>(),
            TestFastGenericNewOneTime<Girl>(),
            TestFastGenericNewOneTime<Boy>(),
            TestCommonGenericNew<Girl>(),
            TestCommonGenericNew<Boy>(),
        };

        foreach (var thread in threadList) thread.Start();

        while (threadList.Any(any => any.IsAlive)) ;

        int pos = 0;
        foreach (var place in places.OrderBy(o => o.Spent)) Console.WriteLine($"{++pos}{place.Label}");
    }

    private static Thread TestFastGenericNewOneTime<T>()
    {
        var thread = new Thread(() =>
        {
            var start = DateTime.Now;
            FastNew.CreateInstance<T>();
            var spent = (DateTime.Now - start).TotalMilliseconds;
            var label = ($"# Place, Total time spent on FastNew[One Iter]   with <{typeof(T).Name}>: {spent} mili");
            places.Add(new Place() { Spent = spent, Label = label });
        });

        return thread;
    }

    private static Thread TestFastGenericNewNoInit<T>() where T : new()
    {
        var thread = new Thread(() =>
        {
            var start = DateTime.Now;

            for (var i = 0; i < ITERATIONS; i++)
            {
                FastNew.CreateInstance<T>();
            }
            var spent = (DateTime.Now - start).TotalMilliseconds;
            var label = ($"# Place, Total time spent on FastNew[NoInit]     with <{typeof(T).Name}>: {spent} mili");
            places.Add(new Place() { Spent = spent, Label = label });
        });

        return thread;
    }

    private static Thread TestFastGenericNew<T>() where T : new()
    {
        var thread = new Thread(() =>
        {
            FastGenericNew.FastNew.CreateInstance<T>();
            var start = DateTime.Now;

            for (var i = 0; i < ITERATIONS; i++)
            {
                FastNew.CreateInstance<T>();
            }
            var spent = (DateTime.Now - start).TotalMilliseconds;
            var label = ($"# Place, Total time spent on FastNew[AfterInit]  with <{typeof(T).Name}>: {spent} mili");
            places.Add(new Place() { Spent = spent, Label = label });
        });



        return thread;
    }

    private static Thread TestCommonGenericNew<T>() where T : new()
    {
        var thread = new Thread(() =>
        {
            Activator.CreateInstance<T>();
            var start = DateTime.Now;
            for (var i = 0; i < ITERATIONS; i++)
            {
                var obj = Activator.CreateInstance<T>();
            }
            var spent = (DateTime.Now - start).TotalMilliseconds;
            var label = ($"# Place, Total time spent on Common              with <{typeof(T).Name}>: {spent} mili");
            places.Add(new Place() { Spent = spent, Label = label });
        });

        return thread;
    }
}