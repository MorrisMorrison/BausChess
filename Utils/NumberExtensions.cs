namespace BausChess.Utils
{
    public static class FloatExtensions
    {
        public static bool CheckRange(this float toCheck, float from, float to){
            return (toCheck >= from && toCheck <= to);
        }    
    }
}