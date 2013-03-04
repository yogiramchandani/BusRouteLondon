using System.Collections.Generic;

namespace BusRouteLondon.Web.Migration
{
    public interface ICSVParser<T>
    {
        List<T> Parse(string filename);
    }
}