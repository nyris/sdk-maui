using Nyris.Api.Model;

namespace Nyris.Api.Api.RequestOptions;

public class NyrisFilterOption : Options
{

    public readonly List<NyrisFilter> Filters;
    public NyrisFilterOption()
    {
        Filters = new List<NyrisFilter>();
        Enabled = true;
    }

    public void AddFilter(string type, List<string> values)
    {
        Filters.Add(new NyrisFilter
        {
            Type = type,
            Values = values
        });
    }
    
    
    public override void Reset()
    {
        Enabled = true;
        Filters.Clear();
    }
}
