namespace Devdog.Rucksack
{
    public class NetworkInputValidatorBase
    {
        
        
        protected Result<bool> Failed()
        {
            return new Result<bool>(false, Errors.NetworkValidationFailed);
        }

        protected bool CheckClamped(int val, int min, int max)
        {
            return val >= min && val <= max;
        }

        protected bool CheckMin(int val, int min)
        {
            return val >= min;
        }

        protected bool CheckMax(int val, int max)
        {
            return val <= max;
        }

        protected bool CheckGuidBytes(byte[] bytes)
        {
            return bytes.Length == 16;
        }
    }
}