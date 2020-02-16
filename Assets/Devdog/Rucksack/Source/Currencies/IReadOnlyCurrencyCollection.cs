namespace Devdog.Rucksack.Currencies
{
//    public interface IReadOnlyCurrencyCollection<in TCurrencyType> : IReadOnlyCurrencyCollection<TCurrencyType, double>
//    {
//        
//    }
    
    public interface ICurrencyCollection
    {
        
    }
    
    public interface IReadOnlyCurrencyCollection<in TCurrencyType, TPrecision> : ICurrencyCollection
    {
        TPrecision GetAmount(TCurrencyType currency);
        bool Contains(TCurrencyType currency);
        
        CurrencyDecorator<TPrecision>[] ToDecorators();
    }
}