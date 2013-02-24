using System.Collections.Generic;

namespace BusrRouteLondon.Web.Migration
{
    public interface ICSVParser<T>
    {
        List<T> Parse(string filename);
    }
}