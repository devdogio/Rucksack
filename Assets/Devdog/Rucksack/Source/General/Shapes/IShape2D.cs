
namespace Devdog.Rucksack
{
    public interface IShape2D
    {
        int convexX { get; }
        int convexY { get; }
        
        bool IsBlocking(int x, int y);
        
    }
}