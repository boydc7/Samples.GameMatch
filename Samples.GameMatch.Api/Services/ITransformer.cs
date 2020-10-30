namespace Samples.GameMatch.Api
{
    public interface ITransformer<in TIn, TOut>
        where TIn : class
        where TOut : class
    {
        TOut To(TIn source, TOut existing = null);
    }
}
