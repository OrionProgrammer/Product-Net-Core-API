using System.Threading;

namespace Api.Business_Rules;

public sealed class ProductCodeSingleton : IProductCodeSingleton
{
    private static ProductCodeSingleton _instance = null;
    private static readonly object chekLock = new();
    private static Object _mutex = new Object();
    private long sequentialNumber;
    
    //private constructor to be thread safe
    public ProductCodeSingleton() { }

    public static ProductCodeSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_mutex) // now I can claim some form of thread safety...
                {
                    if (_instance == null)
                    {
                        _instance = new ProductCodeSingleton();
                    }
                }
            }

            return _instance;
        }
    }

    //format is yyyyMM-### e.g., 202105-023
    public string GenerateProductCode()
    {
        sequentialNumber++;

        return String.Format("{0}{1}{2}", 
            DateTime.Now.Year, 
            DateTime.Now.Month < 10? "0" + DateTime.Now.Month : DateTime.Now.Month,
            sequentialNumber < 100? "0" + sequentialNumber : sequentialNumber);
    }

}