namespace Devdog.Rucksack.Currencies
{
    public interface IConvertible<T, TPrecision>
        where T: class
    {
        ConversionTable<T, TPrecision> conversionTable { get; set; }
    }
}